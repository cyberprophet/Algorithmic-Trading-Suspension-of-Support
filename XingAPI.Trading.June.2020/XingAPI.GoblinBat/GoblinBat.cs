using System;
using System.Windows.Forms;
using XA_SESSIONLib;
using XA_DATASETLib;
using System.IO;

namespace ShareInvest.XingAPI
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
            api = new XASessionClass();
            query = new XAQueryClass();
            real = new XARealClass();
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
                Account = api.GetAccountList(i);

            query.ReceiveData += QueryReceiveData;
            query.ReceiveMessage += QueryReceiveMessage;
            query.SetFieldData(GetBlockName(query.ResFileName = Path.Combine(path, "t1101.res")), "shcode", 0, "005930");
            Console.WriteLine(query.Request(false));
        }
        private void QueryReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            Console.WriteLine(bIsSystemError + "\t" + nMessageCode + "\t" + szMessage);
        }
        private void QueryReceiveData(string szTrCode)
        {
            string[] arr = query.GetResData().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 5; i < arr.Length - 1; i++)
            {
                string[] temp = arr[i].Split(',');
                Console.WriteLine(query.GetFieldData("t1101OutBlock", temp[2], 0));
            }
        }
        private string GetBlockName(string res)
        {
            foreach (string str in query.GetResData().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                if (str.Contains("레코드명:") && str.Contains("InBlock"))
                    return str.Replace("*", string.Empty).Replace("레코드명:", string.Empty).Trim();

            return res;
        }
        private string Account
        {
            get; set;
        }
        private const string path = @"C:\eBEST\xingAPI\Res";
        private readonly XASessionClass api;
        private readonly XAQueryClass query;
        private readonly XARealClass real;
    }
}