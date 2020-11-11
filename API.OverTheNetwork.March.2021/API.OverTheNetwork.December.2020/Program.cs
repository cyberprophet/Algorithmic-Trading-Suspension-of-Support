using System;
using System.Diagnostics;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using ShareInvest;
using ShareInvest.Catalog.Models;

namespace ShareInvet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (new Security(args).GrantAccess)
            {
                if (Security.Client.GetContextAsync(args).Result is Privacies privacy)
                    Security.SetPrivacy(privacy);

                CreateHostBuilder().Build().Run();
            }
            else
            {
                GC.Collect();
                Process.GetCurrentProcess().Kill();
            }
        }
        public static IWebHostBuilder CreateHostBuilder() => WebHost.CreateDefaultBuilder().UseStartup<Startup>();
    }
}