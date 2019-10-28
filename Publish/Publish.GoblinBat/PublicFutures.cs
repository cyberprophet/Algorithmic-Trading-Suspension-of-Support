using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxKHOpenAPILib;
using ShareInvest.AutoMessageBox;
using ShareInvest.Catalog;
using ShareInvest.Communicate;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;

namespace ShareInvest.Publish
{
    public class PublicFutures
    {
        public static PublicFutures Get()
        {
            if (api == null)
                api = new PublicFutures();

            return api;
        }
        public bool OnReceiveBalance
        {
            get; set;
        } = true;
        public int Quantity
        {
            get; private set;
        }
        public string Code
        {
            get; private set;
        }
        public string Retention
        {
            private get; set;
        }
        public string Remaining
        {
            get; private set;
        }
        public double PurchasePrice
        {
            get; private set;
        }
        public void SetAPI(AxKHOpenAPI axAPI)
        {
            this.axAPI = axAPI;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveTrData += OnReceiveTrData;
            axAPI.OnReceiveRealData += OnReceiveRealData;
            axAPI.OnReceiveChejanData += OnReceiveChejanData;
            axAPI.OnReceiveMsg += OnReceiveMsg;
        }
        public void StartProgress(IConfirm confirm, IStrategy st)
        {
            if (axAPI != null)
            {
                this.confirm = confirm;
                this.st = st;
                ErrorCode = axAPI.CommConnect();

                if (st.Type > 0)
                    ErrorCode = st.Type;

                return;
            }
            Box.Show("API Not Found. . .", "Caution", waiting);
            SendExit?.Invoke(this, new ForceQuit(end));
        }
        public void OnReceiveOrder(IOrderMethod om)
        {
            request.RequestTrData(new Task(() =>
            {
                if (ConfirmOrder.Get().CheckCurrent())
                    ErrorCode = axAPI.SendOrderFO("GoblinBat", ScreenNo, Account, Code, 1, om.SlbyTP, om.OrdTp, 1, om.Price, "");

                if (ErrorCode != 0)
                    new Error(ErrorCode);
            }));
        }
        public void RemainingDay()
        {
            request.RequestTrData(new Task(() => InputValueRqData(new Opt50001 { Value = Code })));
        }
        private void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            if (!e.sMsg.Contains("신규주문"))
            {
                SendConfirm?.Invoke(this, new Identify(e.sMsg.Substring(8)));
                OnReceiveBalance = true;
            }
        }
        private void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            sb = new StringBuilder(256);

