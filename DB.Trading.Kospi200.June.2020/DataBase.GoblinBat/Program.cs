using System;
using System.IO;
using System.Runtime.InteropServices;
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
            var secret = new Secret();
            var handle = GetConsoleWindow();
            ShowWindow(handle, secret.Hide);
            var remaining = new Random(new Random().Next(0, Application.StartupPath.Length)).Next(3, 10);
            var registry = Registry.CurrentUser.OpenSubKey(new Secret().Path);
            var str = KeyDecoder.GetWindowsProductKeyFromRegistry();

            if (secret.GetIdentify(str))
            {
                var path = Path.Combine(Application.StartupPath, secret.Indentify);

                if (secret.GetDirectoryInfoExists(path))
                {
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
                            new Strategy.Retrieve(str).SetInitialzeTheCode();
                        }
                    while (DateTime.Now.Hour < 18 && DateTime.Now.Hour > 15 || DateTime.Now.Hour > 4 && DateTime.Now.Hour < 9 || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday))
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
                else
                {
                    ShowWindow(handle, secret.Show);
                    secret.SetIndentify(path, str);
                }
            }
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}