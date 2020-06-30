using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using AxKHOpenAPILib;

using ShareInvest.Catalog;
using ShareInvest.Catalog.OpenAPI;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.OpenAPI.Catalog;

namespace ShareInvest.OpenAPI
{
    public sealed partial class ConnectAPI : UserControl, ISecuritiesAPI
    {
        void ButtonStartProgressClick(object sender, EventArgs e) => BeginInvoke(new Action(() =>
        {
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveMsg += OnReceiveMsg;
            axAPI.OnReceiveRealData += OnReceiveRealData;
            axAPI.OnReceiveChejanData += OnReceiveChejanData;
            API = Connect.GetInstance(axAPI);
        }));
        void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e) => BeginInvoke(new Action(() =>
        {
            if (e.nErrCode == 0 && string.IsNullOrEmpty(axAPI.KOA_Functions(showAccountWindow, string.Empty)))
                Send?.Invoke(this, new SendSecuritiesAPI(FormWindowState.Minimized, new Accounts(axAPI.GetLoginInfo(account))));

            else
            {

            }
        }));
        void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e) => BeginInvoke(new Action(() =>
        {
            if (Connect.Chejan.TryGetValue(e.sGubun, out Chejan chejan))
                chejan.OnReceiveChejanData(e);
        }));
        void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e) => BeginInvoke(new Action(() => Connect.Real.First(o => o.GetType().Name.Equals(e.sRealType)).OnReceiveRealData(e)));
        void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e) => Send?.Invoke(this, new SendSecuritiesAPI(e.sMsg.Substring(9)));
        public AccountInformation SetPrivacy(Privacy privacy)
        {
            var tr = new Opw00005
            {
                Value = string.Concat(privacy.Account, password),
                PrevNext = 0,
                API = axAPI
            };
            if (Connect.TR.Add(tr))
                axAPI.OnReceiveTrData += tr.OnReceiveTrData;

            return new AccountInformation
            {
                Identity = axAPI.GetLoginInfo(user),
                Account = privacy.Account,
                Name = string.Empty,
                Server = axAPI.GetLoginInfo(server) == mock,
                Nick = axAPI.GetLoginInfo(name)
            };
        }
        public ISendSecuritiesAPI InputValueRqData(bool input, string name)
        {
            var ctor = Connect.TR.First(o => o.GetType().Name.Equals(name));

            if (input)
                BeginInvoke(new Action(() =>
                {
                    if (API is Connect api)
                        api.InputValueRqData(ctor);
                }));
            return ctor;
        }
        public ISendSecuritiesAPI OnConnectErrorMessage => API as Connect;
        public void SetForeColor(Color color) => labelOpenAPI.ForeColor = color;
        public dynamic API
        {
            get; private set;
        }
        public ConnectAPI() => InitializeComponent();
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}