using System.Runtime.Versioning;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using ShareInvest;

namespace ShareInvet
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
        }
        public static IWebHostBuilder CreateHostBuilder() => WebHost.CreateDefaultBuilder().UseStartup<Startup>();
    }
}