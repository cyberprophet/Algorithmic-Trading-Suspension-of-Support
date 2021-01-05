using System;
using System.Diagnostics;
using System.Runtime.Versioning;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ShareInvest
{
	public class Program
	{
		[SupportedOSPlatform("windows")]
		public static void Main(string[] args)
		{
			if (Security.Initialize(args))
				CreateHostBuilder().Build().Run();

			GC.Collect();
			Process.GetCurrentProcess().Kill();
		}
		public static IHostBuilder CreateHostBuilder() => Host.CreateDefaultBuilder().ConfigureWebHostDefaults(web => web.UseStartup<Startup>());
	}
}