using System;
using System.Diagnostics;
using System.Runtime.Versioning;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ShareInvest
{
    public class Program
    {
        [SupportedOSPlatform("windows")]
        public static void Main(string[] args)
        {
            Security.ChangePropertyToDebugMode();
            var host = CreateHostBuilder().Build();

            if (new Security(args).GetContextAsync(args, host).Result.GrantAccess)
                host.Run();

            if (host != null)
            {
                host.StopAsync(new TimeSpan(0x3E8)).Wait();
                host.Dispose();
            }
            GC.Collect();
            Security.KillTheConnectedProcess();
            Process.GetCurrentProcess().Kill();
        }
        public static IWebHostBuilder CreateHostBuilder() => WebHost.CreateDefaultBuilder().UseStartup<Startup>();
    }
}