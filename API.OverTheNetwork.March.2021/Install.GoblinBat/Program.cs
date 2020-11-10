using System;
using System.Diagnostics;

namespace ShareInvest
{
    class Program
    {
        static void Main()
        {
            var key = new Verify.KeyDecoder().ProductKeyFromRegistry;
            var security = new Security(key);

            if (string.IsNullOrEmpty(key) == false && security.GrantAccess && security.CheckTheSystemPeriodically(DateTime.Now))
                security.StartProgress();

            GC.Collect();
            Process.GetCurrentProcess().Kill();
        }
    }
}