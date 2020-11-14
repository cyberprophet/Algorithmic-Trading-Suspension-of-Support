using System;
using System.Diagnostics;

namespace ShareInvest
{
    class Program
    {
        static bool IsDebug
        {
            get; set;
        }
        static void Main()
        {
            ChangePropertyToDebugMode();

            if (IsDebug)
                Security.ChooseTheInstallationPath(Console.ReadLine());

            else
            {

            }
            GC.Collect();
            Process.GetCurrentProcess().Kill();
        }
        [Conditional("DEBUG")]
        static void ChangePropertyToDebugMode() => IsDebug = true;
    }
}