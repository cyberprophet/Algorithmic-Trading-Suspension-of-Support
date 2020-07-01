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
using ShareInvest.Verify;

namespace ShareInvest.OpenAPI
{
    public class ConnectAPI : AuxiliaryFunction
    {
        public static ConnectAPI GetInstance() => OpenAPI;
        public static ConnectAPI GetInstance(string key, int delay)
        {
            if (OpenAPI == null)
                OpenAPI = new ConnectAPI(key, delay);

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
        public void Dispose(bool reset)
        {
            if (reset)
            {
                if (request != null)
                    request.Dispose();

                if (API != null)
                    API = null;

                if (OpenAPI != null)
                    OpenAPI = null;
            }
        }
        public void SetScreenNumber(uint start, uint finish)
        {
            for (uint i = start; i < finish; i++)
                API.DisconnectRealData(i.ToString("D4"));
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
        public void LookUpTheDeposit(string[] account) => request.RequestTrData(new Task(() =>
        {
            InputValueRqData(new OPW20010
            {
                Value = string.Concat(Array.Find(account, o => o.Substring(8, 2).Equals("31")), ";;00"),
                PrevNext = 0
            });
        }));
        public void LookUpTheBalance(string[] account) => request.RequestTrData(new Task(() =>
        {
            InputValueRqData(new OPW20007
            {
                Value = string.Concat(Array.Find(account, o => o.Substring(8, 2).Equals("31")), ";;00"),
                PrevNext = 0
            });
        }));
        public void StartProgress(string transfer)
        {
            if (transfer != null)
                foreach (string temp in new Transfer(transfer, KeyDecoder.GetWindowsProductKeyFromRegistry()))
                {
                    if (!temp.Contains(","))
                    {
                        string code = temp.Equals("Tick") || temp.Equals("Day") ? "101Q3000" : temp;
                        SendMemorize?.Invoke(this, new Memorize(temp.Equals("Day") ? "day" : temp, code));

                        continue;
                    }
                    SendMemorize?.Invoke(this, new Memorize(temp.Split(',')));
                }
        }
        public void StartProgress(Temporary temporary)
        {
            if (API != null)
            {
                SendErrorMessage(API.CommConnect());
                Temporary = temporary;

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
            CodeList = RequestCodeList(new List<string>(32)).Result;
            SendMemorize?.Invoke(this, new Memorize("Clear"));
            Request(GetRandomCode(new Random().Next(0, CodeList.Count)));
        }
        public int OnReceiveOrder(string code, int price)
        {
            if (Cash > price * 1.00015)
            {
                Cash -= (long)(price * 1.00015);
                OnReceiveOrder(new CollectedInformation
                {
                    RQName = string.Concat(API.GetMasterCodeName(code), ';', price, ';', Cash),
                    ScreenNo = string.Concat((int)OrderType.신규매수, GetScreenNumber().ToString("D3")),
                    AccNo = secret.Account,
                    OrderType = (int)OrderType.신규매수,
                    Code = code,
                    Qty = 1,
                    Price = price,
                    HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                    OrgOrderNo = string.Empty
                });
            }
            return price - GetQuoteUnit(price, API.KOA_Functions("GetMasterStockInfo", code).Split(';')[0].Contains(market));
        }
        public void OnReceiveOrder(PurchaseInformation o) => request.RequestTrData(new Task(() => SendErrorMessage(API.SendOrderFO(o.RQName, o.ScreenNo, o.AccNo, o.Code, o.OrdKind, o.SlbyTP, o.OrdTp, o.Qty, o.Price, o.OrgOrdNo))));
        void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            if (Array.Exists(basic, o => o.Equals(e.sMsg.Substring(9))))
            {
                var temp = e.sMsg.Substring(9);

                if ((temp.Equals(basic[2]) || temp.Equals(basic[6]) || temp.Equals(basic[8])) && OnReceiveBalance == false)
                    OnReceiveBalance = request.QueueCount == 0;

                if (e.sMsg.Contains("모의투자"))
                    temp = temp.Replace("모의투자 ", string.Empty);

                if (e.sMsg.Last().Equals('다') || e.sMsg.Last().Equals('요'))
                    temp = string.Concat(temp, ".");

                SendState?.Invoke(this, new State(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count, ScreenNumber));
                SendCount?.Invoke(this, new NotifyIconText(temp));

                return;
            }
            if (e.sMsg.Equals(TR) || e.sMsg.Equals(Failure))
            {
                SendMemorize?.Invoke(this, new Memorize("Clear"));
                Request(GetRandomCode(new Random().Next(0, CodeList.Count)));
            }
            if (e.sMsg.Contains(Response) && TimerBox.Show(string.Concat(Response, "."), GoblinBat, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, 1375).Equals(DialogResult.OK))
                return;

            if (Array.Exists(message, o => o.Equals(e.sMsg.Substring(9))))
                return;

            new ExceptionMessage(e.sMsg, e.sRQName);
        }
        void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
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
                                    OnReceiveBalance = request.QueueCount == 0;

                                break;

                            case "확인":
                                if (param[12].Substring(3).Equals("취소") || param[12].Substring(3).Equals("정정"))
                                    OnReceiveBalance = request.QueueCount == 0;

                                break;
                        }
                        SendState?.Invoke(this, new State(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count, ScreenNumber));
                    }
                    return;

