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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SecuritiesAPI(new Secrecy().GetAPI((char)SecuritiesCOM.XingAPI)));
            Process.GetCurrentProcess().Kill();
        }
    }
}