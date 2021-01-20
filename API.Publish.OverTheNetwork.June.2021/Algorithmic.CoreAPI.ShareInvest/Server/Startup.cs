using System;
using System.Linq;
using System.Net.Mime;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
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
using Microsoft.IdentityModel.Tokens;

using ShareInvest.Filter;
using ShareInvest.Hubs;
using ShareInvest.Interface.Server;
using ShareInvest.Models;
using ShareInvest.Service;

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
			services.AddDbContext<CoreApiDbContext>(o => o.UseSqlServer(Configuration[Crypto.Security.Connection], o => o.MigrationsAssembly("Context"))).AddDatabaseDeveloperPageExceptionFilter();
			services.AddSignalR().AddHubOptions<HermesHub>(o =>
			{
				var wait = 0x2800 * 3;
				o.ClientTimeoutInterval = TimeSpan.FromMilliseconds(wait);
				o.HandshakeTimeout = TimeSpan.FromMilliseconds(wait / 3);
				o.KeepAliveInterval = TimeSpan.FromMilliseconds(wait / 3);
				o.EnableDetailedErrors = true;
			});
			services.AddResponseCompression(o => o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { MediaTypeNames.Application.Octet })).Configure<KestrelServerOptions>(o =>
			{
				o.ListenAnyIP(0x1BDF);
				o.Limits.MaxRequestBodySize = int.MaxValue;
			})
				.AddTransient<IJwtTokenService, JwtTokenService>()
				.AddSingleton<HermesHub>().AddScoped<BalanceHub>().AddScoped<MessageHub>()
				.AddScoped(container => new ClientIpCheckActionFilter(Configuration["AdminSafeList"], container.GetRequiredService<ILoggerFactory>().CreateLogger<ClientIpCheckActionFilter>()))
				.AddControllersWithViews(o => o.InputFormatters.Insert(0, GetJsonPatchInputformatter())).AddMvcOptions(o => o.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Latest);
			services.AddDefaultIdentity<CoreUser>().AddEntityFrameworkStores<CoreApiDbContext>();
			services.AddAuthentication(o =>
			{
				o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = Configuration["Jwt:Issuer"],
					ValidAudience = Configuration["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
				});
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
			app.UseAuthentication();
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