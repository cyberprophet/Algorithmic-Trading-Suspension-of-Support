using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Message;

using XA_SESSIONLib;

namespace ShareInvest.XingAPI
{
    class Connect : XASessionClass
    {
        internal static Connect GetInstance() => API;
        internal static Connect GetInstance(Privacy privacy, ShareInvest.Catalog.XingAPI.LoadServer load)
        {
            if (API == null)
                API = new Connect(privacy, load);

            return API;
        }
        internal (string, string, string) SetAccountName(string account, string password)
        {
            Secrecy.Account = account;
            Secrecy.Password = password;

            return (GetAccountName(account), GetAcctDetailName(account), GetAcctNickname(account));
        }
        internal string[] Accounts
        {
            get; private set;
        }
        internal bool OnReceiveBalance
        {
            get; set;
        }
        internal void Dispose()
        {
            DisconnectServer();
            API = null;
        }
        void OnEventConnect(string szCode, string szMsg)
        {
            if (secrecy.GetConnectionStatus(szCode, szMsg) && IsConnected())
            {
                Accounts = new string[GetAccountListCount()];

                for (int i = 0; i < Accounts.Length; i++)
                    Accounts[i] = GetAccountList(i);
            }
        }
        Connect(Privacy privacy, ShareInvest.Catalog.XingAPI.LoadServer load)
        {
            if (ConnectServer(load.Server, 0x4E21) && Login(privacy.Identity, privacy.Password, privacy.Certificate, 0, string.IsNullOrEmpty(privacy.Certificate) == false) && IsLoadAPI())
            {
                _IXASessionEvents_Event_Login += OnEventConnect;
                Disconnect += Dispose;
                secrecy = new Secrecy();

                while (TimerBox.Show(secrecy.Connection, load.Date, MessageBoxButtons.OK, MessageBoxIcon.Information, 3159).Equals(DialogResult.OK))
                    if (Accounts != null)
                        return;
            }
            else
                Dispose();
        }
        static Connect API
        {
            get; set;
        }
        readonly Secrecy secrecy;
    }
}