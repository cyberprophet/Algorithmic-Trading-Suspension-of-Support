using System;
using System.Diagnostics;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Client;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Message;

namespace ShareInvest
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var secrecy = new Secrecy();
            //args = secrecy.Administrator;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args != null && args.Length > 0)
            {
                var api = secrecy.GetPrivacyShare(args[0]);
                StartProgress(GoblinBatClient.GetInstance(args), api.Item1 ?? secrecy.GetAPI(GetResult(secrecy.Choose, secrecy.Name), args[0]), api.Item2);
            }
            GC.Collect();
            Process.GetCurrentProcess().Kill();
        }
        static DialogResult GetResult(string choose, string name) => ChooseBox.Show(choose, name, Enum.GetName(typeof(SecuritiesCOM), SecuritiesCOM.OpenAPI), Enum.GetName(typeof(SecuritiesCOM), SecuritiesCOM.XingAPI));
        static void StartProgress(dynamic client, ISecuritiesAPI<SendSecuritiesAPI> api, Privacies privacy) => Application.Run(new SecuritiesAPI(new Consensus(privacy.Security), client, privacy, api));
    }
}