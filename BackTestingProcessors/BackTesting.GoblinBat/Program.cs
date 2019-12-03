using System;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BackTesting(new Expectancy().EstimatedTime(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\Log\"))));
        }
    }
}