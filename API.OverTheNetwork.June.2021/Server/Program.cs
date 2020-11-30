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
			var security = new Security(args);			

			if (Progress.GetUpdateVisionAsync().Result)
				Process.Start("shutdown.exe", "-r");

			else
			{
				var connect = CreateHostBuilder().Build();

				if (security.GetContextAsync(args, connect).Result.GrantAccess)
					connect.Run();

				if (connect != null)
				{
					connect.StopAsync(new TimeSpan(0x3E8)).Wait();
					connect.Dispose();
				}
			}
			GC.Collect();
			Process.GetCurrentProcess().Kill();
		}
		public static IHostBuilder CreateHostBuilder() => Host.CreateDefaultBuilder().ConfigureWebHostDefaults(web => web.UseStartup<Startup>());
	}
}