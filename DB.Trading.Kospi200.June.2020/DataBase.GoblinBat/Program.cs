using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using ShareInvest.Message;

namespace ShareInvest.GoblinBatForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            int remaining;
            var registry = Registry.CurrentUser.OpenSubKey(new Message().Path);

            if (registry.GetValue(new Message().GoblinBat) == null)
            {
                registry.Close();
                registry = Registry.CurrentUser.OpenSubKey(new Message().Path, true);
                registry.SetValue(new Message().GoblinBat, Array.Find(Directory.GetFiles(Application.StartupPath, "*.exe", SearchOption.AllDirectories), o => o.Contains(string.Concat(new Message().GoblinBat, ".exe"))));
            }
            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday) && DateTime.Now.Hour > 3 && DateTime.Now.Hour < 5)
                remaining = 30;

            else if (DateTime.Now.Hour > 3 && DateTime.Now.Hour < 5)
                remaining = 5;

            else
                remaining = 1;

            while (remaining > 0)
                TimerBox.Show(new Message(remaining--).RemainingTime, new Message().GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Information, 60000U);

            TimerBox.Show(new Message().StartProgress, new Message().GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Information, 3765U);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GoblinBat());
        }
    }
}