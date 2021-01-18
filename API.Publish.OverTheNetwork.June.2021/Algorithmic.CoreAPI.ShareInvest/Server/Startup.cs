using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ShareInvest.Filter;
using ShareInvest.Hubs;

namespace ShareInvest
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;
		public IConfiguration Configuration
		{
			get;
		}
		public void ConfigureServices(IServiceCollection services)
		{
			JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
			services.AddDbContext<CoreApiDbContext>(o => o.UseSqlServer(Configuration[Crypto.Security.Connection], o => o.MigrationsAssembly("Context"))).AddDatabaseDeveloperPageExceptionFilter();
			services.AddSignalR().AddHubOptions<HermesHub>(o =>
			{
				var wait = 0x2800 * 3;
				o.ClientTimeoutInterval = TimeSpan.FromMilliseconds(wait);
				o.HandshakeTimeout = TimeSpan.FromMilliseconds(wait / 3);
				o.KeepAliveInterval = TimeSpan.FromMilliseconds(wait / 3);
				o.EnableDetailedErrors = true;
			});
			services.AddDefaultIdentity<Models.CoreUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<CoreApiDbContext>().AddDefaultTokenProviders();
			services.AddResponseCompression(o => o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" })).Configure<KestrelServerOptions>(o =>
			{
				o.ListenAnyIP(0x1BDF);
				o.Limits.MaxRequestBodySize = int.MaxValue;
			})
				.AddSingleton<HermesHub>().AddScoped<BalanceHub>().AddScoped<MessageHub>()
				.AddScoped(container => new ClientIpCheckActionFilter(Configuration["AdminSafeList"], container.GetRequiredService<ILoggerFactory>().CreateLogger<ClientIpCheckActionFilter>()))
				.AddControllersWithViews(o => o.InputFormatters.Insert(0, GetJsonPatchInputformatter())).AddMvcOptions(o => o.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Latest);
			services.AddIdentityServer().AddApiAuthorization<Models.CoreUser, CoreApiDbContext>();
			services.AddAuthentication().AddIdentityServerJwt();
			services.AddRazorPages();
		}
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseMvc();
				app.UseDeveloperExceptionPage().UseMigrationsEndPoint().UseWebAssemblyDebugging();
				app.UseHttpsRedirection();
			}
			else
				app.UseMvc().UseExceptionHandler("/Error");

			app.UseResponseCompression().UseBlazorFrameworkFiles().UseStaticFiles();
			app.UseRouting();
			app.UseIdentityServer();
			app.UseAuthentication().UseAuthorization();
			app.UseEndpoints(ep =>
			{
				ep.MapRazorPages();
				ep.MapHub<MessageHub>("/hub/message");
				ep.MapHub<BalanceHub>("/hub/balance");
				ep.MapHub<HermesHub>("/hub/hermes", o => o.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling);
				ep.MapControllers();
				ep.MapFallbackToFile("index.html");
			});
		}
		static NewtonsoftJsonInputFormatter GetJsonPatchInputformatter() => new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson().Services.BuildServiceProvider().GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters.OfType<NewtonsoftJsonPatchInputFormatter>().First();
	}
}