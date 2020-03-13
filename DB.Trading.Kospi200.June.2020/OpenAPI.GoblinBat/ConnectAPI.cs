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
using ShareInvest.EventHandler.OpenAPI;
using ShareInvest.Message;

namespace ShareInvest.OpenAPI
{
    public class ConnectAPI : AuxiliaryFunction
    {
        public static ConnectAPI GetInstance()
        {
            if (OpenAPI == null)
                OpenAPI = new ConnectAPI();

            return OpenAPI;
        }
        public int Volume
        {
            get; set;
        }
        public int Quantity
        {
            get; private set;
        }
        public uint ScreenNumber
        {
            get; set;
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
        public string AvgPurchase
        {
            get; private set;
        }
        public string Code
        {
            get; private set;
        }
        public string WindingClass
        {
            get; set;
        }
        public string Classification
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
        public Temporary Temporary
        {
            get; private set;
        }
        public void SetScreenNumber(uint start, uint finish)
        {
            for (uint i = start; i < finish; i++)
                API.DisconnectRealData(i.ToString("D4"));
        }
        public void OnReceiveOrder(PurchaseInformation o)
        {
            request.RequestTrData(new Task(() => SendErrorMessage(API.SendOrderFO(o.RQName, o.ScreenNo, o.AccNo, o.Code, o.OrdKind, o.SlbyTP, o.OrdTp, o.Qty, o.Price, o.OrgOrdNo))));
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
                new Temporary(OpenAPI);

                foreach (string temp in new Transfer(transfer))
                {
                    if (!temp.Contains(","))
                    {
                        string code = temp.Equals("Tick") || temp.Equals("Day") ? "101Q3000" : temp;
                        SendMemorize?.Invoke(this, new OpenMemorize(temp.Equals("Day") ? "day" : temp, code));

                        continue;
                    }
                    SendMemorize?.Invoke(this, new OpenMemorize(temp.Split(',')));
                }
                return;
            }
        }
        public void StartProgress(Temporary temporary)
        {
            if (API != null)
            {
                SendErrorMessage(API.CommConnect());
                Temporary = temporary;
                API.OnReceiveChejanData -= OnReceiveChejanData;

                return;
            }
            Process.Start("shutdown.exe", "-r");
            Dispose();
        }
        public void StartProgress()
        {
            if (API != null)
            {
                SendErrorMessage(API.CommConnect());

                return;
            }
            Process.Start("shutdown.exe", "-r");
            Dispose();
        }
        public void StartProgress(int delay)
        {
            if (Delay.Milliseconds == 3605)
                return;

            DeadLine = true;
            Delay.Milliseconds = delay;
            CodeList = RequestCodeList(new List<string>(32));
            SendMemorize?.Invoke(this, new OpenMemorize("Clear"));
            Request(GetRandomCode(new Random().Next(0, CodeList.Count)));
        }
        private void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            if (Array.Exists(basic, o => o.Equals(e.sMsg.Substring(9))))
            {
                var temp = e.sMsg.Substring(9);

                if ((temp.Equals(basic[2]) || temp.Equals(basic[6]) || temp.Equals(basic[8])) && OnReceiveBalance == false)
                    OnReceiveBalance = request.QueueCount == 0 ? true : false;

                if (e.sMsg.Contains("모의투자"))
                    temp = temp.Replace("모의투자 ", string.Empty);

                if (e.sMsg.Last().Equals('다') || e.sMsg.Last().Equals('요'))
                    temp = string.Concat(temp, ".");

                SendState?.Invoke(this, new OpenState(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count, ScreenNumber));
                SendCount?.Invoke(this, new NotifyIconText(temp));

                return;
            }
            if (e.sMsg.Equals(TR))
            {
                SendMemorize?.Invoke(this, new OpenMemorize("Clear"));
                Request(GetRandomCode(new Random().Next(0, CodeList.Count)));

                return;
            }
            if (e.sMsg.Equals(Failure))
            {
                Process.Start("shutdown.exe", "-r");
                Dispose();

                return;
            }
            if (e.sMsg.Contains(Response) && TimerBox.Show(string.Concat(Response, "."), GoblinBat, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, 1375).Equals(DialogResult.OK))
                return;

            if (e.sMsg.Substring(9).Equals(LookUp))
                return;

