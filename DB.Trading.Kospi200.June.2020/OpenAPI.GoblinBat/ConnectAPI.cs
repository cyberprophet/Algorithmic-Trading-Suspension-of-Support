using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxKHOpenAPILib;
using ShareInvest.Catalog;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Interface.Struct;
using ShareInvest.Message;

namespace ShareInvest.OpenAPI
{
    public class ConnectAPI : AuxiliaryFunction
    {
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
        public string Strategy
        {
            get
            {
                return GetStrategy();
            }
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
        public void OnReceiveOrder(PurchaseInformation o)
        {
            request.RequestTrData(new Task(() =>
            {
                ErrorCode = API.SendOrderFO(o.RQName, o.ScreenNo, o.AccNo, o.Code, o.OrdKind, o.SlbyTP, o.OrdTp, o.Qty, o.Price, o.OrgOrdNo);

                if (ErrorCode < 0)
                    new ExceptionMessage(error.GetErrorMessage(ErrorCode));
            }));
        }
        public void SetAPI(AxKHOpenAPI axAPI)
        {
            API = axAPI;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveTrData += OnReceiveTrData;
            axAPI.OnReceiveRealData += OnReceiveRealData;
            axAPI.OnReceiveMsg += OnReceiveMsg;
            axAPI.OnReceiveChejanData += OnReceiveChejanData;
        }
        public void LookUpTheDeposit(string[] account)
        {
            request.RequestTrData(new Task(() =>
            {
                InputValueRqData(new OPW20010
                {
                    Value = string.Concat(Array.Find(account, o => o.Substring(8, 2).Equals("31")), ";;00"),
                    PrevNext = 0
                });
            }));
        }
        public void LookUpTheBalance(string[] account)
        {
            request.RequestTrData(new Task(() =>
            {
                InputValueRqData(new OPW20007
                {
                    Value = string.Concat(Array.Find(account, o => o.Substring(8, 2).Equals("31")), ";;00"),
                    PrevNext = 0
                });
            }));
        }
        public void StartProgress(string transfer)
        {
            if (transfer != null)
            {
                new Temporary();

                foreach (string temp in new Transfer(transfer))
                {
                    if (!temp.Contains(","))
                    {
                        string code = temp.Equals("Tick") || temp.Equals("Day") ? "101Q3000" : temp;
                        SendMemorize?.Invoke(this, new Memorize(temp.Equals("Day") ? "day" : temp, code));

                        continue;
                    }
                    SendMemorize?.Invoke(this, new Memorize(temp.Split(',')));
                }
                return;
            }
        }
        public void StartProgress()
        {
            if (API != null)
            {
                ErrorCode = API.CommConnect();

                if (ErrorCode < 0)
                    new ExceptionMessage(error.GetErrorMessage(ErrorCode));

                return;
            }
            Process.Start("shutdown.exe", "-r");
            Dispose();
        }
        public void StartProgress(int delay)
        {
            if (Delay.delay == 3605)
                return;

            DeadLine = true;
            Delay.delay = delay;
            Code = RequestCodeList(new List<string>(32));
            SendMemorize?.Invoke(this, new Memorize("Clear"));
            Request(GetRandomCode(new Random().Next(0, Code.Count)));
        }
        public static ConnectAPI GetInstance()
        {
            if (api == null)
                api = new ConnectAPI();

            return api;
        }
        private void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            if (Array.Exists(message.basic, o => o.Equals(e.sMsg.Substring(9))))
            {
                var temp = e.sMsg.Substring(9);

                if (temp.Equals(message.basic[2]) && OnReceiveBalance == false)
                    OnReceiveBalance = request.QueueCount == 0 ? true : false;

                if (e.sMsg.Contains("모의투자"))
                    temp = temp.Replace("모의투자 ", string.Empty);

                if (e.sMsg.Last().Equals('다') || e.sMsg.Last().Equals('요'))
                    temp = string.Concat(temp, ".");

                new Task(() =>
                {
                    SendState?.Invoke(this, new State(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count));
                    SendCount?.Invoke(this, new NotifyIconText(temp));
                }).Start();
                return;
            }
            if (e.sMsg.Equals(message.TR))
            {
                SendMemorize?.Invoke(this, new Memorize("Clear"));
                Request(GetRandomCode(new Random().Next(0, Code.Count)));

                return;
            }
            if (e.sMsg.Equals(message.Failure) || e.sMsg.Substring(9).Equals(message.Restart))
            {
                Process.Start("shutdown.exe", "-r");
                Dispose();

                return;
            }
            if (e.sMsg.Substring(9).Equals(message.LookUp))
                return;

            new ExceptionMessage(e.sMsg);
        }
        private void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            Sb = new StringBuilder(256);
            var index = int.Parse(e.sGubun);

