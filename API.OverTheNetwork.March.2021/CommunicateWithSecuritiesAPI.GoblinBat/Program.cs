using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShareInvest
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            if (Application.SetHighDpiMode(HighDpiMode.SystemAware))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                StartProgress();
            }
            Process.GetCurrentProcess().Kill();
        }
        static void StartProgress()
        {
            var api = new SecuritiesAPI();
            Application.Run(api);
            GC.Collect();

            if (api.Repeat)
            {

            }
        }
    }
}