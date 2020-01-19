using System;
using System.Windows.Forms;
using XA_SESSIONLib;

namespace ShareInvest.XingAPI
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
            api = new XASessionClass();
            SetLogin(new Secret(), api.ConnectServer(new Server().GetSelectServer("1"), 20001));
        }
        private void SetLogin(Secret secret, bool login)
        {
            if (login)
            {
                if (api.Login(secret.Id, secret.Password, secret.Cert, 0, true))
                    api._IXASessionEvents_Event_Login += ApiIXASessionEventsEventLogin;

                return;
            }
        }
        private void ApiIXASessionEventsEventLogin(string szCode, string szMsg)
        {
            if (szCode.Equals("0000"))
                GetAccount(api.GetAccountListCount());
        }
        private void GetAccount(int count)
        {
            for (int i = 0; i < count; i++)
                Console.WriteLine(api.GetAccountList(i));
        }
        private readonly XASessionClass api;
    }
}