using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShareInvest.BackTesting
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BackTesting(Process.GetCurrentProcess().Threads.Count));
        }
    }
}