            foreach (int fid in real.type[index])
                Sb.Append(API.GetChejanData(fid)).Append(';');

            var param = Sb.ToString().Split(';');

            switch (index)
            {
                case 0:
                    if (param[3].Equals(API.GetFutureCodeByIndex(0)))
                    {
                        switch (param[5])
                        {
                            case "체결":
                                if (param[14].Equals("1") ? SellOrder.Remove(param[1]) : BuyOrder.Remove(param[1]))
                                    OnReceiveBalance = request.QueueCount == 0 ? true : false;

                                break;

                            case "접수":
                                if (int.Parse(param[11]) == 0)
                                    OnReceiveBalance = request.QueueCount == 0 ? true : false;

                                break;

                            case "확인":
                                if (param[12].Substring(3).Equals("취소") || param[12].Substring(3).Equals("정정"))
                                    OnReceiveBalance = request.QueueCount == 0 ? true : false;

                                break;
                        }
                        new Task(() => SendState?.Invoke(this, new State(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count))).Start();
                    }
                    return;

                case 1:
                    return;

                case 4:
                    if (param[1].Equals(API.GetFutureCodeByIndex(0)))
                    {
                        Quantity = param[9].Equals("1") ? -int.Parse(param[4]) : int.Parse(param[4]);
                        new Task(() => SendState?.Invoke(this, new State(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count))).Start();
                    }
                    return;
            };
        }
        private void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            Sb = new StringBuilder(512);
            int index = Array.FindIndex(Enum.GetNames(typeof(RealType.EnumType)), o => o.Equals(e.sRealType));

            if (index < 0)
                return;

            foreach (int fid in real.type[index])
                Sb.Append(API.GetCommRealData(e.sRealKey, fid)).Append(';');

            var param = Sb.ToString().Split(';');

