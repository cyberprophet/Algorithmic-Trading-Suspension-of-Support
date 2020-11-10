using System;
using System.Diagnostics;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using ShareInvest;

namespace ShareInvet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (new Security(args).GrantAccess)
                CreateHostBuilder().Build().Run();

            else
            {
                GC.Collect();
                Process.GetCurrentProcess().Kill();
            }
        }
        public static IWebHostBuilder CreateHostBuilder() => WebHost.CreateDefaultBuilder().UseStartup<Startup>();
    }
}