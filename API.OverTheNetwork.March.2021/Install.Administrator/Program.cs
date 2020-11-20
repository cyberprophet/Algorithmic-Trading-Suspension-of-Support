using System;
using System.Diagnostics;
using System.Security.Principal;

namespace ShareInvest
{
    class Program
    {
        static void Main()
        {
            if (Firewall.IsInboundRuleExist(Firewall.Name, Protocol.Tcp, Firewall.Port))
                StartProgress();

            else if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator) && Firewall.AddInboudRule(Firewall.Name, Protocol.Tcp, Firewall.Port))
            {

                StartProgress();
            }
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