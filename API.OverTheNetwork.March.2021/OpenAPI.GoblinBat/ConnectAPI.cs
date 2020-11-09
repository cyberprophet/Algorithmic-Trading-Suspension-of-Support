using System;
using System.Collections.Generic;
using System.Linq;
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
        const string transaction_suspension = "거래정지";
        Stack<string> CatalogStocksCode(IEnumerable<string> market)
        {
            int index = 0;
            var sb = new StringBuilder(0x100);
            var stack = new Stack<string>(0x10);

            foreach (var str in market)
                if (string.IsNullOrEmpty(str) == false && axAPI.GetMasterStockState(str).Contains(transaction_suspension) == false)
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
            string exclusion, date = API?.DistinctDate;
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

                                if (state.Contains(transaction_suspension))
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
            foreach (var code in list.OrderBy(o => o))
                yield return new Tuple<string, string>("Opt50001", code);
        }
        void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e) => BeginInvoke(new Action(() =>
        {
            if (e.nErrCode == 0)
            {
                foreach (var code in GetInformationOfCode(new List<string> { axAPI.GetFutureCodeByIndex(0) }, axAPI.GetCodeListByMarket(string.Empty).Split(';')))
                    Send?.Invoke(this, new SendSecuritiesAPI(code));

                Send?.Invoke(this, new SendSecuritiesAPI(GetType().Name, axAPI.GetLoginInfo("ACCLIST").Split(';')));
            }
            else
                Send?.Invoke(this, new SendSecuritiesAPI(API?.SendErrorMessage(e.nErrCode)));
        }));
        void OnReceiveMessage(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e) => BeginInvoke(new Action(() => Send?.Invoke(this, new SendSecuritiesAPI(string.Concat("[", e.sRQName, "] ", e.sMsg.Substring(9), "(", e.sScrNo, ")")))));
        public ConnectAPI() => InitializeComponent();
        public dynamic API
        {
            get; private set;
        }
        public bool Start
        {
            get; private set;
        }
        public ISendSecuritiesAPI<SendSecuritiesAPI> InputValueRqData(string name, string param)
        {
            TR ctor = null;

            return ctor;
        }
        public void StartProgress()
        {
            Start = true;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveMsg += OnReceiveMessage;
            API = Connect.GetInstance(axAPI);
        }
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}