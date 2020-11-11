using System;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace ShareInvest
{
    class Program
    {
        [SupportedOSPlatform("windows")]
        static void Main()
        {
            var security = new Security(Verify.KeyDecoder.ProductKeyFromRegistry);

            if (security.GrantAccess && security.CheckTheSystemPeriodically(DateTime.Now))
                security.StartProgress();

            GC.Collect();
            Process.GetCurrentProcess().Kill();
        }
    }
}