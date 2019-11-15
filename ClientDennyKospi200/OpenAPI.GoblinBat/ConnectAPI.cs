using System;
using System.Collections.Generic;
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
using ShareInvest.TimerMessageBox;

namespace ShareInvest.OpenAPI
{
    public class ConnectAPI : DistinctDate
    {
        public void SetAPI(AxKHOpenAPI axAPI)
        {
            this.axAPI = axAPI;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveTrData += OnReceiveTrData;
            axAPI.OnReceiveRealData += OnReceiveRealData;
            axAPI.OnReceiveChejanData += OnReceiveChejanData;
            axAPI.OnReceiveMsg += OnReceiveMsg;
        }
        public void StartProgress(IRealType type)
        {
            if (axAPI != null)
            {
                this.type = type;
                ErrorCode = axAPI.CommConnect();

                if (ErrorCode != 0)
                    new Error(ErrorCode);

                return;
            }
            Environment.Exit(0);
        }
        public void LookUpTheDeposit(string account)
        {
            if (account.Contains("-"))
                Account = account.Replace("-", string.Empty);

            request.RequestTrData(new Task(() =>
            {
                InputValueRqData(new OPW20007 { Value = string.Concat(Account, ";;00") });
                InputValueRqData(new OPW20010 { Value = string.Concat(Account, ";;00") });
            }));
        }
        public void OnReceiveOrder(IStrategy order)
        {
            request.RequestTrData(new Task(() =>
            {
                if (ConfirmOrder.Get().CheckCurrent())
                    ErrorCode = axAPI.SendOrderFO(string.Concat(Code[0].Substring(0, 8), ScreenNo), ScreenNo, Account, Code[0].Substring(0, 8), 1, order.SlbyTP, order.OrdTp, order.Qty, order.Price, "");

                if (ErrorCode != 0)
                    new Error(ErrorCode);
            }));
        }
        public Dictionary<int, string> Code
        {
            get; private set;
        }
        public bool OnReceiveBalance
        {
            get; set;
        } = true;
        public int Quantity
        {
            get; private set;
        }
        public static ConnectAPI Get()
        {
            if (api == null)
                api = new ConnectAPI();

            return api;
        }
        private void InputValueRqData(ITR param)
        {
            string[] count = param.ID.Split(';'), value = param.Value.Split(';');
            int i, l = count.Length;

            for (i = 0; i < l; i++)
                axAPI.SetInputValue(count[i], value[i]);

            ErrorCode = axAPI.CommRqData(param.RQName, param.TrCode, param.PrevNext, param.ScreenNo);

            if (ErrorCode != 0)
                new Error(ErrorCode);
        }
        private void FixUp(StringBuilder sb, string code)
        {
            string[] arr = sb.ToString().Split(';');

            Code[Squence++] = string.Concat(code, ";", arr[72], ";", arr[63], ";", arr[45]);
        }
        private void Request(string code)
        {
            if (code.Substring(0, 3).Equals("101") && code.Length < 9)
            {
                request.RequestTrData(new Task(() => InputValueRqData(new Opt50028 { Value = code, RQName = string.Concat(code, Retention(code)), PrevNext = 0 })));

                return;
            }
            foreach (KeyValuePair<int, string> kv in Code)
                if (kv.Value.Substring(0, 8).Equals(code.Substring(0, 8)))
                {
                    if (Code.Last().Key.Equals(kv.Key))
                        SendConfirm?.Invoke(this, new Identify("The latest Data Collection is Complete."));

                    request.RequestTrData(new Task(() => InputValueRqData(new Opt50066 { Value = Code[kv.Key + 1].Substring(0, 8), RQName = string.Concat(Code[kv.Key + 1].Substring(0, 8), Retention(Code[kv.Key + 1].Substring(0, 8))).Substring(0, 20), PrevNext = 0 })));
                    SendConfirm?.Invoke(this, new Identify("Continues to look up " + Code[kv.Key + 1].Substring(0, 8) + "\nChart for the " + kv.Key + " Time."));
                }
        }
        private void RemainingDay(string code)
        {
            request.RequestTrData(new Task(() => InputValueRqData(new Opt50001 { Value = code, RQName = code })));
        }
        private void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            int i;
            string exclusion, date = GetDistinctDate(CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);
            List<string> code = new List<string>
            {
                axAPI.GetFutureCodeByIndex(e.nErrCode)
            };
            for (i = 2; i < 4; i++)
                foreach (var om in axAPI.GetActPriceList().Split(';'))
                {
                    exclusion = axAPI.GetOptionCode(om.Insert(3, "."), i, date);

                    if (code.Exists(o => o.Equals(exclusion)))
                        continue;

                    code.Add(exclusion);
                }
            if (TimerBox.Show("Are You using Automatic Login??\n\nIf You aren't using It,\nClick No.\n\nAfter 5 Seconds,\nIt's Regarded as an Automatic Mode and Proceeds.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 5617).Equals((DialogResult)7))
                axAPI.KOA_Functions("ShowAccountWindow", "");

            SendAccount?.Invoke(this, new Account(axAPI.GetLoginInfo("ACCLIST"), axAPI.GetLoginInfo("USER_ID"), axAPI.GetLoginInfo("USER_NAME"), axAPI.GetLoginInfo("GetServerGubun")));
            code.RemoveAt(1);
            Delay.delay = 615;

            foreach (string output in code)
                RemainingDay(output);

            if (TimerBox.Show("Do You Want to Retrieve Recent Data?\n\nPress 'YES' after 25 Seconds to Receive Data.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, 105752).Equals((DialogResult)6))
            {
                Delay.delay = 4150;
                Request(code[0]);

                return;
            }
            SendConfirm?.Invoke(this, new Identify("It is valid until ", DateTime.ParseExact(Code[0].Substring(18, 8), "yyyyMMdd", null)));
            Delay.delay = 205;
        }
        private void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (e.sTrCode.Equals("opt50028") || e.sTrCode.Equals("opt50066"))
            {
                var temp = axAPI.GetCommDataEx(e.sTrCode, e.sRQName);

                if (temp != null)
                {
                    string[,] ts = new string[((object[,])temp).GetUpperBound(0) + 1, ((object[,])temp).GetUpperBound(1) + 1];
                    int x, y, lx = ((object[,])temp).GetUpperBound(0), ly = ((object[,])temp).GetUpperBound(1);

                    for (x = 0; x <= lx; x++)
                    {
                        sb = new StringBuilder(64);

                        for (y = 0; y <= ly; y++)
                        {
                            ts[x, y] = (string)((object[,])temp)[x, y];

                            if (ts[x, y].Length > 13 && e.sRQName.Substring(8).Equals(ts[x, y].Substring(2)))
                            {
                                sb = new StringBuilder(it);
                                e.sPrevNext = "0";

                                break;
                            }
                            sb.Append(ts[x, y]).Append(';');
                        }
                        if (!it.Equals(sb.ToString()))
                        {
                            SendMemorize?.Invoke(this, new Memorize(sb));

                            continue;
                        }
                        if (it.Equals(sb.ToString()))
                            break;
                    }
                    if (e.sRQName.Substring(0, 3).Equals("101") && e.sPrevNext.Equals("2"))
                    {
                        request.RequestTrData(new Task(() => InputValueRqData(new Opt50028 { Value = e.sRQName.Substring(0, 8), RQName = e.sRQName, PrevNext = 2 })));

                        return;
                    }
                    if (e.sPrevNext.Equals("2"))
                    {
                        request.RequestTrData(new Task(() => InputValueRqData(new Opt50066 { Value = e.sRQName.Substring(0, 8), RQName = e.sRQName, PrevNext = 2 })));

                        return;
                    }
                    if (e.sPrevNext.Equals("0"))
                        SendMemorize?.Invoke(this, new Memorize(e.sPrevNext, e.sRQName.Substring(0, 8)));
                }
                Request(e.sRQName);
                return;
            }
            sb = new StringBuilder(512);
            int i, cnt = axAPI.GetRepeatCnt(e.sTrCode, e.sRQName);

            for (i = 0; i < (cnt > 0 ? cnt : cnt + 1); i++)
            {
                foreach (string item in Array.Find(catalog, o => o.ToString().Contains(e.sTrCode.Substring(1))))
                    sb.Append(axAPI.GetCommData(e.sTrCode, e.sRQName, i, item).Trim()).Append(';');

                if (cnt > 0)
                    sb.Append("*");
            }
            switch (Array.FindIndex(catalog, o => o.ToString().Contains(e.sTrCode.Substring(1))))
            {
                case 0:
                    FixUp(sb, e.sRQName);
                    break;

                case 1:
                    SendDeposit?.Invoke(this, new Deposit(sb));
                    break;

                case 2:
                    SendHolding?.Invoke(this, new Holding(sb.ToString()));
                    break;
            }
        }
        private void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            sb = new StringBuilder(256);

            foreach (int fid in type.Catalog[int.Parse(e.sGubun)])
                sb.Append(axAPI.GetChejanData(fid)).Append(';');

            if (e.sGubun.Equals("4"))
            {
                string[] param = sb.ToString().Split(';');
                SendConfirm?.Invoke(this, new Identify(string.Concat(" holds ", param[9].Equals("1") ? "Sell " : "Buy ", param[4], " Contracts for ", param[2], ".")));
                LookUpTheDeposit(Account);

                if (param[1].Substring(0, 3).Equals("101"))
                    Quantity = param[9].Equals("1") ? -int.Parse(param[4]) : int.Parse(param[4]);

                return;
            }
            if (e.sGubun.Equals("0"))
                OnReceiveBalance = GetConclusion(sb.ToString().Split(';'));
        }
        private void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            sb = new StringBuilder(512);

            foreach (int fid in type.Catalog[Array.FindIndex(Enum.GetNames(typeof(IRealType.RealType)), o => o.Equals(e.sRealType))])
                sb.Append(axAPI.GetCommRealData(e.sRealKey, fid)).Append(';');

            if (e.sRealType.Equals(Enum.GetName(typeof(IRealType.RealType), 1)) && e.sRealKey.Substring(0, 3).Equals("101"))
            {
                SendDatum?.Invoke(this, new Datum(sb));

                return;
            }
            if (e.sRealType.Equals(Enum.GetName(typeof(IRealType.RealType), 7)) && sb.ToString().Substring(0, 1).Equals("e") && DeadLine == false)
            {
                DeadLine = true;
                Delay.delay = 4150;
                Request(Code[0].Substring(0, 8));

                return;
            }
        }
        private void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            if (!e.sMsg.Contains("신규주문"))
            {
                SendConfirm?.Invoke(this, new Identify(e.sMsg.Substring(8)));
                OnReceiveBalance = true;
            }
        }
        private int ErrorCode
        {
            get; set;
        }
        private int Squence
        {
            get; set;
        }
        private bool DeadLine
        {
            get; set;
        } = false;
        private string ScreenNo
        {
            get
            {
                return (screen++ % 20 + 1000).ToString();
            }
        }
        private string Account
        {
            get; set;
        }
        private ConnectAPI()
        {
            Code = new Dictionary<int, string>();
            request = Delay.GetInstance(205);
            request.Run();
        }
        private int screen;
        private IRealType type;
        private StringBuilder sb;
        private AxKHOpenAPI axAPI;
        private readonly Delay request;
        private static ConnectAPI api;
        public event EventHandler<Deposit> SendDeposit;
        public event EventHandler<Datum> SendDatum;
        public event EventHandler<Account> SendAccount;
        public event EventHandler<Holding> SendHolding;
        public event EventHandler<Identify> SendConfirm;
        public event EventHandler<Memorize> SendMemorize;
    }
}