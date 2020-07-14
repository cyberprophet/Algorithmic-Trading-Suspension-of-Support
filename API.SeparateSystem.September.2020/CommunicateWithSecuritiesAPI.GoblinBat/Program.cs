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
        static void Main(string[] args)
        {            
            var secrecy = new Secrecy();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args != null && args.Length > 0)
            {
                var api = secrecy.GetPrivacyShare(args[0]);
                StartProgress(api.Item1 ?? secrecy.GetAPI(GetResult(secrecy.Choose, secrecy.Name), args[0]), api.Item2);
            }
            Process.GetCurrentProcess().Kill();
        }
        static DialogResult GetResult(string choose, string name) => ChooseBox.Show(choose, name, Enum.GetName(typeof(SecuritiesCOM), SecuritiesCOM.OpenAPI), Enum.GetName(typeof(SecuritiesCOM), SecuritiesCOM.XingAPI));
        static void StartProgress(ISecuritiesAPI api, Privacies privacy) => Application.Run(new SecuritiesAPI(privacy, api));
    }
}