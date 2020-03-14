using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;
using ShareInvest.Message;
using ShareInvest.XingAPI.Catalog;
using XA_SESSIONLib;

namespace ShareInvest.XingAPI
{
    public class ConnectAPI : XASessionClass, IEvents<NotifyIconText>
    {
        public static ConnectAPI GetInstance(string code)
        {
            if (XingAPI == null)
                XingAPI = new ConnectAPI();

            if (code.Equals(string.Empty) == false)
            {
                Code = code;
                new T9943().QueryExcute();
            }
            return XingAPI;
        }
        public static string Code
        {
            get; private set;
        }
        public string[] Accounts
        {
            get; private set;
        }
        public readonly IReals[] real = DateTime.Now.Hour < 17 && DateTime.Now.Hour > 5 ? new IReals[]
        {
            new FH0(),
            new FC0(),
            new JIF()
        } : new IReals[]
        {
            new NH0(),
            new NC0(),
            new JIF()
        };
        public readonly IQuerys[] query = DateTime.Now.Hour < 17 && DateTime.Now.Hour > 5 ? new IQuerys[]
        {
            new CFOBQ10500(),
            new T0441(),
        } : new IQuerys[]
        {
            new CCEBQ10500(),
            new CCEAQ50600()
        };
        public void Dispose()
        {
            Process.Start("shutdown.exe", "-r");
            Send?.Invoke(this, new NotifyIconText((char)69));
        }
        public FormWindowState SendNotifyIconText(int number)
        {
            Send?.Invoke(this, new NotifyIconText((int)TimerBox.Show(secret.Connection, secret.GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Information, (uint)number)));

            return FormWindowState.Minimized;
        }
        public Dictionary<string, string> CodeList
        {
            get; internal set;
        }
        private void OnEventConnect(string szCode, string szMsg)
        {
            if (secret.Code.Equals(szCode) && IsConnected() && secret.Success.Equals(szMsg))
            {
                Accounts = new string[GetAccountListCount()];

                for (int i = 0; i < Accounts.Length; i++)
                    Accounts[i] = GetAccountList(i);
            }
        }
        private ConnectAPI()
        {
            secret = new Secret();            

            if (ConnectServer(secret.Server[1], secret.Port) && Login(secret.InfoToConnect[0], secret.InfoToConnect[1], secret.InfoToConnect[2], 0, true) && IsLoadAPI())
            {
                _IXASessionEvents_Event_Login += OnEventConnect;
                Disconnect += Dispose;
            }
            while (Accounts == null)
                TimerBox.Show(secret.Connection, secret.GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Information, 3159);
        }
        private static ConnectAPI XingAPI
        {
            get; set;
        }
        private readonly Secret secret;
        public event EventHandler<NotifyIconText> Send;
    }
}