using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AxKHOpenAPILib;

using ShareInvest.Analysis;
using ShareInvest.Catalog;
using ShareInvest.Catalog.OpenAPI;
using ShareInvest.Client;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;
using ShareInvest.OpenAPI.Catalog;

namespace ShareInvest.OpenAPI
{
    class Connect : ISendSecuritiesAPI<SendSecuritiesAPI>
    {
        internal void SendOrder(SendOrder o) => request.RequestTrData(new Task(() => SendErrorMessage(axAPI.SendOrder(o.RQName, o.ScreenNo, o.AccNo, o.OrderType, o.Code, o.Qty, o.Price, o.HogaGb, o.OrgOrderNo))));
        internal void InputValueRqData(TR param) => request.RequestTrData(new Task(() =>
        {
            string[] count = param.ID.Split(';'), value = param.Value.Split(';');
            int i, l = count.Length;

            for (i = 0; i < l; i++)
                axAPI.SetInputValue(count[i], value[i]);

            SendErrorMessage(axAPI.CommRqData(param.RQName, param.TrCode, param.PrevNext, param.ScreenNo));
        }));
        internal IEnumerable<string> GetInformationOfCode(List<string> list, string[] market)
        {
            string exclusion, date = DistinctDate;
            Delay.Milliseconds = 0x259;

            for (int i = 2; i < 4; i++)
                foreach (var om in axAPI.GetActPriceList().Split(';'))
                {
                    exclusion = axAPI.GetOptionCode(om.Insert(3, "."), i, date);

                    if (list.Exists(o => o.Equals(exclusion)))
                        continue;

                    list.Add(exclusion);
                }
            Parallel.ForEach(Enum.GetNames(typeof(Market)), new ParallelOptions { MaxDegreeOfParallelism = (int)(Environment.ProcessorCount * 0.5) }, new Action<string>(async (sMarket) =>
            {
                if (Enum.TryParse(sMarket, out Market param))
                    switch (param)
                    {
                        case Market.장내:
                        case Market.코스닥:
                        case Market.ETF:
                            for (int i = 0; i < market.Length; i++)
                            {
                                var state = axAPI.GetMasterStockState(market[i]);

                                if (state.Contains("거래정지") && await GoblinBatClient.GetInstance().PutContext<Codes>(new Codes
                                {
                                    Code = market[i],
                                    Name = axAPI.GetMasterCodeName(market[i]),
                                    MaturityMarketCap = state,
                                    Price = axAPI.GetMasterLastPrice(market[i])
                                }) < int.MaxValue)
                                    market[i] = string.Empty;
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
            }));
            var stack = CatalogStocksCode(market.OrderByDescending(o => o));
            list[1] = axAPI.GetFutureCodeByIndex(0x18);

            while (stack.Count > 0)
                yield return stack.Pop();

            foreach (var code in list)
                yield return code;
        }
        internal void InputValueRqData(int nCodeCount, TR param) => request.RequestTrData(new Task(() => SendErrorMessage(axAPI.CommKwRqData(param.Value, 0, nCodeCount, param.PrevNext, param.RQName, param.ScreenNo))));
        internal void SendErrorMessage(int error)
        {
            if (error < 0 && new Error().Message.TryGetValue(error, out string param))
            {
                Send?.Invoke(this, new SendSecuritiesAPI(param));

                switch (error)
                {
                    case -106:
                        Send?.Invoke(this, new SendSecuritiesAPI((short)error));
                        break;
                }
            }
        }
        internal string LookupScreenNo
        {
            get
            {
                if (Count++ == 0x95)
                    Count = 0;

                return (0xBB8 + Count).ToString("D4");
            }
        }
        Stack<string> CatalogStocksCode(IEnumerable<string> market)
        {
            int index = 0;
            var sb = new StringBuilder(0x100);
            var stack = new Stack<string>(0x10);

            foreach (var str in market)
                if (string.IsNullOrEmpty(str) == false && axAPI.GetMasterStockState(str).Contains("거래정지") == false)
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
        string DistinctDate
        {
            get
            {
                DayOfWeek dt = DateTime.Now.AddDays(1 - DateTime.Now.Day).DayOfWeek;
                int check = dt.Equals(DayOfWeek.Friday) || dt.Equals(DayOfWeek.Saturday) ? 3 : 2, usWeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1;

                return usWeekNumber > check || usWeekNumber == check && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday)) ? DateTime.Now.AddMonths(1).ToString(distinctDate) : DateTime.Now.ToString(distinctDate);
            }
        }
        uint Count
        {
            get; set;
        }
        Connect(AxKHOpenAPI axAPI)
        {
            this.axAPI = axAPI;
            request = Delay.GetInstance(0xCD);
            request.Run();
            Chapter = new 장시작시간 { API = axAPI };
        }
        static Connect API
        {
            get; set;
        }
        internal static ulong Cash
        {
            get; set;
        }
        internal static HashSet<TR> TR
        {
            get; private set;
        }
        internal static HashSet<Real> Real
        {
            get; private set;
        }
        internal static Dictionary<string, Chejan> Chejan
        {
            get; private set;
        }
        internal static Dictionary<string, Holding> HoldingStock
        {
            get; private set;
        }
        internal static Connect GetInstance(AxKHOpenAPI axAPI)
        {
            if (API == null && axAPI.CommConnect() == 0)
            {
                API = new Connect(axAPI);
                HoldingStock = new Dictionary<string, Holding>();
                TR = new HashSet<TR>()
                {
                    new KOA_CREATE_FO_ORD { API = axAPI },
                    new KOA_NORMAL_FO_CANCEL { API = axAPI },
                    new KOA_NORMAL_FO_MOD { API = axAPI },
                    new KOA_NORMAL_SELL_KP_ORD { API = axAPI },
                    new KOA_NORMAL_SELL_KQ_ORD { API = axAPI },
                    new KOA_NORMAL_BUY_KP_ORD { API = axAPI },
                    new KOA_NORMAL_BUY_KQ_ORD { API = axAPI }
                };
                Real = new HashSet<Real>()
                {
                    new 주식체결 { API = axAPI },
                    new 주식호가잔량 { API = axAPI },
                    new 주식시세 { API = axAPI },
                    new 주식우선호가 { API = axAPI },
                    Chapter
                };
                Chejan = new Dictionary<string, Chejan>()
                {
                    {
                        ((int)ChejanType.주문체결).ToString(),
                        new 주문체결 { API = axAPI }
                    },
                    {
                        ((int)ChejanType.잔고).ToString(),
                        new 잔고 { API = axAPI } },
                    {
                        ((int)ChejanType.파생잔고).ToString(),
                        new 파생잔고 { API = axAPI }
                    }
                };
            }
            return API;
        }
        internal static 장시작시간 Chapter
        {
            get; set;
        }
        const string distinctDate = "yyyyMM";
        readonly Delay request;
        readonly AxKHOpenAPI axAPI;
        public event EventHandler<SendSecuritiesAPI> Send;
    }
    enum HogaGb
    {
        지정가 = 00,
        시장가 = 03,
        조건부지정가 = 05,
        최유리지정가 = 06,
        최우선지정가 = 07,
        지정가IOC = 10,
        시장가IOC = 13,
        최유리IOC = 16,
        지정가FOK = 20,
        시장가FOK = 23,
        최유리FOK = 26,
        장전시간외종가 = 61,
        시간외단일가매매 = 62,
        장후시간외종가 = 81
    }
}