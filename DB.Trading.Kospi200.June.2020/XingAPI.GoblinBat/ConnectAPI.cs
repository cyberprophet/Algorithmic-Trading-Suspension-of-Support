using System;
using ShareInvest.XingAPI.Catalog;
using XA_SESSIONLib;

namespace ShareInvest.XingAPI
{
    public class ConnectAPI : XASessionClass
    {
        public static ConnectAPI GetInstance()
        {
            if (XingAPI == null)
                XingAPI = new ConnectAPI();

            return XingAPI;
        }
        public string[] Accounts
        {
            get; private set;
        }
        private void OnEventConnect(string szCode, string szMsg)
        {
            if (secret.Code.Equals(szCode) && IsConnected() && secret.Success.Equals(szMsg))
            {
                Accounts = new string[GetAccountListCount()];

                for (int i = 0; i < Accounts.Length; i++)
                    Accounts[i] = GetAccountList(i);

                /*
                new CFOBQ10500().QueryExcute();
                new NC0().OnReceiveRealTime("101Q3000");
                new NH0().OnReceiveRealTime("101Q3000");
                */
            }
        }
        private void Dispose()
        {

        }
        private ConnectAPI()
        {
            secret = new Secret();

            if (ConnectServer(secret.Server[0], secret.Port) && Login(secret.InfoToConnect[0], secret.InfoToConnect[1], secret.InfoToConnect[2], 0, true))
            {
                _IXASessionEvents_Event_Login += OnEventConnect;
                Disconnect += Dispose;
            }
        }
        private static ConnectAPI XingAPI
        {
            get; set;
        }
        private readonly Secret secret;
    }
}