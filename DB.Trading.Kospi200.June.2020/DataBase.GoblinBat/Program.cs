using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
            var remaining = new Random(new Random().Next(0, Application.StartupPath.Length)).Next(3, 10);
            var registry = Registry.CurrentUser.OpenSubKey(new Secret().Path);
            var secret = new Secret();
            var str = KeyDecoder.GetWindowsProductKeyFromRegistry();
            var initial = secret.GetPort(str);

            if (registry.GetValue(secret.GoblinBat) == null)
            {
                registry.Close();
                registry = Registry.CurrentUser.OpenSubKey(new Secret().Path, true);
                registry.SetValue(secret.GoblinBat, Array.Find(Directory.GetFiles(Application.StartupPath, "*.exe", SearchOption.AllDirectories), o => o.Contains(string.Concat(secret.GoblinBat, ".exe"))));
            }
            while (remaining > 0)
                if (TimerBox.Show(new Secret(remaining--).RemainingTime, secret.GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Information, 60000U).Equals(DialogResult.OK) && remaining == 0)
                {
                    new Task(() => new Strategy.Retrieve(str).SetInitialzeTheCode()).Start();
                }
            while (DateTime.Now.Hour > 15 || DateTime.Now.Hour < 9 || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                if ((DateTime.Now.Hour == 8 || DateTime.Now.Hour == 17) && DateTime.Now.Minute > 35)
                    break;

                else if (TimerBox.Show(secret.StartProgress, secret.GoblinBat, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, 3765U).Equals(DialogResult.OK))
                    break;

                Thread.Sleep(30000);
            }
            if (initial.Equals((char)126) == false)
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