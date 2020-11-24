using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using AxKHOpenAPILib;

using ShareInvest.Catalog.Models;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI
{
    public sealed partial class ConnectAPI : UserControl, ISecuritiesAPI<SendSecuritiesAPI>
    {
        Stack<string> CatalogStocksCode(IEnumerable<string> market)
        {
            int index = 0;
            var sb = new StringBuilder(0x100);
            var stack = new Stack<string>(0x10);

            foreach (var str in market)
                if (string.IsNullOrEmpty(str) == false && axAPI.GetMasterStockState(str).Contains(Base.TransactionSuspension) == false)
                {
                    if (index++ % 0x63 == 0x62)
                    {
                        stack.Push(sb.Append(str).ToString());
                        sb = new StringBuilder();
                    }
                    sb.Append(str).Append(';');
                }
            stack.Push(sb.Remove(sb.Length - 1, 1).ToString());

            return stack;
        }
        IEnumerable<Tuple<string, string>> GetInformationOfCode(List<string> list, string[] market)
        {
            string exclusion, date = Base.DistinctDate;
            Delay.Milliseconds = 0x259;

            for (int i = 2; i < 4; i++)
                foreach (var om in axAPI.GetActPriceList().Split(';'))
                {
                    exclusion = axAPI.GetOptionCode(om.Insert(3, "."), i, date);

                    if (list.Exists(o => o.Equals(exclusion)))
                        continue;

                    list.Add(exclusion);
                }
            foreach (var stock in Enum.GetNames(typeof(Market)))
                if (Enum.TryParse(stock, out Market param))
                    switch (param)
                    {
                        case Market.장내:
                        case Market.코스닥:
                        case Market.ETF:
                            for (int i = 0; i < market.Length; i++)
                            {
                                var state = axAPI.GetMasterStockState(market[i]);

                                if (state.Contains(Base.TransactionSuspension))
                                {
                                    Send?.Invoke(this, new SendSecuritiesAPI(new Codes
                                    {
                                        Code = market[i],
                                        Name = axAPI.GetMasterCodeName(market[i]),
                                        MaturityMarketCap = state,
                                        Price = axAPI.GetMasterLastPrice(market[i])
                                    }));
                                    market[i] = string.Empty;
                                }
                            }
                            break;

                        default:
                            foreach (var str in axAPI.GetCodeListByMarket(((int)param).ToString()).Split(';'))
                            {
                                var index = Array.FindIndex(market, o => o.Equals(str));

                                if (index > -1)
                                    market[index] = string.Empty;
                            }
                            break;
                    }
            var stack = CatalogStocksCode(market.OrderBy(o => Guid.NewGuid()));
            list[1] = axAPI.GetFutureCodeByIndex(0x18);
            list.Add(axAPI.GetFutureCodeByIndex(0xD));

            while (stack.Count > 0)
                yield return new Tuple<string, string>("OPTKWFID", stack.Pop());

            foreach (var str in axAPI.GetSFutureList(string.Empty).Split('|'))
                if (string.IsNullOrEmpty(str) == false)
                {
                    var temp = str.Split('^');

                    if (temp[2].Equals(date) == false)
                        list.Add(temp[0]);

                    date = temp[2];
                }
            foreach (var code in list.OrderBy(o => Guid.NewGuid()))
                yield return new Tuple<string, string>("Opt50001", code);
        }
        void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
            => (API as Connect)?.TR.FirstOrDefault(o
                => (o.RQName != null ? o.RQName.Equals(e.sRQName) : o.PrevNext.ToString().Equals(e.sPrevNext)) && o.GetType().Name[1..].Equals(e.sTrCode[1..]))?.OnReceiveTrData(e);
        void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0)
            {
                foreach (var code in GetInformationOfCode(new List<string> { axAPI.GetFutureCodeByIndex(0) }, axAPI.GetCodeListByMarket(string.Empty).Split(';')))
                    Send?.Invoke(this, new SendSecuritiesAPI(code));

                Send?.Invoke(this, new SendSecuritiesAPI(axAPI.GetLoginInfo("ACCLIST").Split(';')));
            }
            else
                Send?.Invoke(this, new SendSecuritiesAPI(API?.SendErrorMessage(e.nErrCode)));
        }
        void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
            => (API as Connect)?.Real.FirstOrDefault(o
                => o.GetType().Name.Replace("_", string.Empty).Equals(e.sRealType, StringComparison.Ordinal))?.OnReceiveRealData(e);
        void OnReceiveMessage(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
            => Send?.Invoke(this, new SendSecuritiesAPI(string.Concat("[", e.sRQName, "] ", e.sMsg[9..], "(", e.sScrNo, ")")));
        public ConnectAPI()
        {
            InitializeComponent();
            ConnectToReceiveRealTime
                = new NamedPipeServerStream(Process.GetCurrentProcess().ProcessName.Split(' ')[2], PipeDirection.Out, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        }
        public dynamic API
        {
            get; private set;
        }
        public string Account
        {
            get; set;
        }
        public string SecuritiesName => axAPI.GetLoginInfo("USER_NAME").Trim();
        public bool Start
        {
            get; private set;
        }
        public uint Count
        {
            get; private set;
        }
        public ISendSecuritiesAPI<SendSecuritiesAPI> InputValueRqData(string name, string param)
        {
            var ctor = Assembly.GetExecutingAssembly().CreateInstance(name) as TR;
            ctor.API = axAPI;

            if (Enum.TryParse(name[0x1C..], out CatalogTR tr) && API?.TR.Add(ctor))
                switch (tr)
                {
                    case CatalogTR.Opt10079:
                        if (param.Length == 0x16)
                            ctor.RQName = param.Substring(7, 0xC);

                        ctor.Value = param.Substring(0, 6);
                        API?.InputValueRqData(ctor);
                        break;

                    case CatalogTR.Opt50001:
                        ctor.Value = param;
                        ctor.RQName = param;
                        API?.InputValueRqData(ctor);
                        Count++;
                        break;

                    case CatalogTR.Opt50028:
                    case CatalogTR.Opt50066:
                        if (param.Length == 0x18)
                            ctor.RQName = param.Substring(9, 0xC);

                        ctor.Value = param.Substring(0, 8);
                        API?.InputValueRqData(ctor);
                        break;

                    case CatalogTR.OPTKWFID:
                        ctor.Value = param;
                        API?.InputValueRqData(param.Split(';').Length, ctor);
                        Count++;
                        break;

                    case CatalogTR.Opt10081:
                        var str = param[7..];
                        ctor.RQName = str;
                        ctor.Value = string.Concat(param.Substring(0, 6), ';', str);
                        API?.InputValueRqData(ctor);
                        Count++;
                        break;

                    case CatalogTR.Opw00005:
                    case CatalogTR.OPW20007:
                    case CatalogTR.OPW20010:
                        ctor.Value = param;
                        API?.InputValueRqData(ctor);
                        break;
                }
            return ctor;
        }
        public ISendSecuritiesAPI<SendSecuritiesAPI> RemoveValueRqData(string name, string param)
        {
            var ctor = (API as Connect)?.TR.FirstOrDefault(o => o.GetType().Name.Equals(name) && (o.RQName.Contains(param) || o.Value.Contains(param)));

            if (API?.TR.Remove(ctor))
                Base.SendMessage(GetType(), param, name);

            return ctor;
        }
        public void StartProgress() => BeginInvoke(new Action(async () =>
        {
            Start = true;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveMsg += OnReceiveMessage;
            axAPI.OnReceiveTrData += OnReceiveTrData;
            axAPI.OnReceiveRealData += OnReceiveRealData;
            await ConnectToReceiveRealTime.WaitForConnectionAsync();
            Writer = new StreamWriter(ConnectToReceiveRealTime)
            {
                AutoFlush = true
            };
            API = Connect.GetInstance(axAPI, Writer);
        }));
        public void SendOrder(ISendOrder order) => API?.SendOrder(order);
        public StreamWriter Writer
        {
            get; private set;
        }
        public NamedPipeServerStream ConnectToReceiveRealTime
        {
            get;
        }
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}