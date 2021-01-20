using System;
using System.Diagnostics;
using System.Runtime.Versioning;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ShareInvest.Hubs;

namespace ShareInvest
{
	public class Program
	{
		[SupportedOSPlatform("windows")]
		public static void Main(string[] args)
		{
			var initialize = Security.Initialize(args);

			if (initialize.Item1)
			{
				var host = CreateHostBuilder(args).Build();
				initialize.Item2.Send += host.Services.GetService<HermesHub>().OnReceiveSecuritiesAPI;
				Security.Host = host;
				GC.Collect();
				host.Run();

				if (Base.IsDebug is false)
					Process.Start("shutdown.exe", "-r");
			}
			Process.GetCurrentProcess().Kill();
		}
		public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(web => web.UseStartup<Startup>());
	}
}