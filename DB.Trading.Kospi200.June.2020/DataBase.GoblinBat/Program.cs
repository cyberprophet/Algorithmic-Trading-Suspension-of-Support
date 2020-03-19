using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using ShareInvest.Message;
using ShareInvest.Verify;

namespace ShareInvest
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            int remaining;
            var registry = Registry.CurrentUser.OpenSubKey(new Secret().Path);
            var secret = new Secret();

            if (registry.GetValue(secret.GoblinBat) == null)
            {
                registry.Close();
                registry = Registry.CurrentUser.OpenSubKey(new Secret().Path, true);
                registry.SetValue(secret.GoblinBat, Array.Find(Directory.GetFiles(Application.StartupPath, "*.exe", SearchOption.AllDirectories), o => o.Contains(string.Concat(secret.GoblinBat, ".exe"))));
            }
            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday) && DateTime.Now.Hour > 3 && DateTime.Now.Hour < 5)
                remaining = new Random(new Random().Next(0, Application.StartupPath.Length)).Next(30, 36);

            else if (DateTime.Now.Hour > 3 && DateTime.Now.Hour < 5)
                remaining = new Random(new Random().Next(0, Application.StartupPath.Length)).Next(5, 11);

            else
                remaining = new Random(new Random().Next(0, Application.StartupPath.Length)).Next(1, 6);

            while (remaining > 0)
                TimerBox.Show(new Secret(remaining--).RemainingTime, secret.GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Information, 60000U);

            TimerBox.Show(secret.StartProgress, secret.GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Information, 3765U);
            var str = KeyDecoder.GetWindowsProductKeyFromRegistry();

            if (secret.Connect.TryGetValue(str, out char initial))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new GoblinBat(initial, secret));
            }
            else
                new ExceptionMessage(str);
        }
    }
}