                case 1:
                    return;

                case 4:
                    if (param[1].Equals(Code))
                    {
                        Quantity = param[9].Equals("1") ? -int.Parse(param[4]) : int.Parse(param[4]);
                        AvgPurchase = param[5];
                        OnReceiveBalance = request.QueueCount == 0;
                        SendState?.Invoke(this, new State(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count, ScreenNumber));
                    }
                    return;
            }
        }
        void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
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
                case 0b1:
                    if (e.sRealKey.Equals(Code))
                    {
                        SendDatum?.Invoke(this, new Datum(param));
                        SendTrend?.Invoke(this, new Trends(Trend, Volume));
                    }
                    return;

                case 0b10:
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

                case 0b1000:
                    if (e.sRealKey.Equals(Code))
                        SendCurrent?.Invoke(this, new Current(Quantity, param));

                    return;

                case 0b1001:
                    if (param[0].Equals("e") && DeadLine)
                    {
                        DeadLine = false;

                        if (Temporary != null)
                        {
                            Temporary.SetStorage(Code);
                            SendCount?.Invoke(this, new NotifyIconText(-106));
                        }
                        else
                            OnReceiveBalance = false;
                    }
                    else if (param[0].Equals("3") && DeadLine == false)
                    {
                        DeadLine = true;
                        Delay.Milliseconds = 205;

                        if (Temporary == null)
                            OnReceiveBalance = true;
                    }
                    break;

                case 0xA:
                    SendStocksDatum?.Invoke(this, new Stocks(e.sRealKey, param));
                    return;

                case 0xB:
                    SendStocksQuotes?.Invoke(this, new StocksQuotes(e.sRealKey, param[0b100]));
                    return;
            }
        }
        void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
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
                            SendMemorize?.Invoke(this, new Memorize(sb));

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
                        SendMemorize?.Invoke(this, new Memorize(e.sPrevNext, e.sRQName.Split(';')[0]));
                }
                SetScreenNumber(9000, 9031);
                SendMemorize?.Invoke(this, new Memorize("Clear"));
                Request(GetRandomCode(new Random().Next(0, CodeList.Count)));

                if (DateTime.Now.Hour == 16 && secret.IsServer(key))
                    SendCount?.Invoke(this, new NotifyIconText(-106));

                return;
            }
            var str = new StringBuilder(512);
            int i, cnt = API.GetRepeatCnt(e.sTrCode, e.sRQName);

            for (i = 0; i < (cnt > 0 && index != 0xC ? cnt : cnt + 1); i++)
            {
                Opw00005.Switch = index == 0b1100 && i == 0;

                foreach (string item in Array.Find(catalogTR, o => o.ToString().Contains(e.sTrCode.Substring(1))))
                    str.Append(API.GetCommData(e.sTrCode, e.sRQName, Opw00005.Switch == false && index == 0xC ? i - 1 : i, item).Trim()).Append(';');

                if (cnt > 0)
                    str.Append("*");
            }
            switch (index)
            {
                case 0b101:
                    FixUp(str.ToString().Split(';'), e.sRQName);
                    return;

                case 0b110:
                    foreach (string info in str.ToString().Split('*'))
                        FixUp(info.Split(';'));

                    return;

                case 0b111:
                case 0b1000:
                    if (str.Length > 1 && e.sRQName.Equals("DoNotRollOver") == false)
                    {
                        if (e.sScrNo.Substring(0, 1).Equals("1"))
                            SellOrder[str.ToString().Split(';')[0]] = double.Parse(e.sRQName.Split(';')[0]);

                        else if (e.sScrNo.Substring(0, 1).Equals("2"))
                            BuyOrder[str.ToString().Split(';')[0]] = double.Parse(e.sRQName.Split(';')[0]);
                    }
                    SendState?.Invoke(this, new State(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count, ScreenNumber));
                    return;

                case 0b1001:
                    SendState?.Invoke(this, new State(OnReceiveBalance, SellOrder.Count, Quantity, BuyOrder.Count, ScreenNumber));
                    return;

                case 0b1010:
                    SendDeposit?.Invoke(this, new Deposit(str.ToString().Split(';')));
                    break;

                case 0b1011:
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

                case 0b1100:
                    SetCollectionConditions(str.ToString().Split('*'));
                    var cs = CallUpStorage;
                    str.Clear();

                    if (cs != null && str.Length == 0)
                    {
                        foreach (var cn in cs.Item2.Split(';'))
                            str.Append(API.GetMasterCodeName(cn)).Append(';');

                        var param = str.Remove(str.Length - 1, 1).ToString();
                        var tuple = new Tuple<string, string>(cs.Item2, param);
                        SendCount?.Invoke(this, new NotifyIconText(tuple));
                        new ExceptionMessage(tuple.Item2.Replace(';', '\n'), Cash.ToString("C0"));
                        RemainingDay(cs);
                    }
                    break;

                case 0b1101:
                case 0b1110:
                case 0b1111:
                case 0b10000:
                    SendCount?.Invoke(this, new NotifyIconText(str.Remove(str.Length - 1, 1).ToString(), e.sRQName.Split(';')));
                    break;
            }
        }
        void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            SendErrorMessage(e.nErrCode);
            var code = API.GetFutureCodeByIndex(e.nErrCode);
            Code = OnReceiveRemainingDay(code).Equals(DateTime.Now.ToString(format)) ? API.GetFutureCodeByIndex(1) : code;
            RemainingDay(Code);
            CodeList = new List<string>
            {
                Code
            };
            if (DateTime.Now.Hour > 4 && (DateTime.Now.Hour < 15 || DateTime.Now.Hour == 15 && DateTime.Now.Minute < 45) && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday)) == false)
            {
                if (Temporary == null)
                    PrepareForTrading(API.GetLoginInfo("ACCLIST"));

                DeadLine = DateTime.Now.Hour >= 9;

                if (secret.IsCollector(key, API.GetLoginInfo("ACCLIST"), API.GetLoginInfo("USER_ID")) && string.IsNullOrEmpty(secret.Account) == false)
                    LookUpTheBalance(secret.Account);
            }
            else if (Temporary != null)
            {
                OnCollectingData(GetInformation());
                Request();
            }
            SendCount?.Invoke(this, new NotifyIconText((byte)e.nErrCode));
        }
        void PrepareForTrading(string account)
        {
            SendCount?.Invoke(this, new NotifyIconText(account, API.GetLoginInfo("USER_ID"), API.GetLoginInfo("USER_NAME"), API.GetLoginInfo("GetServerGubun")));
            SellOrder = new Dictionary<string, double>();
            BuyOrder = new Dictionary<string, double>();
            Trend = new Dictionary<string, string>();
            LookUpTheDeposit(account.Split(';'));
            LookUpTheBalance(account.Split(';'));
            SetPasswordWhileCollectingData(5971);
            OnReceiveBalance = DateTime.Now.Hour > 8;
        }
        void SetPasswordWhileCollectingData(int wait)
        {
            do
            {
                if (TimerBox.Show(OnReceiveData, GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Information, (uint)(request.QueueCount + wait)).Equals(DialogResult.OK))
                    if (TimerBox.Show(SetPassword, GoblinBat, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, (uint)(request.QueueCount + wait)).Equals(DialogResult.OK))
                        API.KOA_Functions("ShowAccountWindow", "");
            }
            while (request.QueueCount > 0);
        }
        void OnCollectingData(string[] markets)
        {
            Temporary.SetConnection(OpenAPI, key);
            SetScreenNumber(8900, 9050);
            new Task(() => SetScreenNumber(1000, 8900)).Start();
            SetPasswordWhileCollectingData(markets.Length);
            CodeList = RequestCodeList(new List<string>(32), markets);
            SendMemorize?.Invoke(this, new Memorize("Clear"));
            var server = DateTime.Now.Hour == 15 && secret.IsServer(key);
            Delay.Milliseconds = server ? 1935 : 4315;

            if (server)
                Request(Code);

            else
                Request(GetRandomCode(new Random().Next(0, CodeList.Count)));
        }
        string[] GetInformation()
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
            string[] temp, market = API.GetCodeListByMarket(string.Empty).Split(';');
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
                    foreach (string ex in new CodeListByExclude())
                        if (API.GetMasterCodeName(tempCode).EndsWith(ex) && Array.Exists(exclude, o => o.Equals(tempCode)) == false)
                            market[i] = string.Empty;
            }
            foreach (string output in SetCodeStorage(market))
                RemainingDay(output);

            return market;
        }
        string GetRandomCode(int index)
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
        void Request(string code)
        {
            if (string.IsNullOrEmpty(code) == false)
            {
                int param = DeadLine ? 4 : (code.Length > 6 ? (code.Substring(5, 3).Equals("000") ? 1 : 2) : 3);
                ITRs tr = (ITRs)catalogTR[param];
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
                foreach (var str in CodeList)
                    if (str == null || str.Equals(string.Empty))
                        CodeList.Remove(str);

            if (CodeList.Count == 0)
            {
                SendCount?.Invoke(this, new NotifyIconText(CodeList.Count, code));
                CodeList = null;
                GC.Collect();

                return;
            }
            Request(GetRandomCode(new Random().Next(0, CodeList.Count)));
        }
        void Request()
        {
            if (DateTime.Now.AddDays(1).ToString(format).Equals(OnReceiveRemainingDay(Code)))
            {
                var code = API.GetFutureCodeByIndex(1);
                RemainingDay(code);
                Code = code;
            }
        }
        void FixUp(string[] param, string code)
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
        void RemainingDay(string code)
        {
            if (code.Length == 0b1000)
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
                ITRs tr = new OPTKWFID
                {
                    Value = code,
                    PrevNext = 0
                };
                SendErrorMessage(API.CommKwRqData(tr.Value, 0, 0x64, tr.PrevNext, tr.RQName, tr.ScreenNo));
            }));
        }
        void RemainingDay(Tuple<int, string> param)
        {
            if (param.Item1 < 0x65)
                request.RequestTrData(new Task(() =>
                {
                    ITRs tr = new OPTKWFID
                    {
                        Value = param.Item2,
                        PrevNext = 0
                    };
                    SendErrorMessage(API.CommKwRqData(tr.Value, 0, param.Item1, tr.PrevNext, tr.RQName, tr.ScreenNo));
                }));
            else if (TimerBox.Show(secret.LookUp, secret.GetNumberOfStocks(param.Item1), MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, (uint)Math.Pow(param.Item1, 0b10)).Equals(DialogResult.Retry))
                new ExceptionMessage(secret.LookUp, secret.GetNumberOfStocks(param.Item1));
        }
        void InputValueRqData(ITRs param)
        {
            string[] count = param.ID.Split(';'), value = param.Value.Split(';');
            int i, l = count.Length;

            for (i = 0; i < l; i++)
                API.SetInputValue(count[i], value[i]);

            SendErrorMessage(API.CommRqData(param.RQName, param.TrCode, param.PrevNext, param.ScreenNo));
        }
        void SendErrorMessage(int error)
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

                    case -106:
                        SendCount?.Invoke(this, new NotifyIconText(error));
                        break;

                    case -300:
                        SendMemorize?.Invoke(this, new Memorize("Clear"));
                        Request(GetRandomCode(new Random().Next(0, CodeList.Count)));
                        return;

                    default:
                        return;
                }
                if (API != null)
                    API = null;

                if (OpenAPI != null)
                    OpenAPI = null;
            }
        }
        void Dispose()
        {
            new Task(() => SendCount?.Invoke(this, new NotifyIconText((char)69))).Start();
            Dispose(true);
        }
        void LookUpTheBalance(string account) => request.RequestTrData(new Task(() =>
        {
            InputValueRqData(new Opw00005
            {
                Value = string.Concat(account, ";;00"),
                PrevNext = 0
            });
        }));
        void OnReceiveOrder(CollectedInformation o) => request.RequestTrData(new Task(() => SendErrorMessage(API.SendOrder(o.RQName, o.ScreenNo, o.AccNo, o.OrderType, o.Code, o.Qty, o.Price, o.HogaGb, o.OrgOrderNo))));
        void SetCollectionConditions(string[] param)
        {
            if (long.TryParse(param[0].Split(';')[7], out long cash))
            {
                Cash = cash;

                for (int i = 1; i < param.Length - 1; i++)
                {
                    var co = FixUp(param[i]);
                    uint quantity = co.Amount, price = uint.TryParse(API.GetMasterLastPrice(co.Code), out uint before) ? before : 0;

                    if (co.Price > 0 && price > 0 && quantity > 0)
                    {
                        var stock = API.KOA_Functions("GetMasterStockInfo", co.Code).Split(';')[0].Contains(market);
                        int sell = (int)(co.Purchase * 1.05), buy = (int)(co.Purchase * 0.95), upper = (int)(price * 1.3), lower = (int)(price * 0.7), bPrice = GetStartingPrice(lower, stock), sPrice = GetStartingPrice(sell, stock);

                        while (sPrice < upper)
                        {
                            if (sPrice > lower && quantity-- > 0)
                                OnReceiveOrder(new CollectedInformation
                                {
                                    RQName = string.Concat(co.Name, ';', sPrice, ';', Cash),
                                    ScreenNo = string.Concat((int)OrderType.신규매도, GetScreenNumber().ToString("D2"), i % 10),
                                    AccNo = secret.Account,
                                    OrderType = (int)OrderType.신규매도,
                                    Code = co.Code,
                                    Qty = 1,
                                    Price = sPrice,
                                    HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                                    OrgOrderNo = string.Empty
                                });
                            sPrice += GetQuoteUnit(sPrice, stock);
                        }
                        while (bPrice < upper && bPrice < buy && Cash > bPrice * 1.00015)
                        {
                            OnReceiveOrder(new CollectedInformation
                            {
                                RQName = string.Concat(co.Name, ';', bPrice, ';', Cash),
                                ScreenNo = string.Concat((int)OrderType.신규매수, GetScreenNumber().ToString("D2"), i % 10),
                                AccNo = secret.Account,
                                OrderType = (int)OrderType.신규매수,
                                Code = co.Code,
                                Qty = 1,
                                Price = bPrice,
                                HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                                OrgOrderNo = string.Empty
                            });
                            bPrice += GetQuoteUnit(bPrice, stock);
                            Cash -= (long)(bPrice * 1.00015);
                        }
                        SetCodeStorage(co.Code);
                    }
                }
            }
        }
        bool DeadLine
        {
            get; set;
        }
        long Cash
        {
            get; set;
        }
        List<string> CodeList
        {
            get; set;
        }
        ConnectAPI(string key, int delay) : base(key)
        {
            error = new Error();
            secret = new Secrets();
            request = Delay.GetInstance(delay);
            request.Run();
        }
        AxKHOpenAPI API
        {
            get; set;
        }
        static ConnectAPI OpenAPI
        {
            get; set;
        }
        readonly Error error;
        readonly Delay request;
        readonly Secrets secret;
        public event EventHandler<Datum> SendDatum;
        public event EventHandler<Memorize> SendMemorize;
        public event EventHandler<NotifyIconText> SendCount;
        public event EventHandler<Quotes> SendQuotes;
        public event EventHandler<Deposit> SendDeposit;
        public event EventHandler<Balance> SendBalance;
        public event EventHandler<Current> SendCurrent;
        public event EventHandler<State> SendState;
        public event EventHandler<Trends> SendTrend;
        public event EventHandler<Stocks> SendStocksDatum;
        public event EventHandler<StocksQuotes> SendStocksQuotes;
    }
}