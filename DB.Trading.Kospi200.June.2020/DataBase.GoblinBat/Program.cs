using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ShareInvest.DataBase
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            int remaining;

            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday))
                remaining = 30;

            else
                remaining = 5;

            do
            {
                Thread.Sleep(60000);
                remaining--;
            }
            while (remaining > 0);

            string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", bat = "GoblinBat";
            var registry = Registry.CurrentUser.OpenSubKey(path);

            if (registry.GetValue(bat) != null)
            {
                registry.Close();
                registry = Registry.CurrentUser.OpenSubKey(path, true);
                registry.DeleteValue(bat);
            }
            registry.Close();
            registry = Registry.CurrentUser.OpenSubKey(path, true);
            registry.SetValue(bat, Array.Find(Directory.GetFiles(Application.StartupPath, "*.exe", SearchOption.AllDirectories), o => o.Contains(string.Concat(bat, ".exe"))));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GoblinBat());
        }
    }
}