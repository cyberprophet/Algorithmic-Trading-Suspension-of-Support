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
        public Dictionary<double, string[]> OrderNumber
        {
            get; private set;
        }
        public int OnReceiveBalance
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
        public void OnReceiveOrder(PurchaseInformation o)
        {
            ErrorCode = API.SendOrderFO(o.RQName, o.ScreenNo, o.AccNo, o.Code, o.OrdKind, o.SlbyTP, o.OrdTp, o.Qty, o.Price, o.OrgOrdNo);

            if (ErrorCode < 0)
                new ExceptionMessage(error.GetErrorMessage(ErrorCode));
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
        public void Dispose()
        {
            SendCount?.Invoke(this, new NotifyIconText('E'));
        }
        public static ConnectAPI GetInstance()
        {
            if (api == null)
                api = new ConnectAPI();

            return api;
        }
        private void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            if (Array.Exists(message.Basic, o => o.Equals(e.sMsg.Substring(9))))
            {
                new Task(() =>
                {
                    var temp = e.sMsg.Substring(9);

                    if (e.sMsg.Contains("모의투자"))
                        temp.Replace("모의투자 ", string.Empty);

                    if (e.sMsg.Last().Equals('다') || e.sMsg.Last().Equals('요'))
                        temp = string.Concat(temp, ".");

                    Console.WriteLine(OnReceiveBalance + "\t" + temp);
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
            if (e.sMsg.Equals(message.Failure))
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
                    if (param[5].Equals("체결"))
                        OrderNumber.Remove(double.Parse(param[8]));

                    return;

                case 1:
                    return;

                case 4:
                    if (param[1].Equals(API.GetFutureCodeByIndex(0)))
                        Quantity = param[9].Equals("1") ? -int.Parse(param[4]) : int.Parse(param[4]);

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
                    new Task(() =>
                    {
                        if (e.sRealKey.Equals(API.GetFutureCodeByIndex(0)))
                            SendDatum?.Invoke(this, new Datum(param));
                    }).Start();
                    return;

                case 2:
                    new Task(() =>
                    {
                        if (e.sRealKey.Equals(API.GetFutureCodeByIndex(0)))
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
                            }, param[0], OrderNumber));
                    }).Start();
                    return;

                case 8:
                    new Task(() =>
                    {
                        if (e.sRealKey.Equals(API.GetFutureCodeByIndex(0)))
                            SendCurrent?.Invoke(this, new Current(Quantity, Sb.ToString().Split(';')));
                    }).Start();
                    return;

                case 9:
                    if (param[0].Equals("e") && DeadLine)
                    {
                        DeadLine = false;
                        OnReceiveBalance++;
                        Code = RequestCodeList(new List<string>(32), CodeStorage);
                        SendMemorize?.Invoke(this, new Memorize("Clear"));
                        Delay.delay = 3705;
                        Request(GetRandomCode(new Random().Next(0, Code.Count)));
                    }
                    else if (param[0].Equals("3") && DeadLine == false)
                    {
                        DeadLine = true;
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
                                Sb = new StringBuilder(exists);
                                e.sPrevNext = "0";

                                break;
                            }
                            Sb.Append(ts[x, y]).Append(';');
                        }
                        if (!exists.Equals(Sb.ToString()))
                        {
                            SendMemorize?.Invoke(this, new Memorize(Sb));

                            continue;
                        }
                        if (exists.Equals(Sb.ToString()))
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
                    if (Sb.Length > 1)
                    {
                        OrderNumber[double.Parse(e.sRQName)] = new string[]
                        {
                            e.sRQName,
                            Sb.ToString().Split(';')[0]
                        };
                        OnReceiveBalance--;
                    }
                    break;

                case 8:
                    if (Sb.Length > 1)
                    {
                        var temp = e.sRQName.Split(';');
                        var price = double.Parse(temp[0]);

                        if (OrderNumber.TryGetValue(price, out string[] correction))
                        {
                            OrderNumber[double.Parse(temp[1])] = correction;
                            OrderNumber.Remove(price);
                            OnReceiveBalance--;
                        }
                    }
                    break;

                case 9:
                    if (Sb.Length > 1)
                    {
                        var price = double.Parse(e.sRQName.Split(';')[0]);

                        if (OrderNumber.ContainsKey(price))
                        {
                            OrderNumber.Remove(price);
                            OnReceiveBalance--;
                        }
                    }
                    break;

                case 10:
                    new Task(() => SendDeposit?.Invoke(this, new Deposit(Sb.ToString().Split(';')))).Start();
                    break;

                case 11:
                    new Task(() => SendBalance?.Invoke(this, new Balance(Sb.ToString().Split('*')))).Start();
                    break;
            }
        }
        private void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            int i, l;
            bool onlyOnce = false;
            string exclusion, date = GetDistinctDate(CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);

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
                        if (API.GetMasterCodeName(tempCode).EndsWith(ex) && !Array.Exists(exclude, o => o.Equals(tempCode)))
                            market[i] = string.Empty;

                    continue;
                }
            }
            foreach (string output in SetCodeStorage(market))
                RemainingDay(output);

            do
            {
                if (onlyOnce == false)
                {
                    onlyOnce = true;
                    OrderNumber = new Dictionary<double, string[]>();
                    SendCount?.Invoke(this, new NotifyIconText(API.GetLoginInfo("ACCLIST"), API.GetLoginInfo("USER_ID"), API.GetLoginInfo("USER_NAME"), API.GetLoginInfo("GetServerGubun")));
                }
                else if (onlyOnce && TimerBox.Show(message.OnReceiveData, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, (uint)(request.QueueCount + market.Length)).Equals(DialogResult.OK))
                    if (TimerBox.Show(message.SetPassword, "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, (uint)(request.QueueCount + market.Length)).Equals(DialogResult.OK))
                        API.KOA_Functions("ShowAccountWindow", "");
            }
            while (request.QueueCount > 0);

            if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 16 && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday)) == false)
            {
                DeadLine = true;
                Delay.delay = 205;
                OnReceiveBalance--;
            }
            else
            {
                Code = RequestCodeList(new List<string>(32), market);
                Request(GetRandomCode(new Random(e.nErrCode).Next(0, Code.Count)));
                Delay.delay = 4135;
            }
            SendCount?.Invoke(this, new NotifyIconText((byte)7));
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
                    SendCount?.Invoke(this, new NotifyIconText(Code.Count));
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
                }, param[32], OrderNumber));
            SetInsertCode(code, param[72], param[63]);
        }
        private void RemainingDay(string code)
        {
            if (code.Length < 9 && code.Length > 6)
            {
                request.RequestTrData(new Task(() => InputValueRqData(new Opt50001
                {
                    Value = code,
                    RQName = code
                })));
                return;
            }
            request.RequestTrData(new Task(() =>
            {
                ITR tr = new OPTKWFID
                {
                    Value = code
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
            }
        }
        private int ErrorCode
        {
            get; set;
        }
        private bool DeadLine
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
    }
}