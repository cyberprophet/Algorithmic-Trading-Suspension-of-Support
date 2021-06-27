using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ShareInvest.Hubs;

using System;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

			app.UseWebSockets(new Microsoft.AspNetCore.Builder.WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(0x80) }).Use(async (context, next) =>
			{
				if (context.Request.Path.Equals("/socket"))
				{
					if (context.WebSockets.IsWebSocketRequest)
						using (var socket = await context.WebSockets.AcceptWebSocketAsync())
						{
							Token = new CancellationToken();
							await Echo(context, socket);
						}
					else
						context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				}
				else
					await next();

			}).UseFileServer().UseResponseCompression().UseHttpsRedirection().UseBlazorFrameworkFiles().UseStaticFiles().UseRouting().UseIdentityServer().UseAuthentication().UseAuthorization().UseEndpoints(ep =>
			{
				ep.MapRazorPages();
				ep.MapHub<MessageHub>("/hub/message");
				ep.MapHub<BalanceHub>("/hub/balance");
				ep.MapHub<HermesHub>("/hub/hermes", o => o.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling);
				ep.MapHub<ChatHub>("/hub/chat");
				ep.MapControllers();
				ep.MapFallbackToFile("index.html");
			});
		}
		async Task Echo(HttpContext context, WebSocket socket)
		{
			var seg = new ArraySegment<byte>(new byte[0x400 * 4]);
			var result = await socket.ReceiveAsync(seg, Token);

			if (result.Count > 0 && result.EndOfMessage && result.MessageType.Equals(WebSocketMessageType.Text))
			{
				var message = Encoding.UTF8.GetString(seg.Array, seg.Offset, result.Count);

				if (Security.User.TryGetValue(message, out Catalog.Models.User ws))
				{
					ws.Socket = socket;

					if (Base.IsDebug)
						Base.SendMessage(context.GetType(), message, context.Connection.RemoteIpAddress);
				}
				while (result.CloseStatus.HasValue is false)
				{
					result = await socket.ReceiveAsync(seg, Token);

					if (result.EndOfMessage)
						switch (result.MessageType)
						{
							case WebSocketMessageType.Text when result.Count > 0:
								message = Encoding.UTF8.GetString(seg.Array, seg.Offset, result.Count);
								break;

							case WebSocketMessageType.Close when result.Count == 0 && Security.User.TryGetValue(result.CloseStatusDescription, out Catalog.Models.User user):
								user.Socket = null;
								message = result.CloseStatusDescription;
								break;

							default:
								message = string.Empty;
								break;
						}
					if (Base.IsDebug && string.IsNullOrEmpty(message) is false)
						Base.SendMessage(context.GetType(), message, context.Connection.RemoteIpAddress);
				}
				await socket.CloseOutputAsync(result.CloseStatus.Value, result.CloseStatusDescription, Token);
			}
		}
		CancellationToken Token
		{
			get; set;
		}
	}
}