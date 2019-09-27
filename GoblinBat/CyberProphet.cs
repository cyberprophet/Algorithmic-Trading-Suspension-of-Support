using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using AxKHOpenAPILib;
using ShareInvest.AutoMessageBox;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Information;
using ShareInvest.Screen;
using ShareInvest.Storage;

namespace ShareInvest.GoblinBat
{
    public class CyberProphet : Conceal
    {
        private AxKHOpenAPI axAPI;
        private TR tr;
        private Task rq;

        private static CyberProphet cp;

        private readonly Delay request;
        private readonly IEnumerable[] catalog =
        {
            new 선물시세(),
            new 선물옵션우선호가(),
            new 선물옵션합계(),
            new 선물이론가(),
            new 선물호가잔량(),
            new 주문체결(),
            new 파생잔고(),
            new 파생실시간상하한(),
            new 장시작시간(),
            new 순간체결량(),
            new 시간외종목정보(),
            new 업종등락(),
            new 업종지수(),
            new 옵션시세(),
            new 옵션이론가(),
            new 옵션호가잔량(),
            new 임의연장정보(),
            new 잔고(),
            new 종목프로그램매매(),
            new 주식거래원(),
            new 주식당일거래원(),
            new 주식시간외호가(),
            new 주식시세(),
            new 주식예상체결(),
            new 주식우선호가(),
            new 주식종목정보(),
            new 주식체결(),
            new 주식호가잔량(),
            new 투자자별매매(),
            new ELW_이론가(),
            new ELW_지표(),
            new ETF_NAV()
        };
        public event EventHandler<DayEvent> SendDay;
        public event EventHandler<TickEvent> SendTick;
        public event EventHandler<Error> SendError;
        public event EventHandler<ForceQuit> SendExit;
        public event EventHandler<BalanceEvent> SendBalance;
        public event EventHandler<ConclusionEvent> SendConclusion;
        public event EventHandler<MemorizeEvent> SendMemorize;

        private CyberProphet()
        {
            request = Delay.GetInstance(delay);
            request.Run();
        }
        public static CyberProphet Get()
        {
            if (cp == null)
                cp = new CyberProphet();

            return cp;
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
        public void StartProgress()
        {
            if (axAPI != null)
            {
                Error_code = axAPI.CommConnect();

                if (Error_code != 0)
                {
                    SendError?.Invoke(this, new Error(Error_code));

                    Box.Show("Connection Failed!!", "Caution", waiting);

                    SendExit?.Invoke(this, new ForceQuit(end));
                }
                return;
            }
            Box.Show("API Not Found!!", "Caution", waiting);

            SendExit?.Invoke(this, new ForceQuit(end));
        }
        public void RemainingDay()
        {
            rq = new Task(() =>
            {
                tr = new Opt50001
                {
                    Value = Code
                };
                InputValueRqData(tr);
            });
            request.RequestTrData(rq);
        }
        public void OnReceiveOrder(Elements e)
        {
            rq = new Task(() =>
            {
                Error_code = axAPI.SendOrderFO(e.sRQName, e.screen, Account, Code, 1, e.classification, "3", Math.Abs(e.quantity), "", "");
            });
            request.RequestTrData(rq);

            if (Error_code != 0)
                SendError?.Invoke(this, new Error(Error_code));
        }
        private void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            if (!e.sMsg.Contains("신규주문") && !e.sMsg.Contains("지연"))
            {
                Box.Show(e.sMsg.Substring(8), "Caution", 895);

                string[] arr = e.sRQName.Split(';');

                OnReceiveOrder(new Elements(Number.GetScreen(), arr[2], arr[3], arr[0], arr[1]));
            }
            SendError?.Invoke(this, new Error(e.sTrCode, e.sRQName, e.sScrNo, e.sMsg));
        }
        private void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            sb = new StringBuilder(256);

            foreach (int fid in e.sGubun.Equals("0") ? catalog[5] : catalog[6])
                sb.Append(axAPI.GetChejanData(fid)).Append(',');