            new ExceptionMessage(e.sMsg);
        }
        private void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            var sb = new StringBuilder(256);
            var index = int.Parse(e.sGubun);

            foreach (int fid in catalogReal[index])
                sb.Append(API.GetChejanData(fid)).Append(';');

            var param = sb.ToString().Split(';');

            switch (index)
            {
                case 0:
                    if (param[3].Equals(Code))
                    {
                        switch (param[5])
                        {
                            case "체결":
                                if (param[14].Equals("1") ? SellOrder.Remove(param[1]) : BuyOrder.Remove(param[1]))
                                    OnReceiveBalance = false;

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
                        SendState?.Invoke(this, new OpenState(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count, ScreenNumber));
                    }
                    return;

                case 1:
                    return;

                case 4:
                    if (param[1].Equals(Code))
                    {
                        Quantity = param[9].Equals("1") ? -int.Parse(param[4]) : int.Parse(param[4]);
                        AvgPurchase = param[5];
                        OnReceiveBalance = request.QueueCount == 0 ? true : false;
                        SendState?.Invoke(this, new OpenState(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count, ScreenNumber));
                    }
                    return;
            };
        }
        private void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            var sb = new StringBuilder(512);
            int index = Array.FindIndex(Enum.GetNames(typeof(RealType)), o => o.Equals(e.sRealType));

            if (index < 0)
                return;

            foreach (int fid in catalogReal[index])
                sb.Append(API.GetCommRealData(e.sRealKey, fid)).Append(';');

            var param = sb.ToString().Split(';');

