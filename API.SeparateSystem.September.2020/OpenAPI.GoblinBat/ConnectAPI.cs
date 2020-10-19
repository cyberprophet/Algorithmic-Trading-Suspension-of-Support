using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AxKHOpenAPILib;

using ShareInvest.Analysis;
using ShareInvest.Analysis.OpenAPI;
using ShareInvest.Catalog;
using ShareInvest.Catalog.OpenAPI;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Interface.OpenAPI;
using ShareInvest.OpenAPI.Catalog;

namespace ShareInvest.OpenAPI
{
    public sealed partial class ConnectAPI : UserControl, ISecuritiesAPI<SendSecuritiesAPI>
    {
        void ButtonStartProgressClick(object sender, EventArgs e) => BeginInvoke(new Action(() =>
        {
            Start = true;
            API = Connect.GetInstance(axAPI);
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveMsg += OnReceiveMsg;
        }));
        void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e) => BeginInvoke(new Action(() =>
        {
            if (e.nErrCode == 0 && (string.IsNullOrEmpty(privacy.SecurityAPI) == false || string.IsNullOrEmpty(axAPI.KOA_Functions(showAccountWindow, string.Empty))))
                Send?.Invoke(this, new SendSecuritiesAPI(FormWindowState.Minimized, securites != null && securites.Length == 0xA ? new Accounts(securites) : new Accounts(axAPI.GetLoginInfo(account))));

            else
                (API as Connect)?.SendErrorMessage(e.nErrCode);
        }));
        void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            if (Connect.Chejan.TryGetValue(e.sGubun, out Chejan chejan))
                chejan.OnReceiveChejanData(e);
        }
        void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e) => Connect.TR.FirstOrDefault(o => (o.RQName != null ? o.RQName.Equals(e.sRQName) : o.PrevNext.ToString().Equals(e.sPrevNext)) && o.GetType().Name.Substring(1).Equals(e.sTrCode.Substring(1)))?.OnReceiveTrData(e);
        void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e) => Connect.Real.FirstOrDefault(o => o.GetType().Name.Replace('_', ' ').Equals(e.sRealType))?.OnReceiveRealData(e);
        void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e) => Send?.BeginInvoke(this, new SendSecuritiesAPI(string.Concat("[", e.sRQName, "] ", e.sMsg.Substring(9), "(", e.sScrNo, ")")), null, null);
        void OnReceiveConditionVersion(object sender, _DKHOpenAPIEvents_OnReceiveConditionVerEvent e)
        {
            if (e.lRet == 1)
                foreach (var str in axAPI.GetConditionNameList().Split(';'))
                    if (string.IsNullOrEmpty(str) == false)
                    {
                        var param = str.Split('^');

                        if (int.TryParse(param[0], out int index))
                            Connect.Conditions[index] = param[1];
                    }
            SendMessage(e.sMsg, e.lRet.ToString("N0"));
        }
        void OnReceiveRealConditions(object sender, _DKHOpenAPIEvents_OnReceiveRealConditionEvent e)
        {
            if (e.strType.Equals("I") && Strategics.Any(o => o.Code.Equals(e.sTrCode)) == false && Connect.HoldingStock.ContainsKey(e.sTrCode) == false)
            {
                new Task(() => Console.WriteLine(e.sTrCode)).Start();
            }
        }
        void OnReceiveTrConditions(object sender, _DKHOpenAPIEvents_OnReceiveTrConditionEvent e)
        {
            foreach (var code in e.strCodeList.Split(';'))
                if (Array.Exists(Codes, o => o.Equals(code)) == false && string.IsNullOrEmpty(code) == false)
                {

                    Console.WriteLine(code);
                }
        }
        [Conditional("DEBUG")]
        void SendMessage(string code, string message) => Console.WriteLine(code + "\t" + message);
        TR GetRequestTR(string name) => Connect.TR.FirstOrDefault(o => o.GetType().Name.Equals(name)) ?? null;
        public IAccountInformation SetPrivacy(IAccountInformation privacy)
        {
            if (Connect.TR.Add(new OPT50010
            {
                PrevNext = 0,
                API = axAPI
            }))
            {
                bool response, message;

                if (privacy.AccountNumber.Substring(privacy.AccountNumber.Length - 2).Equals("31"))
                {
                    response = Connect.TR.Add(new OPW20007
                    {
                        Value = string.Concat(privacy.AccountNumber, password),
                        PrevNext = 0,
                        API = axAPI
                    });
                    message = Connect.TR.Add(new OPW20010
                    {
                        Value = string.Concat(privacy.AccountNumber, password),
                        PrevNext = 0,
                        API = axAPI
                    });
                }
                else
                {
                    response = Connect.TR.Add(new Opw00005
                    {
                        Value = string.Concat(privacy.AccountNumber, password),
                        PrevNext = 0,
                        API = axAPI
                    });
                    message = axAPI.GetLoginInfo(server).Equals(mock);
                }
                SendMessage(response.ToString(), message.ToString());
                axAPI.OnReceiveTrData += OnReceiveTrData;
                axAPI.OnReceiveRealData += OnReceiveRealData;
                axAPI.OnReceiveChejanData += OnReceiveChejanData;
                axAPI.OnReceiveConditionVer += OnReceiveConditionVersion;
            }
            string mServer = axAPI.GetLoginInfo(server), log = axAPI.GetLoginInfo(name);
            Invoke(new Action(async () =>
            {
                if (checkAccount.Checked && await new Security().Encrypt(this.privacy, privacy.AccountNumber, checkAccount.Checked) < int.MaxValue)
                    Console.WriteLine(log);
            }));
            Real.Account = privacy.AccountNumber;
            var aInfo = new AccountInformation
            {
                Identity = axAPI.GetLoginInfo(user),
                Account = privacy.AccountNumber,
                Name = string.Empty,
                Server = mServer.Equals(mock),
                Nick = log
            };
            switch (privacy.AccountNumber.Substring(privacy.AccountNumber.Length - 2))
            {
                case "31":
                    aInfo.Name = "선물옵션";
                    break;

                default:
                    if (axAPI.GetConditionLoad() == 1)
                    {
                        Connect.Conditions = new Dictionary<int, string>();
                        SendMessage(log, mServer);
                    }
                    aInfo.Name = "위탁종합";
                    break;
            }
            return aInfo;
        }
        public IEnumerable<string> InputValueRqData()
        {
            foreach (var code in (API as Connect)?.GetInformationOfCode(new List<string> { axAPI.GetFutureCodeByIndex(0) }, axAPI.GetCodeListByMarket(string.Empty).Split(';')))
                yield return code;
        }
        public ISendSecuritiesAPI<SendSecuritiesAPI> InputValueRqData(string name, string param)
        {
            TR ctor;

            if (name.Length > 0x1F)
            {
                ctor = Assembly.GetExecutingAssembly().CreateInstance(name) as TR;
                ctor.API = axAPI;
                var api = API as Connect;

                if (Connect.TR.Add(ctor) && Enum.TryParse(name.Substring(28), out CatalogTR tr))
                    switch (tr)
                    {
                        case CatalogTR.Opt10079:
                            if (param.Length == 0x16)
                                ctor.RQName = param.Substring(7, 12);

                            ctor.Value = param.Substring(0, 6);
                            api.InputValueRqData(ctor);
                            break;

                        case CatalogTR.Opt50001:
                            ctor.Value = param;
                            ctor.RQName = param;
                            api.InputValueRqData(ctor);
                            Count++;
                            break;

                        case CatalogTR.Opt50028:
                        case CatalogTR.Opt50066:
                            if (param.Length == 0x18)
                                ctor.RQName = param.Substring(9, 12);

                            ctor.Value = param.Substring(0, 8);
                            api.InputValueRqData(ctor);
                            break;

                        case CatalogTR.OPTKWFID:
                            ctor.Value = param;
                            api.InputValueRqData(param.Split(';').Length, ctor);
                            Count++;
                            break;

                        case CatalogTR.Opt10081:
                            var str = param.Substring(7);
                            ctor.RQName = str;
                            ctor.Value = string.Concat(param.Substring(0, 6), ';', str);
                            api.InputValueRqData(ctor);
                            Count++;
                            break;
                    }
            }
            else
            {
                ctor = Connect.TR.FirstOrDefault(o => o.GetType().Name.Substring(1).Equals(name.Substring(1)) && (o.RQName.Contains(param) || o.Value.Contains(param)));

                if (Connect.TR.Remove(ctor))
                    SendMessage(param, name);
            }
            return ctor ?? null;
        }
        public ISendSecuritiesAPI<SendSecuritiesAPI> InputValueRqData(bool input, string name)
        {
            var ctor = GetRequestTR(name);

            if (input)
                BeginInvoke(new Action(() => (API as Connect)?.InputValueRqData(ctor)));

            return ctor ?? null;
        }
        public void SetConditions(string[] codes)
        {
            Codes = codes;
            axAPI.OnReceiveTrCondition += OnReceiveTrConditions;
            axAPI.OnReceiveRealCondition += OnReceiveRealConditions;
            var count = 0x56E;

            foreach (var kv in Connect.Conditions)
                if (kv.Value.StartsWith("SC_"))
                    (API as Connect)?.SendCondition(count++.ToString("D4"), kv.Value, kv.Key);
        }
        public void StartProgress() => buttonStartProgress.PerformClick();
        public void SetForeColor(Color color, string remain)
        {
            labelOpenAPI.ForeColor = color;
            labelMessage.Text = remain;
        }
        public int SetStrategics(IStrategics strategics)
        {
            switch (strategics)
            {
                case TrendsInValuation tv:
                    Connect.HoldingStock[strategics.Code] = new HoldingStocks(tv)
                    {
                        Code = strategics.Code,
                        Current = 0,
                        Purchase = 0,
                        Quantity = 0,
                        Rate = 0,
                        Revenue = 0
                    };
                    break;

                case TrendToCashflow tc:
                    Connect.HoldingStock[strategics.Code] = new HoldingStocks(tc)
                    {
                        Code = strategics.Code,
                        Current = 0,
                        Purchase = 0,
                        Quantity = 0,
                        Rate = 0,
                        Revenue = 0
                    };
                    break;

                case TrendFollowingBasicFutures tf:
                    Connect.HoldingStock[strategics.Code] = new HoldingStocks(tf)
                    {
                        Code = strategics.Code,
                        Current = 0D,
                        Purchase = 0D,
                        Quantity = 0,
                        Rate = 0,
                        Revenue = 0
                    };
                    break;

                case TrendsInStockPrices ts:
                    Connect.HoldingStock[strategics.Code] = new HoldingStocks(ts)
                    {
                        Code = strategics.Code,
                        Current = 0,
                        Purchase = 0,
                        Quantity = 0,
                        Rate = 0,
                        Revenue = 0
                    };
                    break;
            }
            return Connect.HoldingStock.Count;
        }
        public void SendOrder(IAccountInformation info, Tuple<string, int, string, string, int, string, string> order) => (API as Connect)?.SendOrder(new SendOrderFO
        {
            RQName = order.Item1,
            ScreenNo = (API as Connect)?.LookupScreenNo,
            AccNo = info.Account,
            Code = order.Item1,
            OrdKind = order.Item2,
            SlbyTp = order.Item3,
            OrdTp = order.Item4,
            Qty = order.Item5,
            Price = order.Item6,
            OrgOrdNo = order.Item7
        });
        public void SendOrder(IAccountInformation info, Tuple<int, string, int, int, string> order)
        {
            if (Connect.HoldingStock.TryGetValue(order.Item2, out Holding holding))
            {
                if (order.Item1 == 1 && Connect.Cash < order.Item4 * 2 && holding.OrderNumber.Count(o => o.Value < order.Item4) > 0)
                    (API as Connect)?.SendOrder(new SendOrder
                    {
                        RQName = axAPI.GetMasterCodeName(order.Item2),
                        ScreenNo = (API as Connect)?.LookupScreenNo,
                        AccNo = info.Account,
                        OrderType = 3,
                        Code = order.Item2,
                        Qty = order.Item3,
                        Price = order.Item4,
                        HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                        OrgOrderNo = holding.OrderNumber.OrderBy(o => o.Value).First().Key
                    });
                else if (order.Item1 == 2 && holding.Quantity > 0 && holding.Quantity - holding.OrderNumber.Count(o => o.Value > order.Item4) < 1)
                    (API as Connect)?.SendOrder(new SendOrder
                    {
                        RQName = axAPI.GetMasterCodeName(order.Item2),
                        ScreenNo = (API as Connect)?.LookupScreenNo,
                        AccNo = info.Account,
                        OrderType = 4,
                        Code = order.Item2,
                        Qty = order.Item3,
                        Price = order.Item4,
                        HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                        OrgOrderNo = holding.OrderNumber.OrderByDescending(o => o.Value).First().Key
                    });
            }
            (API as Connect)?.SendOrder(new SendOrder
            {
                RQName = axAPI.GetMasterCodeName(order.Item2),
                ScreenNo = (API as Connect)?.LookupScreenNo,
                AccNo = info.Account,
                OrderType = order.Item1,
                Code = order.Item2,
                Qty = order.Item3,
                Price = order.Item4,
                HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                OrgOrderNo = order.Item5
            });
            Connect.Cash += (order.Item1 == 1 ? -order.Item4 : order.Item4) * order.Item3;
        }
        public ISendSecuritiesAPI<SendSecuritiesAPI> ConnectChapterOperation => Connect.Chapter;
        public ISendSecuritiesAPI<SendSecuritiesAPI> OnConnectErrorMessage => API as Connect ?? null;
        public IEnumerable<Holding> HoldingStocks
        {
            get
            {
                foreach (var ctor in Connect.HoldingStock)
                    yield return ctor.Value ?? null;
            }
        }
        public dynamic API
        {
            get; private set;
        }
        public uint Count
        {
            get; set;
        }
        public bool Start
        {
            get; private set;
        }
        public HashSet<IStrategics> Strategics
        {
            get; private set;
        }
        public ConnectAPI(Privacies privacy)
        {
            this.privacy = privacy;
            InitializeComponent();

            if (string.IsNullOrEmpty(privacy.SecurityAPI) == false)
            {
                securites = new Security().Decipher(privacy.Security, privacy.SecuritiesAPI, privacy.SecurityAPI);
                checkAccount.CheckState = CheckState.Checked;
            }
            Strategics = new HashSet<IStrategics>();
        }
        string[] Codes
        {
            get; set;
        }
        readonly StringBuilder securites;
        readonly Privacies privacy;
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}