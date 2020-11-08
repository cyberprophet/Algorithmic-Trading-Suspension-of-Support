using System;
using System.Windows.Forms;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;
using ShareInvest.Interface;

namespace ShareInvest.OpenAPI
{
    public sealed partial class ConnectAPI : UserControl, ISecuritiesAPI<SendSecuritiesAPI>
    {
        void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0)
            {

                Send?.Invoke(this, new SendSecuritiesAPI(GetType().Name, axAPI.GetLoginInfo("ACCLIST").Split(';')));
            }
            else
                Send?.Invoke(this, new SendSecuritiesAPI((API as Connect)?.SendErrorMessage(e.nErrCode)));
        }
        void OnReceiveMessage(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e) => Send?.Invoke(this, new SendSecuritiesAPI(string.Concat("[", e.sRQName, "] ", e.sMsg.Substring(9), "(", e.sScrNo, ")")));
        public ConnectAPI()
        {
            InitializeComponent();

        }
        public dynamic API
        {
            get; private set;
        }
        public bool Start
        {
            get; private set;
        }
        public void StartProgress()
        {
            Start = true;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveMsg += OnReceiveMessage;
            API = Connect.GetInstance(axAPI);
        }
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}