using System.Windows.Forms;

using ShareInvest.Catalog.XingAPI;
using ShareInvest.Message;

using XA_SESSIONLib;

namespace ShareInvest.XingAPI
{
    class Connect : XASessionClass
    {
        internal (string, string, string) SetAccountName(string account) => (GetAccountName(account), GetAcctDetailName(account), GetAcctNickname(account));
        internal static Connect GetInstance(Privacy privacy, LoadServer load)
        {
            if (API == null)
            {
                API = new Connect(privacy, load);
            }
            return API;
        }
        internal string[] Accounts
        {
            get; private set;
        }
        void Dispose()
        {

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
        Connect(Privacy privacy, LoadServer load)
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
            {
                DisconnectServer();
                Dispose();
            }
        }
        static Connect API
        {
            get; set;
        }
        readonly Secrecy secrecy;
    }
}