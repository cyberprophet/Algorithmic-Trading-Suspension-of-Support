using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShareInvest
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (Application.SetHighDpiMode(HighDpiMode.SystemAware))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (Security.CheckAccessRights(args))
                    StartProgress(args);
            }
            GC.Collect();
            Process.GetCurrentProcess().Kill();
        }
        static void StartProgress(dynamic param) => Application.Run(new SecuritiesAPI(param, new OpenAPI.ConnectAPI()));
    }
}