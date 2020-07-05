using System;
using System.Diagnostics;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Message;

namespace ShareInvest
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var secrecy = new Secrecy();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var api = secrecy.GetPrivacyShare();

            if (api.Item1 != null)
                StartProgress(api.Item1, api.Item2);

            else
                StartProgress(secrecy.GetAPI(ChooseBox.Show(secrecy.Choose, secrecy.Name, Enum.GetName(typeof(SecuritiesCOM), SecuritiesCOM.OpenAPI), Enum.GetName(typeof(SecuritiesCOM), SecuritiesCOM.XingAPI))));

            Process.GetCurrentProcess().Kill();
        }
        static void StartProgress(ISecuritiesAPI api) => Application.Run(new SecuritiesAPI(api));
        static void StartProgress(ISecuritiesAPI api, Models.Privacy privacy) => Application.Run(new SecuritiesAPI(privacy, api));
    }
}