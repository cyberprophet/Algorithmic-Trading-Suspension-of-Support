using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
			services.AddSignalR().AddHubOptions<HermesHub>(o =>
			{
				var wait = 0x2800 * 3;
				o.ClientTimeoutInterval = TimeSpan.FromMilliseconds(wait);
				o.HandshakeTimeout = TimeSpan.FromMilliseconds(wait / 3);
				o.KeepAliveInterval = TimeSpan.FromMilliseconds(wait / 3);
				o.EnableDetailedErrors = true;
			});
			services.AddDbContext<CoreApiDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped).AddDatabaseDeveloperPageExceptionFilter().AddResponseCompression(o => o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" })).AddSingleton<HermesHub>().AddScoped<BalanceHub>().AddScoped<MessageHub>().Configure<KestrelServerOptions>(o =>
			{
				o.ListenAnyIP(0x1BDF, o => o.UseHttps(StoreName.My, "coreapi.shareinvest.net", true).UseConnectionLogging());
				o.Limits.MaxRequestBodySize = int.MaxValue;
			});
			services.AddDefaultIdentity<Models.CoreUser>(o => o.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<CoreApiDbContext>();
			services.AddIdentityServer().AddApiAuthorization<Models.CoreUser, CoreApiDbContext>();
			services.AddAuthentication().AddIdentityServerJwt();
			services.AddControllersWithViews();
			services.AddRazorPages();
		}
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage().UseMigrationsEndPoint().UseWebAssemblyDebugging();

			else
				app.UseExceptionHandler("/Error").UseHsts();

			app.UseHttpsRedirection();
			app.UseBlazorFrameworkFiles().UseStaticFiles();
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
	}
}