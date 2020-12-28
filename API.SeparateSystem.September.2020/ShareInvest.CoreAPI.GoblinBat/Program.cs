using System;
using System.Diagnostics;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ShareInvest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (Security.GetGrantAccess(args))
				CreateHostBuilder().Build().Run();

			else
			{
				GC.Collect();
				Process.GetCurrentProcess().Kill();
			}
		}
		public static IWebHostBuilder CreateHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
		public static IHostBuilder CreateHostBuilder() => Host.CreateDefaultBuilder().ConfigureWebHostDefaults(web => web.UseStartup<Startup>());
	}
}