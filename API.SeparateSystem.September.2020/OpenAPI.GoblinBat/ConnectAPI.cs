using System;
using System.Drawing;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Controls;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI
{
    public sealed partial class ConnectAPI : UserControl, ISecuritiesAPI
    {
        void ButtonStartProgressClick(object sender, EventArgs e) => BeginInvoke(new Action(() =>
        {
            axAPI.OnEventConnect += OnEventConnect;
            API = Connect.GetInstance(axAPI);
        }));
        void OnEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e) => BeginInvoke(new Action(() =>
        {
            Console.WriteLine(e.nErrCode);
            Send?.Invoke(this, new SendSecuritiesAPI(FormWindowState.Minimized, new Accounts(axAPI.GetLoginInfo(account))));
        }));
        public AccountInformation SetPrivacy(Privacy privacy)
        {
            return new AccountInformation
            {
                Identity = axAPI.GetLoginInfo(user),
                Account = privacy.Account,
                Name = string.Empty,
                Server = axAPI.GetLoginInfo(server) == mock,
                Nick = axAPI.GetLoginInfo(name)
            };
        }
        public void SetForeColor(Color color) => labelOpenAPI.ForeColor = color;
        public dynamic API
        {
            get; private set;
        }
        public ConnectAPI() => InitializeComponent();
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}