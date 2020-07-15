using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
            Start = true;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveMsg += OnReceiveMsg;
            API = Connect.GetInstance(axAPI);
        }));
        void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e) => BeginInvoke(new Action(() =>
        {
            if (e.nErrCode == 0 && (string.IsNullOrEmpty(privacy.SecurityAPI) == false || string.IsNullOrEmpty(axAPI.KOA_Functions(showAccountWindow, string.Empty))))
                Send?.Invoke(this, new SendSecuritiesAPI(FormWindowState.Minimized, securites != null && securites.Length == 0xA ? new Accounts(securites) : new Accounts(axAPI.GetLoginInfo(account))));

            else
                (API as Connect)?.SendErrorMessage(e.nErrCode);
        }));
        void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            if (Connect.Chejan.TryGetValue(e.sGubun, out Chejan chejan))
                chejan.OnReceiveChejanData(e);
        }
        void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e) => GetRequestTR(string.Concat(e.sTrCode.Substring(0, 1).ToUpper(), e.sTrCode.Substring(1)))?.OnReceiveTrData(e);
        void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e) => Connect.Real.FirstOrDefault(o => o.GetType().Name.Replace('_', ' ').Equals(e.sRealType))?.OnReceiveRealData(e);
        void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e) => Send?.Invoke(this, new SendSecuritiesAPI(string.Concat("[", e.sRQName, "] ", e.sMsg.Substring(9), "(", e.sScrNo, ")")));
        TR GetRequestTR(string name) => Connect.TR.FirstOrDefault(o => o.GetType().Name.Equals(name)) ?? null;
        public AccountInformation SetPrivacy(Privacies privacy)
        {
            if (Connect.TR.Add(new OPT50010 { PrevNext = 0, API = axAPI }) && Connect.TR.Add(new Opt50001 { PrevNext = 0, API = axAPI }) && Connect.TR.Add(new Opw00005 { Value = string.Concat(privacy.AccountNumber, password), PrevNext = 0, API = axAPI }))
            {
                axAPI.OnReceiveTrData += OnReceiveTrData;
                axAPI.OnReceiveRealData += OnReceiveRealData;
                axAPI.OnReceiveChejanData += OnReceiveChejanData;
            }
            var mServer = axAPI.GetLoginInfo(server);
            checkAccount.CheckState = mServer.Equals(mock) && checkAccount.Checked ? CheckState.Unchecked : CheckState.Checked;
            string strAccount;

            if (checkAccount.Checked && new Security().Encrypt(this.privacy.Security, this.privacy.SecuritiesAPI, privacy.AccountNumber, checkAccount.Checked) == 0xC8)
                switch (privacy.AccountNumber.Substring(privacy.AccountNumber.Length - 2))
                {
                    case "31":
                        strAccount = "선물옵션";
                        break;

                    default:
                        strAccount = "위탁종합";
                        break;
                }
            else
                strAccount = string.Empty;

            return new AccountInformation
            {
                Identity = axAPI.GetLoginInfo(user),
                Account = privacy.AccountNumber,
                Name = strAccount,
                Server = mServer.Equals(mock),
                Nick = axAPI.GetLoginInfo(name)
            };
        }
        public ISendSecuritiesAPI InputValueRqData(string name, string sArrCode, int nCodeCount)
        {
            var ctor = GetRequestTR(name);

            if (string.IsNullOrEmpty(sArrCode) == false && nCodeCount > 0 && API is Connect api)
            {
                ctor.Value = sArrCode;
                api.InputValueRqData(nCodeCount, ctor);
            }
            return ctor ?? null;
        }
        public ISendSecuritiesAPI InputValueRqData(string name, string param)
        {
            var ctor = GetRequestTR(name);

            if (string.IsNullOrEmpty(param) == false)
                BeginInvoke(new Action(() =>
                {
                    ctor.Value = param;
                    ctor.RQName = param.Split(';')[0];
                    (API as Connect)?.InputValueRqData(ctor);
                }));
            return ctor ?? null;
        }
        public ISendSecuritiesAPI InputValueRqData(bool input, string name)
        {
            var ctor = GetRequestTR(name);

            if (input)
                BeginInvoke(new Action(() => (API as Connect)?.InputValueRqData(ctor)));

            return ctor ?? null;
        }
        public void StartProgress() => buttonStartProgress.PerformClick();
        public void SetForeColor(Color color, string remain)
        {
            labelOpenAPI.ForeColor = color;
            labelMessage.Text = remain;
        }
        public ISendSecuritiesAPI OnConnectErrorMessage => API as Connect ?? null;
        public IEnumerable<Holding> HoldingStocks
        {
            get
            {
                foreach (var ctor in Connect.HoldingStock)
                    yield return ctor.Value ?? null;
            }
        }
        public dynamic API
        {
            get; private set;
        }
        public bool Start
        {
            get; private set;
        }
        public ConnectAPI(Privacies privacy)
        {
            this.privacy = privacy;
            InitializeComponent();

            if (string.IsNullOrEmpty(privacy.SecurityAPI) == false)
            {
                securites = new Security().Decipher(privacy.Security, privacy.SecuritiesAPI, privacy.SecurityAPI);
                checkAccount.CheckState = CheckState.Checked;
            }
        }
        readonly StringBuilder securites;
        readonly Privacies privacy;
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}