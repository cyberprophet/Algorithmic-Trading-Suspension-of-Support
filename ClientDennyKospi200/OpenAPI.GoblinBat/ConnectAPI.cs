using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxKHOpenAPILib;
using ShareInvest.Catalog;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Interface;

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

                return;
            }
            Environment.Exit(0);
        }
        public void RemainingDay(string code)
        {
            request.RequestTrData(new Task(() => InputValueRqData(new Opt50001 { Value = code, RQName = code })));
        }
        public Dictionary<int, string> Code
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
                    if (Code[kv.Key + 1] != null)
                        request.RequestTrData(new Task(() => InputValueRqData(new Opt50066 { Value = Code[kv.Key + 1].Substring(0, 8), RQName = string.Concat(Code[kv.Key + 1].Substring(0, 8), Retention(Code[kv.Key + 1].Substring(0, 8))).Substring(0, 20), PrevNext = 0 })));

                    break;
                }
        }
        private void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            string exclusion, date = GetDistinctDate(CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);
            List<string> code = new List<string>
            {
                axAPI.GetFutureCodeByIndex(e.nErrCode)
            };
            for (int i = 2; i < 4; i++)
                foreach (var om in axAPI.GetActPriceList().Split(';'))
                {
                    exclusion = axAPI.GetOptionCode(om.Insert(3, "."), i, date);

                    if (code.Exists(o => o.Equals(exclusion)))
                        continue;

                    code.Add(exclusion);
                }
            code.RemoveAt(1);
            Delay.delay = 615;

            foreach (string output in code)
                RemainingDay(output);

            SendAccount?.Invoke(this, new Account(axAPI.GetLoginInfo("ACCLIST")));
            axAPI.KOA_Functions("ShowAccountWindow", "");

            if (DialogResult.Yes == MessageBox.Show("Do You Want to Retrieve Recent Data?\n\nPress 'YES' after 40 Seconds to Receive Data.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                Delay.delay = 4150;
                Request(code[0]);

                return;
            }
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

            foreach (string item in Array.Find(catalog, o => o.ToString().Contains(e.sTrCode.Substring(1))))
                sb.Append(axAPI.GetCommData(e.sTrCode, e.sRQName, 0, item).Trim()).Append(';');

            FixUp(sb, e.sRQName);
        }
        private void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            sb = new StringBuilder(256);

            foreach (int fid in type.Catalog[int.Parse(e.sGubun)])
                sb.Append(axAPI.GetChejanData(fid)).Append(';');
        }
        private void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            sb = new StringBuilder(512);

            foreach (int fid in type.Catalog[Array.FindIndex(Enum.GetNames(typeof(IRealType.RealType)), o => o.Equals(e.sRealType))])
                sb.Append(axAPI.GetCommRealData(e.sRealKey, fid)).Append(';');

            if (e.sRealKey.Substring(0, 3).Equals("101"))
            {
                SendDatum?.Invoke(this, new Datum(sb));

                return;
            }
        }
        private void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
        }
        private int ErrorCode
        {
            get; set;
        }
        private int Squence
        {
            get; set;
        }
        private ConnectAPI()
        {
            Code = new Dictionary<int, string>();
            request = Delay.GetInstance(205);
            request.Run();
        }
        private IRealType type;
        private StringBuilder sb;
        private AxKHOpenAPI axAPI;
        private readonly Delay request;
        private static ConnectAPI api;
        public event EventHandler<Datum> SendDatum;
        public event EventHandler<Account> SendAccount;
        public event EventHandler<Memorize> SendMemorize;
    }
}