            switch (index)
            {
                case 1:
                    if (e.sRealKey.Equals(Code))
                    {
                        SendDatum?.Invoke(this, new Datum(param));
                        SendTrend?.Invoke(this, new OpenTrends(Trend, Volume));
                    }
                    return;

                case 2:
                    if (e.sRealKey.Equals(Code))
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
                        }, param[0], SellOrder, BuyOrder, string.Concat(param[44], ";", param[47])));
                    return;

                case 8:
                    if (e.sRealKey.Equals(Code))
                        SendCurrent?.Invoke(this, new OpenCurrent(Quantity, sb.ToString().Split(';')));

                    return;

                case 9:
                    if (param[0].Equals("e") && DeadLine)
                    {
                        DeadLine = false;

                        if (Temporary != null)
                        {
                            Temporary.SetStorage(Code);
                            OnCollectingData(GetInformation());
                        }
                        else
                            OnReceiveBalance = false;
                    }
                    else if (param[0].Equals("3") && DeadLine == false)
                    {
                        DeadLine = true;
                        Delay.Milliseconds = 205;
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
            int index = Array.FindIndex(catalogTR, o => o.ToString().Contains(e.sTrCode.Substring(1)));

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
                        var sb = new StringBuilder(64);

                        for (y = 0; y <= ly; y++)
                        {
                            ts[x, y] = (string)((object[,])temp)[x, y];

                            if (ts[x, y].Length > 13 && e.sRQName.Split(';')[1].Equals(ts[x, y].Substring(2)))
                            {
                                sb = Exists;
                                e.sPrevNext = "0";

                                break;
                            }
                            sb.Append(ts[x, y]).Append(';');
                        }
                        if (Exists.Equals(sb) == false)
                        {
                            SendMemorize?.Invoke(this, new OpenMemorize(sb));

                            continue;
                        }
                        if (Exists.Equals(sb))
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
                        SendMemorize?.Invoke(this, new OpenMemorize(e.sPrevNext, e.sRQName.Split(';')[0]));
                }
                SetScreenNumber(9000, 9031);
                SendMemorize?.Invoke(this, new OpenMemorize("Clear"));
                Request(GetRandomCode(new Random().Next(0, CodeList.Count)));

                return;
            }
            var str = new StringBuilder(512);
            int i, cnt = API.GetRepeatCnt(e.sTrCode, e.sRQName);

            for (i = 0; i < (cnt > 0 ? cnt : cnt + 1); i++)
            {
                foreach (string item in Array.Find(catalogTR, o => o.ToString().Contains(e.sTrCode.Substring(1))))
                    str.Append(API.GetCommData(e.sTrCode, e.sRQName, i, item).Trim()).Append(';');

                if (cnt > 0)
                    str.Append("*");
            }
            switch (Array.FindIndex(catalogTR, o => o.ToString().Contains(e.sTrCode.Substring(1))))
            {
                case 5:
                    FixUp(str.ToString().Split(';'), e.sRQName);
                    return;

                case 6:
                    foreach (string info in str.ToString().Split('*'))
                        FixUp(info.Split(';'));

                    return;

                case 7:
                case 8:
                    if (str.Length > 1 && e.sRQName.Equals("DoNotRollOver") == false)
                    {
                        if (e.sScrNo.Substring(0, 1).Equals("1"))
                            SellOrder[str.ToString().Split(';')[0]] = double.Parse(e.sRQName.Split(';')[0]);

                        else if (e.sScrNo.Substring(0, 1).Equals("2"))
                            BuyOrder[str.ToString().Split(';')[0]] = double.Parse(e.sRQName.Split(';')[0]);
                    }
                    SendState?.Invoke(this, new OpenState(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count, ScreenNumber));
                    return;

                case 9:
                    SendState?.Invoke(this, new OpenState(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count, ScreenNumber));
                    return;

                case 10:
                    SendDeposit?.Invoke(this, new Deposit(str.ToString().Split(';')));
                    break;

                case 11:
                    new Task(() =>
                    {
                        var temporary = str.ToString().Split('*');

                        for (i = 0; i < temporary.Length; i++)
                            if (temporary[i].Length > 0 && temporary[i].Substring(0, 8).Equals(API.GetFutureCodeByIndex(0)))
                            {
                                var quantity = temporary[i].Split(';');
                                Quantity = quantity[2].Equals("1") ? -int.Parse(quantity[3]) : int.Parse(quantity[3]);
                                AvgPurchase = (double.Parse(quantity[4]) / 100).ToString("F2");
                            }
                        SendBalance?.Invoke(this, new Balance(temporary));
                    }).Start();
                    break;
            }
        }
        private void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            SendErrorMessage(e.nErrCode);
            Code = API.GetFutureCodeByIndex(e.nErrCode);
            RemainingDay(Code);
            CodeList = new List<string>
            {
                Code
            };
            if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 16 && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday)) == false)
            {
                if (Temporary == null)
                    PrepareForTrading(API.GetLoginInfo("ACCLIST"));

                DeadLine = DateTime.Now.Hour < 9 ? false : true;
            }
            else if (Temporary != null)
                OnCollectingData(GetInformation());

            SendCount?.Invoke(this, new NotifyIconText((byte)7));
        }
        private void PrepareForTrading(string account)
        {
            SendCount?.Invoke(this, new NotifyIconText(account, API.GetLoginInfo("USER_ID"), API.GetLoginInfo("USER_NAME"), API.GetLoginInfo("GetServerGubun")));
            SellOrder = new Dictionary<string, double>();
            BuyOrder = new Dictionary<string, double>();
            Trend = new Dictionary<string, string>();
            LookUpTheDeposit(account.Split(';'));
            LookUpTheBalance(account.Split(';'));
            SetPasswordWhileCollectingData(5971);
            OnReceiveBalance = DateTime.Now.Hour > 8 ? true : false;
        }
        private void SetPasswordWhileCollectingData(int wait)
        {
            do
            {
                if (TimerBox.Show(OnReceiveData, GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Information, (uint)(request.QueueCount + wait)).Equals(DialogResult.OK))
                    if (TimerBox.Show(SetPassword, GoblinBat, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, (uint)(request.QueueCount + wait)).Equals(DialogResult.OK))
                        API.KOA_Functions("ShowAccountWindow", "");
            }
            while (request.QueueCount > 0);
        }
        private void OnCollectingData(string[] markets)
        {
            SetScreenNumber(9000, 9031);
            new Task(() => SetScreenNumber(1000, 9000)).Start();
            SetPasswordWhileCollectingData(markets.Length);
            CodeList = RequestCodeList(new List<string>(32), markets);
            Temporary = new Temporary(OpenAPI);
            SendMemorize?.Invoke(this, new OpenMemorize("Clear"));
            Delay.Milliseconds = 4315;
            Request(GetRandomCode(new Random().Next(0, CodeList.Count)));
        }
        private string[] GetInformation()
        {
            int i, l;
            string exclusion, date = GetDistinctDate(CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);
            Delay.Milliseconds = 605;

            for (i = 2; i < 4; i++)
                foreach (var om in API.GetActPriceList().Split(';'))
                {
                    exclusion = API.GetOptionCode(om.Insert(3, "."), i, date);

                    if (CodeList.Exists(o => o.Equals(exclusion)))
                        continue;

                    CodeList.Add(exclusion);
                }
            CodeList[1] = API.GetFutureCodeByIndex(24);
            string[] temp, market = API.GetCodeListByMarket("").Split(';');
            l = market.Length;

            foreach (string output in CodeList)
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
                        if (API.GetMasterCodeName(tempCode).EndsWith(ex) && Array.Exists(exclude, o => o.Equals(tempCode)) == false)
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
            if (CodeList.Count < 1)
                return string.Empty;

            else if (CodeList[index] != null)
            {
                var temp = CodeList[index];
                CodeList.Remove(temp);

                return temp;
            }
            return GetRandomCode(new Random(index).Next(0, CodeList.Count));
        }
        private void Request(string code)
        {
            if (code != null && code.Equals(string.Empty) == false)
            {
                int param = DeadLine ? 4 : (code.Length > 6 ? (code.Substring(5, 3).Equals("000") ? 1 : 2) : 3);
                ITR tr = (ITR)catalogTR[param];
                tr.Value = code;
                tr.RQName = string.Concat(code, ";", GetRetention(param, code));
                tr.PrevNext = 0;
                request.RequestTrData(new Task(() =>
                {
                    InputValueRqData(tr);
                    SendCount?.Invoke(this, new NotifyIconText(CodeList.Count, code));
                }));
                return;
            }
            else if (CodeList.Count < 50)
            {
                foreach (var str in CodeList)
                    if (str == null || str.Equals(string.Empty))
                        CodeList.Remove(str);

                if (CodeList.Count < 1)
                {
                    SendCount?.Invoke(this, new NotifyIconText(CodeList.Count, code));
                    CodeList = null;
                    GC.Collect();

                    return;
                }
            }
            Request(GetRandomCode(new Random().Next(0, CodeList.Count)));
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
                }, param[32], SellOrder, BuyOrder, string.Empty));
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
                SendErrorMessage(API.CommKwRqData(tr.Value, 0, 100, tr.PrevNext, tr.RQName, tr.ScreenNo));
            }));
        }
        private void InputValueRqData(ITR param)
        {
            string[] count = param.ID.Split(';'), value = param.Value.Split(';');
            int i, l = count.Length;

            for (i = 0; i < l; i++)
                API.SetInputValue(count[i], value[i]);

            SendErrorMessage(API.CommRqData(param.RQName, param.TrCode, param.PrevNext, param.ScreenNo));
        }
        private void SendErrorMessage(int error)
        {
            if (error < 0)
            {
                new ExceptionMessage(this.error.GetErrorMessage(error));

                switch (error)
                {
                    case -100:
                    case -101:
                    case -102:
                    case -200:
                        Process.Start("shutdown.exe", "-r");
                        Dispose();
                        return;

                    case -300:
                        SendMemorize?.Invoke(this, new OpenMemorize("Clear"));
                        Request(GetRandomCode(new Random().Next(0, CodeList.Count)));
                        return;

                    default:
                        return;
                }
            }
        }
        private void Dispose()
        {
            SendCount?.Invoke(this, new NotifyIconText('E'));
        }
        private bool DeadLine
        {
            get; set;
        }
        private List<string> CodeList
        {
            get; set;
        }
        private ConnectAPI()
        {
            error = new Error();
            request = Delay.GetInstance(205);
            request.Run();
        }
        private AxKHOpenAPI API
        {
            get; set;
        }
        private static ConnectAPI OpenAPI
        {
            get; set;
        }
        private readonly Error error;
        private readonly Delay request;
        public event EventHandler<Datum> SendDatum;
        public event EventHandler<OpenMemorize> SendMemorize;
        public event EventHandler<NotifyIconText> SendCount;
        public event EventHandler<Quotes> SendQuotes;
        public event EventHandler<Deposit> SendDeposit;
        public event EventHandler<Balance> SendBalance;
        public event EventHandler<OpenCurrent> SendCurrent;
        public event EventHandler<OpenState> SendState;
        public event EventHandler<OpenTrends> SendTrend;
    }
}