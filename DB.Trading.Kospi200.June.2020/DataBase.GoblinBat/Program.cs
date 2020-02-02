using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using ShareInvest.Message;
using ShareInvest.Strategy;

namespace ShareInvest.GoblinBatForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            int remaining;

            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday) && DateTime.Now.Hour > 3 && DateTime.Now.Hour < 5)
                remaining = 30;

            else if (DateTime.Now.Hour > 3 && DateTime.Now.Hour < 5)
                remaining = 5;

            else
                remaining = 1;

            do
            {
                TimerBox.Show(new Message(remaining--).RemainingTime, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, 60000U);
            }
            while (remaining > 0);

            new Task(() =>
            {
                string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", bat = "GoblinBat";
                var registry = Registry.CurrentUser.OpenSubKey(path);

                if (registry.GetValue(bat) == null)
                {
                    registry.Close();
                    registry = Registry.CurrentUser.OpenSubKey(path, true);
                    registry.SetValue(bat, Array.Find(Directory.GetFiles(Application.StartupPath, "*.exe", SearchOption.AllDirectories), o => o.Contains(string.Concat(bat, ".exe"))));
                }
                new BackTesting(35000000L);
            }).Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GoblinBat());
        }
    }
}