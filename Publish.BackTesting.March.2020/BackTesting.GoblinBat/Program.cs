using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ShareInvest.EstimatedTime;

namespace ShareInvest.BackTesting
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            string[] temp;

            foreach (string file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "*.lnk", SearchOption.TopDirectoryOnly))
            {
                temp = file.Split('\\');
                temp = temp[temp.Length - 1].Split('.');

                if (temp[0].Equals("BackTesting"))
                    Operation = true;
            }
            if (Operation)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new BackTesting(new Expectancy().EstimatedTime(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\Log\"))));

                return;
            }
            MessageBox.Show("You didn't Agree to the 'GoblinBat' program\nTerms and Conditions.\n\nAccept the Terms and Conditions\non the Installation Screen.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Process.Start(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\Install.exe"));
            Application.Exit();
        }
        static bool Operation
        {
            get; set;
        }
    }
}