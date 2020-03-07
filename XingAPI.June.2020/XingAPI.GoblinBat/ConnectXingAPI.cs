using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Message;
using ShareInvest.Struct;
using XA_DATASETLib;

namespace ShareInvest.XingAPI
{
    public class ConnectXingAPI : Catalog
    {
        public int Volume
        {
            get; set;
        }
        public int Quantity
        {
            get; private set;
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
        public string WindingClass
        {
            get; set;
        }
        public string Classification
        {
            get; set;
        }
        public Queue<string> Total
        {
            get; set;
        }
        public Dictionary<string, string> Trend
        {
            get; set;
        }
        public Dictionary<string, double> BuyOrder
        {
            get; private set;
        }
        public Dictionary<string, double> SellOrder
        {
            get; private set;
        }
        public void LookUpTheDeposit()
        {
            if (Query.LoadFromResFile(string.Concat(Path, "CFOBQ10500.res")))
            {
                foreach (var block in new CFOBQ10500().GetInBlock(Query.GetResData()))
                    Query.SetFieldData(block.Name, block.Field, block.Occurs, block.Property);

                Delay.delay = 1000 / Query.GetTRCountPerSec("CFOBQ10500");
                request.RequestTrData(new Task(() => SendErrorMessage(Query.Request(false))));
            }
        }
        public void LookUpTheBalance()
        {
            if (Query.LoadFromResFile(string.Concat(Path, "t0441.res")))
            {
                foreach (var block in new T0441().GetInBlock(Query.GetResData()))
                    Query.SetFieldData(block.Name, block.Field, block.Occurs, block.Property);

                Delay.delay = 1000 / Query.GetTRCountPerSec("t0441");
                request.RequestTrData(new Task(() => SendErrorMessage(Query.Request(false))));
            }
        }
        public void StartProgress(string path, string[] accounts)
        {
            if (Query != null && Real != null && Query.LoadFromResFile(string.Concat(path, "t9943.res")))
            {
                foreach (var block in new T9943().GetInBlock(Query.GetResData()))
                    Query.SetFieldData(block.Name, block.Field, block.Occurs, block.Property);

                Path = path;
                Delay.delay = 1000 / Query.GetTRCountPerSec("t9943");
                request.RequestTrData(new Task(() => SendErrorMessage(Query.Request(false))));
                new Secret(accounts[0]);

                return;
            }
            Process.Start("shutdown.exe", "-r");
            Dispose();
        }
        public void SetAPI(XAQueryClass query)
        {
            Query = query;
            query.ReceiveData += OnReceiveData;
            query.ReceiveMessage += OnReceiveMessage;
        }
        public void SetAPI(XARealClass real)
        {
            Real = real;
            real.ReceiveRealData += OnReceiveRealData;
        }
        public static ConnectXingAPI GetInstance()
        {
            if (api == null)
                api = new ConnectXingAPI();

            return api;
        }
        private void OnReceiveRealData(string szTrCode)
        {

        }
        private void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {

        }
        private void OnReceiveData(string szTrCode)
        {
            Sb = new StringBuilder(128);

            foreach (var block in Array.Find(catalog, o => o.ToString().Contains(szTrCode.Substring(1))).GetOutBlock(Query.GetResData()))
                for (int i = 0; i < Query.GetBlockCount(block.Name); i++)
                    Sb.Append(Query.GetFieldData(block.Name, block.Field, i)).Append(';');

            switch (szTrCode)
            {
                case "t9943":
                    foreach (var str in Sb.ToString().Split(';'))
                        if (str.Substring(0, 3).Equals("101"))
                        {
                            new Secret(new StringBuilder(str));
                            SendCount?.Invoke(this, new NotifyIconText(Query.GetAccountName(Secret.Account), Query.GetAcctDetailName(Secret.Account), Query.GetAcctNickname(Secret.Account), str));
                            SellOrder = new Dictionary<string, double>();
                            BuyOrder = new Dictionary<string, double>();
                            Trend = new Dictionary<string, string>();
                            Total = new Queue<string>();

                            if (Query.LoadFromResFile(string.Concat(Path, "t2105.res")))
                            {
                                foreach (var block in new T2105().GetInBlock(Query.GetResData()))
                                    Query.SetFieldData(block.Name, block.Field, block.Occurs, block.Property);

                                Delay.delay = 1000 / Query.GetTRCountPerSec("t2105");
                                request.RequestTrData(new Task(() => SendErrorMessage(Query.Request(false))));
                            }
                            if (TimerBox.Show(Secret.OnReceiveData, Secret.GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Information, 13591).Equals(DialogResult.OK))
                            {
                                LookUpTheBalance();
                                SendCount?.Invoke(this, new NotifyIconText(7));
                            }
                            return;
                        }
                    return;

                case "t2105":
                    LookUpTheDeposit();
                    var quotes = Sb.ToString().Split(';');
                    Name = quotes[0];
                    SendQuotes?.Invoke(this, new Quotes(new string[]
                    {
                        quotes[32],
                        quotes[26],
                        quotes[20],
                        quotes[14],
                        quotes[8],
                        quotes[9],
                        quotes[15],
                        quotes[21],
                        quotes[27],
                        quotes[33]
                    }, new string[]
                    {
                        quotes[34],
                        quotes[28],
                        quotes[22],
                        quotes[16],
                        quotes[10],
                        quotes[11],
                        quotes[17],
                        quotes[23],
                        quotes[29],
                        quotes[35]
                    }, new string[]
                    {
                        quotes[36],
                        quotes[30],
                        quotes[24],
                        quotes[18],
                        quotes[12],
                        quotes[13],
                        quotes[19],
                        quotes[25],
                        quotes[31],
                        quotes[37]
                    }, quotes[42], SellOrder, BuyOrder, string.Empty));
                    return;

                case "CFOBQ10500":
                    var temp = Sb.ToString().Split(';');
                    SendDeposit?.Invoke(this, new Deposit(new string[]
                    {
                        temp[5],
                        temp[6],
                        temp[7],
                        temp[14],
                        temp[15],
                        string.Empty,
                        temp[16],
                        temp[17],
                        string.Empty,
                        temp[18],
                        temp[19],
                        string.Empty,
                        temp[23],
                        temp[20],
                        temp[21],
                        temp[11],
                        temp[12],
                        temp[13],
                        temp[8],
                        temp[9],
                        temp[24],
                        string.Empty,
                        temp[10],
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty
                    }));
                    return;

                case "t0441":
                    if (Query.IsNext)
                    {
                        Console.WriteLine(Secret.Account + "\t" + Sb);
                        var bal = Sb.ToString().Split(';');
                        SendBalance?.Invoke(this, new Balance(new string[]
                        {
                            bal[10],
                            Name,
                            bal[11],
                            bal[12],
                            bal[14],
                            bal[19],
                            bal[21],
                            bal[13],
                            bal[15],
                            bal[20]
                        }));
                    }
                    else
                    {
                        var cts = Sb.ToString().Split(';');

                        if (cts[1].Equals(string.Empty) && cts[2].Equals(string.Empty))
                            return;

                        new Secret(cts[1], cts[2]);

                        foreach (var block in new T0441().GetInBlock(Query.GetResData()))
                            Query.SetFieldData(block.Name, block.Field, block.Occurs, block.Property);

                        Delay.delay = 1000 / Query.GetTRCountPerSec("t0441");
                        request.RequestTrData(new Task(() => SendErrorMessage(Query.Request(true))));
                    }
                    return;
            }
        }
        private void SendErrorMessage(int error)
        {
            if (error < 0)
                new ExceptionMessage(Query.GetErrorMessage(error));
        }
        private void Dispose()
        {

        }
        private string Path
        {
            get; set;
        }
        private string Name
        {
            get; set;
        }
        private StringBuilder Sb
        {
            get; set;
        }
        private ConnectXingAPI()
        {
            request = Delay.GetInstance(501);
            request.Run();
        }
        private XARealClass Real
        {
            get; set;
        }
        private XAQueryClass Query
        {
            get; set;
        }
        private static ConnectXingAPI api;
        private readonly Delay request;
        public event EventHandler<Datum> SendDatum;
        public event EventHandler<Memorize> SendMemorize;
        public event EventHandler<NotifyIconText> SendCount;
        public event EventHandler<Quotes> SendQuotes;
        public event EventHandler<Deposit> SendDeposit;
        public event EventHandler<Balance> SendBalance;
        public event EventHandler<Current> SendCurrent;
        public event EventHandler<State> SendState;
        public event EventHandler<Trends> SendTrend;
    }
}