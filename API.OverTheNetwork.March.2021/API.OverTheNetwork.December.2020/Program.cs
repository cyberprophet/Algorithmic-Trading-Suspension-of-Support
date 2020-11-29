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
            Security.ChangePropertyToDebugMode();
            var security = new Security(args);

            if (Security.GetUpdateVisionAsync().Result)
                Process.Start("shutdown.exe", "-r");

            else
            {
                var host = CreateHostBuilder().Build();

                if (security.GetContextAsync(args, host).Result.GrantAccess)
                    host.Run();

                if (host != null)
                {
                    host.StopAsync(new TimeSpan(0x3E8)).Wait();
                    host.Dispose();
                }
            }
            GC.Collect();
            Security.KillTheConnectedProcess();
            Process.GetCurrentProcess().Kill();
        }
        public static IHostBuilder CreateHostBuilder() => Host.CreateDefaultBuilder().ConfigureWebHostDefaults(web => web.UseStartup<Startup>());
    }
}