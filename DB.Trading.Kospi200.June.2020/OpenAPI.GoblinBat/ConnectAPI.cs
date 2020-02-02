using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxKHOpenAPILib;
using ShareInvest.Catalog;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.GoblinBatControls;
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
        public int RequestQueueCount
        {
            get
            {
                return request.QueueCount;
            }
        }
        public Dictionary<string, string[]> FuturesQuotes
        {
            get; private set;
        }
        public void OnReceiveOrder(PurchaseInformation o)
        {
            request.RequestTrData(new Task(() =>
            {
                ErrorCode = API.SendOrderFO(o.RQName, o.ScreenNo, o.AccNo, o.Code, o.OrdKind, o.SlbyTP, o.OrdTp, o.Qty, o.Price, o.OrgOrdNo);

                if (ErrorCode != 0)
                    new ExceptionMessage(new Error().GetErrorMessage(ErrorCode));
            }));
        }
        public void SetAPI(AxKHOpenAPI axAPI)
        {
            API = axAPI;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveTrData += OnReceiveTrData;
            axAPI.OnReceiveRealData += OnReceiveRealData;
            axAPI.OnReceiveMsg += OnReceiveMsg;
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

                if (ErrorCode != 0)
                    new ExceptionMessage(new Error().GetErrorMessage(ErrorCode));

                return;
            }
            Process.Start("shutdown.exe", "-r");
            Application.ExitThread();
            Application.Exit();
        }
        public static ConnectAPI GetInstance()
        {
            if (api == null)
                api = new ConnectAPI();

            return api;
        }
        private void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            new ExceptionMessage(e.sMsg);

            if (e.sMsg.Equals(new Message().TR))
            {
                SendMemorize?.Invoke(this, new Memorize("Clear"));
                Request(GetRandomCode(new Random().Next(0, Code.Count)));
            }
            else if (e.sMsg.Equals(new Message().Failure))
            {
                Process.Start("shutdown.exe", "-r");
                Application.ExitThread();
                Application.Exit();
            }
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
                    break;

                case 2:
                    new Task(() =>
                    {
                        if (e.sRealKey.Equals(API.GetFutureCodeByIndex(0)))
                        {
                            FuturesQuotes[string.Concat(e.sRealKey, 1)] = new string[]
                            {
                                param[3],
                                param[11],
                                param[19],
                                param[27],
                                param[35]
                            };
                            FuturesQuotes[string.Concat(e.sRealKey, 2)] = new string[]
                            {
                                param[7],
                                param[15],
                                param[23],
                                param[31],
                                param[39]
                            };
                        }
                    }).Start();
                    break;

                case 9:
                    if (param[0].Equals("e") && DeadLine)
                    {
                        DeadLine = false;
                        Delay.delay = 3705;
                        Code = RequestCodeList(new List<string>(32), CodeStorage);
                        SendMemorize?.Invoke(this, new Memorize("Clear"));
                        Request(GetRandomCode(new Random().Next(0, Code.Count)));
                    }
                    else if (param[0].Equals("3") && DeadLine == false)
                    {
                        DeadLine = true;
                        Delay.delay = 205;
                        new Task(() =>
                        {
                            Code = RequestCodeList(new List<string>(32));
                            SendMemorize?.Invoke(this, new Memorize("Clear"));
                            Request(GetRandomCode(new Random().Next(0, Code.Count)));
                        }).Start();
                    }
                    else if (param[0].Equals("0") && param[2].Equals("002000"))
                    {
                        Process.Start("shutdown.exe", "-r");
                        Application.ExitThread();
                        Application.Exit();
                    }
                    break;
            };
        }
        private void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (e.sTrCode.Contains("KOA"))
                return;

            if (Array.FindIndex(catalog, o => o.ToString().Contains(e.sTrCode.Substring(1))) < 4)
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
                case 4:
                    FixUp(Sb.ToString().Split(';'), e.sRQName);
                    break;

                case 5:
                    foreach (string info in Sb.ToString().Split('*'))
                        FixUp(info.Split(';'));

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
                Application.ExitThread();
                Application.Exit();
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
                if (onlyOnce && TimerBox.Show(new Message().OnReceiveData, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, (uint)market.Length).Equals(DialogResult.OK))
                    onlyOnce = true;

                else
                {
                    onlyOnce = true;
                    FuturesQuotes = new Dictionary<string, string[]>();
                    SendCount?.Invoke(this, new NotifyIconText(API.GetLoginInfo("ACCLIST"), API.GetLoginInfo("USER_ID"), API.GetLoginInfo("USER_NAME"), API.GetLoginInfo("GetServerGubun")));
                    SendCount?.Invoke(this, new NotifyIconText(StatisticalAnalysis.GetInstance(market)));
                    SendCount?.Invoke(this, new NotifyIconText(Code[0]));
                }
            }
            while (request.QueueCount > 0);

            if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 16 && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday)) == false)
            {
                DeadLine = true;
                Code = RequestCodeList(new List<string>(32));
                Request(GetRandomCode(new Random(e.nErrCode).Next(0, Code.Count)));
                Delay.delay = 3705;

                if (TimerBox.Show(new Message().SetPassword, "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, (uint)market.Length).Equals(DialogResult.OK))
                    API.KOA_Functions("ShowAccountWindow", "");
            }
            else
            {
                Code = RequestCodeList(new List<string>(32), market);
                Request(GetRandomCode(new Random(e.nErrCode).Next(0, Code.Count)));
                Delay.delay = 4135;
            }
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
                int param = DeadLine ? 3 : (code.Length > 6 ? (code.Substring(5, 3).Equals("000") ? 0 : 1) : 2);
                ITR tr = (ITR)catalog[param];
                tr.Value = code;
                tr.RQName = string.Concat(code, ";", Retention(param, code));
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
                    new ExceptionMessage(new Error().GetErrorMessage(ErrorCode));
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
                new ExceptionMessage(new Error().GetErrorMessage(ErrorCode));

                if (ErrorCode == -200)
                    Process.Start("shutdown.exe", "-r");
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
        private readonly Delay request;
        private readonly RealType real;
        public event EventHandler<Datum> SendDatum;
        public event EventHandler<Memorize> SendMemorize;
        public event EventHandler<NotifyIconText> SendCount;
    }
}