            if (e.sGubun.Equals("0"))
            {
                foreach (int fid in new 주문체결())
                    sb.Append(axAPI.GetChejanData(fid)).Append(',');

                string[] arr = sb.ToString().Split(',');

                if (!arr[18].Equals(string.Empty))
                {
                    Box.Show(string.Concat("Conclusion ", arr[17].Contains("-") ? string.Concat("Sell ", arr[17].Substring(1)) : string.Concat("Buy ", arr[17]), "\n", "Commission ￦", (int.Parse(arr[18]) * st.TransactionMultiplier * st.Commission * double.Parse(arr[17].Contains("-") ? arr[17].Substring(1) : arr[17])).ToString("N0")), DateTime.ParseExact(arr[15], "HHmmss", null).ToString("HH시 mm분 ss초"), waiting / 5);
                    OnReceiveBalance = true;
                }
                return;
            }
            if (e.sGubun.Equals("4"))
            {
                foreach (int fid in new 파생잔고())
                    sb.Append(axAPI.GetChejanData(fid)).Append(',');

                string[] arr = sb.ToString().Split(',');

                Quantity = arr[9].Equals("1") ? -int.Parse(arr[4]) : int.Parse(arr[4]);
                PurchasePrice = double.Parse(arr[5].Contains("-") ? arr[5].Substring(1) : arr[5]);
                SendConfirm?.Invoke(this, new Identify(confirm, st, string.Concat(" holds ", arr[9].Equals("1") ? "Sell " : "Buy ", arr[4], " Contracts for ", arr[2], ".")));
            }
        }
        private void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            sb = new StringBuilder(512);

            if (e.sRealType.Equals("선물시세"))
            {
                foreach (int fid in new 선물시세())
                    sb.Append(axAPI.GetCommRealData(e.sRealKey, fid)).Append(',');

                Send?.Invoke(this, new Datum(sb));

                return;
            }
            if (e.sRealType.Equals("선물호가잔량"))
            {
                foreach (int fid in new 선물호가잔량())
                    sb.Append(axAPI.GetCommRealData(e.sRealKey, fid)).Append(',');

                string[] fg = sb.ToString().Split(',');

                if (int.Parse(fg[0]) > 153459)
                {
                    if (fg[52].Contains("-"))
                        fg[52] = fg[52].Substring(1);

                    Send?.Invoke(this, new Datum(false, fg[0], double.Parse(fg[52])));
                }
                return;
            }
            if (e.sRealType.Equals("장시작시간"))
            {
                foreach (int fid in new 장시작시간())
                    sb.Append(axAPI.GetCommRealData(e.sRealKey, fid)).Append(',');

                string[] tg = sb.ToString().Split(',');

                if (DeadLine == false && tg[0].Equals("e"))
                {
                    DeadLine = true;
                    Request();
                }
            }
        }
        private void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
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

                        if (ts[x, y].Length > 13 && !e.sTrCode.Equals("opt50001") && Retention.Equals(ts[x, y].Substring(2)))
                        {
                            sb = new StringBuilder(it);
                            e.sPrevNext = "0";

                            break;
                        }
                        sb.Append(ts[x, y]);

                        if (y != ly)
                            sb.Append(",");
                    }
                    if (!e.sTrCode.Equals("opt50001") && sb.ToString() != it)
                    {
                        SendMemorize?.Invoke(this, new Memorize(sb));

                        continue;
                    }
                    if (sb.ToString() == it)
                        break;

                    if (e.sTrCode.Equals("opt50001"))
                    {
                        Remaining = axAPI.GetCommData(e.sTrCode, e.sRQName, 0, "잔존일수").Trim();

                        return;
                    }
                }
                if (e.sPrevNext.Equals("2") && !e.sTrCode.Equals("opt50001"))
                {
                    request.RequestTrData(new Task(() => InputValueRqData(new Opt50028 { Value = Code, RQName = Code + Retention, PrevNext = 2 })));

                    return;
                }
                if (e.sPrevNext.Equals("0") && !e.sTrCode.Equals("opt50001"))
                    SendMemorize?.Invoke(this, new Memorize(e.sPrevNext));
            }
        }
        private void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (!axAPI.GetLoginInfo("GetServerGubun").Equals("1") && e.nErrCode == 0 && confirm.Identify(axAPI.GetLoginInfo("USER_ID"), axAPI.GetLoginInfo("USER_NAME")))
            {
                Account = axAPI.GetLoginInfo("ACCLIST");
                Code = ErrorCode == 0 ? axAPI.GetFutureCodeByIndex(e.nErrCode) : axAPI.GetFutureCodeByIndex(ErrorCode);
                axAPI.KOA_Functions("ShowAccountWindow", "");
                RemainingDay();

                if (DialogResult.OK == MessageBox.Show("Do You Want to Retrieve Recent Data?", "Notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                {
                    Delay.delay = 4150;
                    Request();
                }
                SendConfirm?.Invoke(this, new Identify("The Remaining Validity Period is ", Remaining));

                return;
            }
            Box.Show("등록되지 않은 사용자이거나 모의투자로 접속하셨습니다.\n\n프로그램을 종료합니다.", "오류", waiting);
            SendExit?.Invoke(this, new ForceQuit(end));
        }
        private void InputValueRqData(TR param)
        {
            string[] count = param.ID.Split(';'), value = param.Value.Split(';');
            int i, l = count.Length;

            for (i = 0; i < l; i++)
                axAPI.SetInputValue(count[i], value[i]);

            ErrorCode = axAPI.CommRqData(param.RQName, param.TrCode, param.PrevNext, param.ScreenNo);

            if (ErrorCode != 0)
                new Error(ErrorCode);
        }
        private void Request()
        {
            request.RequestTrData(new Task(() => InputValueRqData(new Opt50028 { Value = Code, RQName = Code + Retention, PrevNext = 0 })));
        }
        private PublicFutures()
        {
            request = Delay.GetInstance(delay);
            request.Run();
        }
        private bool DeadLine
        {
            get; set;
        } = false;
        private string Account
        {
            get
            {
                return account;
            }
            set
            {
                string[] acc = value.Split(';');

                foreach (string val in acc)
                    if (val.Length > 0 && val.Substring(8, 2).Equals("31"))
                        account = val;
            }
        }
        private string ScreenNo
        {
            get
            {
                return (screen++ % 20 + 1000).ToString();
            }
        }
        private int ErrorCode
        {
            get; set;
        }
        private IConfirm confirm;
        private IStrategy st;
        private AxKHOpenAPI axAPI;
        private StringBuilder sb;
        private int screen;
        private string account;
        private const string it = "Information that already Exists";
        private const int waiting = 3500;
        private const int delay = 205;
        private const int end = 1;
        private static PublicFutures api;
        private readonly Delay request;
        public event EventHandler<Memorize> SendMemorize;
        public event EventHandler<ForceQuit> SendExit;
        public event EventHandler<Identify> SendConfirm;
        public event EventHandler<Datum> Send;
    }
}