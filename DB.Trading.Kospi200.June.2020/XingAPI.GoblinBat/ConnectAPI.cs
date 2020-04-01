using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;
using ShareInvest.Message;
using ShareInvest.Verify;
using ShareInvest.XingAPI.Catalog;
using XA_SESSIONLib;

namespace ShareInvest.XingAPI
{
    public class ConnectAPI : XASessionClass, IEvents<NotifyIconText>
    {
        public static ConnectAPI GetInstance(string code, string date)
        {
            if (XingAPI == null)
            {
                Date = date;
                XingAPI = new ConnectAPI();
                Code = code;
                new T9943().QueryExcute();
            }
            return XingAPI;
        }
        public static ConnectAPI GetInstance()
        {
            return XingAPI;
        }
        public static string Code
        {
            get; private set;
        }
        public Dictionary<string, double> BuyOrder
        {
            get; private set;
        }
        public Dictionary<string, double> SellOrder
        {
            get; private set;
        }
        public Dictionary<string, string> Trend
        {
            get; set;
        }
        public Dictionary<string, string> CodeList
        {
            get; internal set;
        }
        public int Volume
        {
            get; set;
        }
        public int Quantity
        {
            get; internal set;
        }
        public bool OnReceiveBalance
        {
            get; set;
        }
        public double WindingUp
        {
            get; set;
        }
        public double Difference
        {
            get; set;
        }
        public double MaxAmount
        {
            get; private set;
        }
        public string[] Accounts
        {
            get; private set;
        }
        public string AvgPurchase
        {
            get; internal set;
        }
        public string WindingClass
        {
            get; set;
        }
        public string Classification
        {
            get; set;
        }
        public string DetailName
        {
            get
            {
                return GetAcctDetailName(Accounts.Length == 1 ? Accounts[0] : Array.Find(Accounts, o => o.Substring(o.Length - 2, 2).Equals("02")));
            }
        }
        public void Dispose()
        {
            Process.Start("shutdown.exe", "-r");
            Send?.Invoke(this, new NotifyIconText((char)69));
        }
        public void Dispose(bool reset)
        {
            if (reset)
                XingAPI = null;
        }
        public void Max(double trend, ShareInvest.Catalog.XingAPI.Specify specify, string check)
        {
            Judge[specify.Time] = trend;
            double temp = 0;

            foreach (var kv in Judge)
                temp += kv.Value;

            Classification = temp == 0 ? string.Empty : temp > 0 ? "2" : "1";
            Trend[specify.Time.ToString()] = string.Concat(trend.ToString("F2"), " (", specify.Time == 1440 ? "Base" : check, ")");
        }
        public double Max(double max)
        {
            int num = 5;

            foreach (var kv in Judge)
            {
                if (Classification.Equals("1") && kv.Value > 0)
                    num--;

                else if (Classification.Equals("2") && kv.Value < 0)
                    num--;
            }
            MaxAmount = max * num * 0.2 * (Classification.Equals("2") ? 1 : -1);

            return max * num * 0.2;
        }
        public FormWindowState SendNotifyIconText(int number)
        {
            Send?.Invoke(this, new NotifyIconText((int)TimerBox.Show(secret.Connection, Date, MessageBoxButtons.OK, MessageBoxIcon.Information, (uint)number)));

            return FormWindowState.Minimized;
        }
        public readonly IReals[] reals = (DateTime.Now.Hour == 15 && DateTime.Now.Minute < 45 || DateTime.Now.Hour < 15) && DateTime.Now.Hour > 4 ? new IReals[]
        {
            new FH0(),
            new FC0(),
            new JIF(),
            new O01(),
            new C01(),
            new H01()
        } : new IReals[]
        {
            new NH0(),
            new NC0(),
            new JIF(),
            new CM0(),
            new CM1(),
            new CM2()
        };
        public readonly IQuerys[] querys = (DateTime.Now.Hour == 15 && DateTime.Now.Minute < 45 || DateTime.Now.Hour < 15) && DateTime.Now.Hour > 4 ? new IQuerys[]
        {
            new CFOBQ10500(),
            new T0441()
        } : new IQuerys[]
        {
            new CCEBQ10500(),
            new CCEAQ50600()
        };
        public readonly IOrders[] orders = (DateTime.Now.Hour == 15 && DateTime.Now.Minute < 45 || DateTime.Now.Hour < 15) && DateTime.Now.Hour > 4 ? new IOrders[]
        {
            new CFOAT00100(),
            new CFOAT00200(),
            new CFOAT00300()
        } : new IOrders[]
        {
            new CCEAT00100(),
            new CCEAT00200(),
            new CCEAT00300()
        };
        private void OnEventConnect(string szCode, string szMsg)
        {
            if (secret.Code.Equals(szCode) && IsConnected() && secret.Success.Equals(szMsg))
            {
                Accounts = new string[GetAccountListCount()];

                for (int i = 0; i < Accounts.Length; i++)
                    Accounts[i] = GetAccountList(i);

                secret.GetAccount(Accounts);
            }
        }
        private Dictionary<uint, double> Judge
        {
            get;
        }
        private ConnectAPI()
        {
            secret = new Secret();
            var str = KeyDecoder.GetWindowsProductKeyFromRegistry();

            if (str.Length > 0 && secret.InfoToConnect.TryGetValue(str, out string[] connect) && secret.Server.TryGetValue(str, out string server) && ConnectServer(server, secret.Port) && Login(connect[0], connect[1], connect[2], 0, true) && IsLoadAPI())
            {
                _IXASessionEvents_Event_Login += OnEventConnect;
                Disconnect += Dispose;

                while (Accounts == null)
                    TimerBox.Show(secret.Connection, Date, MessageBoxButtons.OK, MessageBoxIcon.Information, 3159);
            }
            else if (MessageBox.Show(secret.Identity, secret.GoblinBat, MessageBoxButtons.OKCancel, MessageBoxIcon.Information).Equals(DialogResult.OK))
            {
                new ExceptionMessage(str);
                DisconnectServer();
                Dispose();
            }
            Judge = new Dictionary<uint, double>();
            Trend = new Dictionary<string, string>();
            SellOrder = new Dictionary<string, double>();
            BuyOrder = new Dictionary<string, double>();
        }
        private static ConnectAPI XingAPI
        {
            get; set;
        }
        private static string Date
        {
            get; set;
        }
        private readonly Secret secret;
        public event EventHandler<NotifyIconText> Send;
    }
}