            switch (index)
            {
                case 1:
                    if (e.sRealKey.Equals(API.GetFutureCodeByIndex(0)))
                    {
                        SendDatum?.Invoke(this, new Datum(param));
                        SendTrend?.Invoke(this, new Trends(Trend));
                    }
                    return;

                case 2:
                    if (e.sRealKey.Equals(API.GetFutureCodeByIndex(0)))
                    {
                        Total.Enqueue(string.Concat(param[44], ";", param[47]));
                        SendQuotes?.Invoke(this, new Quotes(new string[]
                        {
                            param[35],
                            param[27],
                            param[19],
                            param[11],
                            param[3],
                            param[7],
                            param[15],
                            param[23],
                            param[31],
                            param[39]
                        }, new string[]
                        {
                            param[36],
                            param[28],
                            param[20],
                            param[12],
                            param[4],
                            param[8],
                            param[16],
                            param[24],
                            param[32],
                            param[40]
                        }, new string[]
                        {
                            param[38],
                            param[30],
                            param[22],
                            param[14],
                            param[6],
                            param[10],
                            param[18],
                            param[26],
                            param[34],
                            param[42]
                        }, param[0], SellOrder, BuyOrder));
                    }
                    return;

                case 8:
                    if (e.sRealKey.Equals(API.GetFutureCodeByIndex(0)))
                        SendCurrent?.Invoke(this, new Current(Quantity, Sb.ToString().Split(';')));

                    return;

                case 9:
                    if (param[0].Equals("e") && DeadLine)
                    {
                        DeadLine = false;
                        OnReceiveBalance = false;
                        Code = RequestCodeList(new List<string>(32), Markets);
                        SendMemorize?.Invoke(this, new Memorize("Clear"));
                        Delay.delay = 4215;
                        Request(GetRandomCode(new Random().Next(0, Code.Count)));
                    }
                    else if (param[0].Equals("3") && DeadLine == false)
                    {
                        DeadLine = true;
                        OnReceiveBalance = true;
                        Delay.delay = 205;
                    }
                    else if (param[0].Equals("0") && param[2].Equals("002000"))
                    {
                        Process.Start("shutdown.exe", "-r");
                        Dispose();
                    }
                    break;
            };
        }
        private void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            int index = Array.FindIndex(catalog, o => o.ToString().Contains(e.sTrCode.Substring(1)));

            if (index < 1)
            {
                new ExceptionMessage(e.sTrCode);

                return;
            }
            if (index < 5 && index > 0)
            {
                var temp = API.GetCommDataEx(e.sTrCode, e.sRQName);

                if (temp != null)
                {
                    string[,] ts = new string[((object[,])temp).GetUpperBound(0) + 1, ((object[,])temp).GetUpperBound(1) + 1];
                    int x, y, lx = ((object[,])temp).GetUpperBound(0), ly = ((object[,])temp).GetUpperBound(1);

                    for (x = 0; x <= lx; x++)
                    {
                        Sb = new StringBuilder(64);

                        for (y = 0; y <= ly; y++)
                        {
                            ts[x, y] = (string)((object[,])temp)[x, y];

                            if (ts[x, y].Length > 13 && e.sRQName.Split(';')[1].Equals(ts[x, y].Substring(2)))
                            {
                                Sb = message.Exists;
                                e.sPrevNext = "0";

                                break;
                            }
                            Sb.Append(ts[x, y]).Append(';');
                        }
                        if (!message.Exists.Equals(Sb))
                        {
                            SendMemorize?.Invoke(this, new Memorize(Sb));

                            continue;
                        }
                        if (message.Exists.Equals(Sb))
                            break;
                    }
                    if (DeadLine && (e.sRQName.Split(';')[1].Length == 8 || e.sRQName.Split(';')[1].Equals("DoesNotExist")) && e.sPrevNext.Equals("2"))
                    {
                        request.RequestTrData(new Task(() => InputValueRqData(new Opt10081
                        {
                            Value = e.sRQName.Split(';')[0],
                            RQName = e.sRQName,
                            PrevNext = 2
                        })));
                        return;
                    }
                    if (DeadLine == false && e.sRQName.Split(';')[0].Length == 6 && e.sPrevNext.Equals("2"))
                    {
                        request.RequestTrData(new Task(() => InputValueRqData(new Opt10079
                        {
                            Value = e.sRQName.Split(';')[0],
                            RQName = e.sRQName,
                            PrevNext = 2
                        })));
                        return;
                    }
                    if (DeadLine == false && e.sRQName.Substring(5, 3).Equals("000") && e.sPrevNext.Equals("2"))
                    {
                        request.RequestTrData(new Task(() => InputValueRqData(new Opt50028
                        {
                            Value = e.sRQName.Substring(0, 8),
                            RQName = e.sRQName,
                            PrevNext = 2
                        })));
                        return;
                    }
                    if (DeadLine == false && e.sRQName.Split(';')[0].Length == 8 && e.sPrevNext.Equals("2"))
                    {
                        request.RequestTrData(new Task(() => InputValueRqData(new Opt50066
                        {
                            Value = e.sRQName.Substring(0, 8),
                            RQName = e.sRQName,
                            PrevNext = 2
                        })));
                        return;
                    }
                    if (e.sPrevNext.Equals("0"))
                        SendMemorize?.Invoke(this, new Memorize(e.sPrevNext, e.sRQName.Split(';')[0]));
                }
                SendMemorize?.Invoke(this, new Memorize("Clear"));
                Request(GetRandomCode(new Random().Next(0, Code.Count)));

                return;
            }
            Sb = new StringBuilder(512);
            int i, cnt = API.GetRepeatCnt(e.sTrCode, e.sRQName);

            for (i = 0; i < (cnt > 0 ? cnt : cnt + 1); i++)
            {
                foreach (string item in Array.Find(catalog, o => o.ToString().Contains(e.sTrCode.Substring(1))))
                    Sb.Append(API.GetCommData(e.sTrCode, e.sRQName, i, item).Trim()).Append(';');

                if (cnt > 0)
                    Sb.Append("*");
            }
            switch (Array.FindIndex(catalog, o => o.ToString().Contains(e.sTrCode.Substring(1))))
            {
                case 5:
                    FixUp(Sb.ToString().Split(';'), e.sRQName);
                    return;

                case 6:
                    foreach (string info in Sb.ToString().Split('*'))
                        FixUp(info.Split(';'));

                    return;

                case 7:
                case 8:
                    if (Sb.Length > 1 && e.sRQName.Equals("DoNotRollOver") == false)
                    {
                        if (e.sScrNo.Substring(0, 1).Equals("1"))
                            SellOrder[Sb.ToString().Split(';')[0]] = double.Parse(e.sRQName);

                        else if (e.sScrNo.Substring(0, 1).Equals("2"))
                            BuyOrder[Sb.ToString().Split(';')[0]] = double.Parse(e.sRQName);
                    }
                    new Task(() => SendState?.Invoke(this, new State(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count))).Start();
                    return;

                case 9:
                    new Task(() => SendState?.Invoke(this, new State(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count))).Start();
                    return;

                case 10:
                    new Task(() => SendDeposit?.Invoke(this, new Deposit(Sb.ToString().Split(';')))).Start();
                    break;

                case 11:
                    var temporary = Sb.ToString().Split('*');
                    new Task(() => SendBalance?.Invoke(this, new Balance(temporary))).Start();

                    for (i = 0; i < temporary.Length; i++)
                        if (temporary[i].Length > 0 && temporary[i].Substring(0, 8).Equals(API.GetFutureCodeByIndex(0)))
                        {
                            var quantity = temporary[i].Split(';');
                            Quantity = quantity[2].Equals("1") ? -int.Parse(quantity[3]) : int.Parse(quantity[3]);
                        }
                    break;
            }
        }
        private void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            try
            {
                Code = new List<string>
                {
                    API.GetFutureCodeByIndex(e.nErrCode)
                };
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
                Process.Start("shutdown.exe", "-r");
                Dispose();
            }
            bool onlyOnce = true, check = DateTime.Now.Hour > 7 && DateTime.Now.Hour < 16 && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday)) == false;
            var account = API.GetLoginInfo("ACCLIST");
            Markets = GetInformation();

            do
            {
                if (onlyOnce)
                {
                    onlyOnce = false;
                    SendCount?.Invoke(this, new NotifyIconText(account, API.GetLoginInfo("USER_ID"), API.GetLoginInfo("USER_NAME"), API.GetLoginInfo("GetServerGubun")));
                    SellOrder = new Dictionary<string, double>();
                    BuyOrder = new Dictionary<string, double>();
                    Trend = new Dictionary<string, string>();
                    Total = new Queue<string>();
                    LookUpTheDeposit(account.Split(';'));
                    LookUpTheBalance(account.Split(';'));
                }
                if (TimerBox.Show(message.OnReceiveData, message.GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Information, (uint)(request.QueueCount + Markets.Length)).Equals(DialogResult.OK))
                    if (TimerBox.Show(message.SetPassword, message.GoblinBat, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, (uint)(request.QueueCount + Markets.Length)).Equals(DialogResult.OK))
                        API.KOA_Functions("ShowAccountWindow", "");
            }
            while (request.QueueCount > 0);

            if (check)
            {
                DeadLine = DateTime.Now.Hour < 9 ? false : true;
                Delay.delay = 205;
                OnReceiveBalance = DateTime.Now.Hour > 8 ? true : false;
            }
            else
            {
                Code = RequestCodeList(new List<string>(32), Markets);
                Request(GetRandomCode(new Random(e.nErrCode).Next(0, Code.Count)));
                Delay.delay = 4135;
            }
            SendCount?.Invoke(this, new NotifyIconText(7));
        }
        private string[] GetInformation()
        {
            int i, l;
            string exclusion, date = GetDistinctDate(CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);

            for (i = 2; i < 4; i++)
                foreach (var om in API.GetActPriceList().Split(';'))
                {
                    exclusion = API.GetOptionCode(om.Insert(3, "."), i, date);

                    if (Code.Exists(o => o.Equals(exclusion)))
                        continue;

                    Code.Add(exclusion);
                }
            Code[1] = API.GetFutureCodeByIndex(24);
            string[] temp, market = API.GetCodeListByMarket("").Split(';');
            l = market.Length;

            foreach (string output in Code)
                RemainingDay(output);

            foreach (string sMarket in new CodeListByMarket())
            {
                temp = API.GetCodeListByMarket(sMarket).Split(';');

                for (i = 0; i < l; i++)
                    if (Array.Exists(temp, o => o.Equals(market[i])))
                        market[i] = string.Empty;
            }
            for (i = 0; i < l; i++)
            {
                string tempCode = market[i];

                if (API.GetMasterStockState(tempCode).Contains("거래정지"))
                {
                    market[i] = string.Empty;

                    continue;
                }
                if (tempCode.Length > 0)
                {
                    foreach (string ex in new CodeListByExclude())
                        if (API.GetMasterCodeName(tempCode).EndsWith(ex) && !Array.Exists(message.exclude, o => o.Equals(tempCode)))
                            market[i] = string.Empty;

                    continue;
                }
            }
            foreach (string output in SetCodeStorage(market))
                RemainingDay(output);

            return market;
        }
        private string GetRandomCode(int index)
        {
            if (Code.Count < 1)
                return string.Empty;

            else if (Code[index] != null)
            {
                var temp = Code[index];
                Code.Remove(temp);

                return temp;
            }
            return GetRandomCode(new Random(index).Next(0, Code.Count));
        }
        private void Request(string code)
        {
            if (code != null && !code.Equals(string.Empty))
            {
                int param = DeadLine ? 4 : (code.Length > 6 ? (code.Substring(5, 3).Equals("000") ? 1 : 2) : 3);
                ITR tr = (ITR)catalog[param];
                tr.Value = code;
                tr.RQName = string.Concat(code, ";", GetRetention(param, code));
                tr.PrevNext = 0;
                request.RequestTrData(new Task(() =>
                {
                    InputValueRqData(tr);
                    SendCount?.Invoke(this, new NotifyIconText(Code.Count, code));
                }));
                return;
            }
            else if (Code.Count < 50)
            {
                foreach (var str in Code)
                    if (str == null || str.Equals(string.Empty))
                        Code.Remove(str);

                if (Code.Count < 1)
                {
                    Code = null;
                    GC.Collect();

                    return;
                }
            }
            Request(GetRandomCode(new Random().Next(0, Code.Count)));
        }
        private void FixUp(string[] param, string code)
        {
            if (code.Equals(API.GetFutureCodeByIndex(0)))
                SendQuotes?.Invoke(this, new Quotes(new string[]
                {
                    param[0],
                    param[1],
                    param[2],
                    param[3],
                    param[4],
                    param[5],
                    param[6],
                    param[7],
                    param[8],
                    param[9]
                }, new string[]
                {
                    param[15],
                    param[16],
                    param[17],
                    param[18],
                    param[19],
                    param[20],
                    param[21],
                    param[22],
                    param[23],
                    param[24]
                }, new string[]
                {
                    param[10],
                    param[11],
                    param[12],
                    param[13],
                    param[14],
                    param[25],
                    param[26],
                    param[27],
                    param[28],
                    param[29]
                }, param[32], SellOrder, BuyOrder));
            SetInsertCode(code, param[72], param[63]);
        }
        private void RemainingDay(string code)
        {
            if (code.Length < 9 && code.Length > 6)
            {
                request.RequestTrData(new Task(() => InputValueRqData(new Opt50001
                {
                    Value = code,
                    RQName = code,
                    PrevNext = 0
                })));
                return;
            }
            request.RequestTrData(new Task(() =>
            {
                ITR tr = new OPTKWFID
                {
                    Value = code,
                    PrevNext = 0
                };
                ErrorCode = API.CommKwRqData(tr.Value, 0, 100, tr.PrevNext, tr.RQName, tr.ScreenNo);

                if (ErrorCode < 0)
                    new ExceptionMessage(error.GetErrorMessage(ErrorCode));
            }));
        }
        private void InputValueRqData(ITR param)
        {
            string[] count = param.ID.Split(';'), value = param.Value.Split(';');
            int i, l = count.Length;

            for (i = 0; i < l; i++)
                API.SetInputValue(count[i], value[i]);

            ErrorCode = API.CommRqData(param.RQName, param.TrCode, param.PrevNext, param.ScreenNo);

            if (ErrorCode < 0)
            {
                new ExceptionMessage(error.GetErrorMessage(ErrorCode));

                if (ErrorCode == -200)
                {
                    Process.Start("shutdown.exe", "-r");
                    Dispose();
                }
                else if (ErrorCode == -300)
                {
                    SendMemorize?.Invoke(this, new Memorize("Clear"));
                    Request(GetRandomCode(new Random().Next(0, Code.Count)));
                }
            }
        }
        private void Dispose()
        {
            SendCount?.Invoke(this, new NotifyIconText('E'));
        }
        private int ErrorCode
        {
            get; set;
        }
        private bool DeadLine
        {
            get; set;
        }
        private string[] Markets
        {
            get; set;
        }
        private ConnectAPI()
        {
            error = new Error();
            message = new Message();
            real = new RealType();
            request = Delay.GetInstance(605);
            request.Run();
        }
        private List<string> Code
        {
            get; set;
        }
        private StringBuilder Sb
        {
            get; set;
        }
        private AxKHOpenAPI API
        {
            get; set;
        }
        private static ConnectAPI api;
        private readonly Error error;
        private readonly Message message;
        private readonly Delay request;
        private readonly RealType real;
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