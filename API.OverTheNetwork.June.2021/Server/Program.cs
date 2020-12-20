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
			{
				Process.Start("shutdown.exe", "-r");
				Base.SendMessage(Security.api);
			}
			else if (security.GetContextAsync(args).Result.GrantAccess)
				CreateHostBuilder().Build().Run();

			GC.Collect();
			Process.GetCurrentProcess().Kill();
		}
		public static IHostBuilder CreateHostBuilder()
			=> Host.CreateDefaultBuilder().ConfigureWebHostDefaults(web => web.UseStartup<Startup>());
	}
}