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

                if (StartProgress().Repeat)
                {

                }
            }
            Process.GetCurrentProcess().Kill();
        }
        static SecuritiesAPI StartProgress()
        {
            var api = new SecuritiesAPI(new OpenAPI.ConnectAPI());
            Application.Run(api);
            GC.Collect();

            return api;
        }
    }
}