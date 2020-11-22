using System;
using System.Diagnostics;

namespace ShareInvest
{
    class Program
    {
        static void Main()
        {
            if (Security.DirectoryInfo && Firewall.AddInboudRule(Firewall.Name, Protocol.Tcp, Firewall.Port) && Security.UpdateToVersion(DateTime.Now.AddDays(-1)))
                StartProgress();

            else
                Process.Start(Security.StartInfo);
        }
        static void StartProgress()
        {
            var security = new Security(Verify.KeyDecoder.ProductKeyFromRegistry);

            if (security.GrantAccess && security.CheckTheSystemPeriodically(DateTime.Now))
                security.StartProgress();

            GC.Collect();
            Process.GetCurrentProcess().Kill();
        }
    }
}