            if (e.sGubun.Equals("0"))
            {
                SendConclusion?.Invoke(this, new ConclusionEvent(sb));

                return;
            }
            if (e.sGubun.Equals("4"))
                SendBalance?.Invoke(this, new BalanceEvent(sb));
        }
        private void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            sb = new StringBuilder(512);

            foreach (int fid in Array.Find(catalog, o => o.ToString().Contains(e.sRealType)))
                sb.Append(axAPI.GetCommRealData(e.sRealKey, fid)).Append(',');

            if (e.sRealType.Equals(sRealType[0]))
            {
                SendTick?.Invoke(this, new TickEvent(sb));
                SendDay?.Invoke(this, new DayEvent(sb));

                return;
            }
            if (e.sRealType.Equals(sRealType[1]) && int.Parse(sb.ToString().Substring(0, 6)) > 153459)
            {
                string[] fg = sb.ToString().Split(',');

                if (fg[52].Contains("-"))
                    fg[52] = fg[52].Substring(1);

                double price = double.Parse(fg[52]);

                SendTick?.Invoke(this, new TickEvent(false, fg[0], price, 0));
                SendDay?.Invoke(this, new DayEvent(false, price));

                return;
            }
            if (e.sRealType.Equals(sRealType[2]))
            {
                string[] tg = sb.ToString().Split(',');

                if (tg[0].Equals("e") && Deadline == false)
                {
                    Deadline = true;

                    Request();

                    SendExit?.Invoke(this, new ForceQuit(0));
                }
                return;
            }
            sb = null;
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
                        SendMemorize?.Invoke(this, new MemorizeEvent(sb));

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
                    rq = new Task(() =>
                    {
                        tr = new Opt50028
                        {
                            Value = Code,
                            RQName = Code + Retention,
                            PrevNext = 2
                        };
                        InputValueRqData(tr);
                    });
                    request.RequestTrData(rq);

                    return;
                }
                if (e.sPrevNext.Equals("0") && !e.sTrCode.Equals("opt50001"))
                    SendMemorize?.Invoke(this, new MemorizeEvent(e.sPrevNext));
            }
        }
        private void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0 && Identify(axAPI.GetLoginInfo("USER_ID"), axAPI.GetLoginInfo("USER_NAME")) == true)
            {
                Account = axAPI.GetLoginInfo("ACCLIST");
                Code = axAPI.GetFutureCodeByIndex(e.nErrCode);

                if (Account == null)
                {
                    Box.Show("This Account is not Registered.", "Caution", waiting);

                    SendExit?.Invoke(this, new ForceQuit(end));
                }
                string login = axAPI.GetLoginInfo("GetServerGubun");

                if (!login.Equals("1"))
                    Box.Show("It's a Real Investment.", "Caution", waiting);

                axAPI.KOA_Functions("ShowAccountWindow", "");
                RemainingDay();

                return;
            }
            Box.Show("등록되지 않은 사용자이거나\n로그인이 원활하지 않습니다.\n프로그램을 종료합니다.", "오류", waiting);

            SendExit?.Invoke(this, new ForceQuit(end));
        }
        private void InputValueRqData(TR param)
        {
            string[] count = param.ID.Split(';'), value = param.Value.Split(';');
            int i, l = count.Length;

            for (i = 0; i < l; i++)
                axAPI.SetInputValue(count[i], value[i]);

            Error_code = axAPI.CommRqData(param.RQName, param.TrCode, param.PrevNext, param.ScreenNo);

            if (Error_code != 0)
                SendError?.Invoke(this, new Error(Error_code));
        }
        private void Request()
        {
            rq = new Task(() =>
            {
                tr = new Opt50028
                {
                    Value = Code,
                    RQName = Code + Retention,
                    PrevNext = 0
                };
                InputValueRqData(tr);
            });
            request.RequestTrData(rq);
        }
    }
}