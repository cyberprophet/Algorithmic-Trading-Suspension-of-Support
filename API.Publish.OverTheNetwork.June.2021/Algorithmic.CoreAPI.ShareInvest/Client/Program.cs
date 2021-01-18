using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ShareInvest
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");
			builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
			builder.Services.AddHttpClient(Crypto.Security.Route, o => o.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)).AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
			builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(Crypto.Security.Route));
			builder.Services.AddApiAuthorization();
			await builder.Build().RunAsync();
		}
	}
}