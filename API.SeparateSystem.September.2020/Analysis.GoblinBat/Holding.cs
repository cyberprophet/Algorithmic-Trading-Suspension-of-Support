using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Analysis
{
    public abstract class Holding
    {
        Queue<Charts> Days
        {
            get; set;
        }
        IEnumerable<Queue<Charts>> FindTheOldestDueDate(string code)
        {
            if (code.Length == 8 && Temporary.CodeStorage != null
                && Temporary.CodeStorage.Any(o => o.Code.Length == 8 && o.Code.StartsWith(code.Substring(0, 3)) && o.Code.EndsWith(code.Substring(5))))
            {
                var stack = new Stack<Codes>();

                if (code[0].Equals('1'))
                {
                    MarginRate = Temporary.CodeStorage.First(o => o.Code.Equals(code)).MarginRate;
                    TransactionMultiplier = GetTransactionMultiplier(code);
                }
                foreach (var arg in Temporary.CodeStorage.Where(o => o.Code.StartsWith(code.Substring(0, 3)) && o.Code.EndsWith(code.Substring(5)))
                    .OrderByDescending(o => o.MaturityMarketCap.Length == 8 ? o.MaturityMarketCap.Substring(2) : o.MaturityMarketCap))
                {
                    stack.Push(arg);
                    Days = new Queue<Charts>();

                    if (uint.TryParse(arg.MaturityMarketCap.Length == 8 ? arg.MaturityMarketCap.Substring(2) : arg.MaturityMarketCap, out uint remain)
                        && Temporary.RemainingDay.Add(remain - 1))
                        Console.WriteLine(code + "_" + Temporary.RemainingDay.Count + "_" + (remain - 1));
                }
                foreach (var day in Temporary.CallUpTheChartAsync(code).Result)
                    Days.Enqueue(day);

                while (stack.Count > 0)
                {
                    var codes = stack.Pop();

                    if (codes.Code.Equals("105QA000"))
                    {
                        foreach (var arg in Temporary.CallUpTheChartAsync(Temporary.CodeStorage.First(o => o.Code.Equals("101QC000"))).Result)
                            if (arg.Date.Substring(6, 4).Equals("1545"))
                                Days.Enqueue(new Charts
                                {
                                    Date = string.Concat("20", arg.Date.Substring(0, 6)),
                                    Price = arg.Price
                                });
                        continue;
                    }
                    yield return Temporary.CallUpTheChartAsync(codes).Result;
                }
            }
            else if (code.Length == 6 && Temporary.CodeStorage != null && Temporary.CodeStorage.Any(o => o.Code.Equals(code)))
            {
                Market = Temporary.CodeStorage.First(o => o.Code.Equals(code)).MarginRate == 1;
                string sDate = Temporary.FindTheChartStartsAsync(code).Result,
                    date = string.IsNullOrEmpty(sDate) ? DateTime.Now.AddDays(-5).ToString(format) : sDate.Substring(0, 6);
                Days = new Queue<Charts>();

                foreach (var day in Temporary.CallUpTheChartAsync(code).Result)
                    if (string.Compare(day.Date.Substring(2), date) < 0)
                        Days.Enqueue(day);

                if (int.TryParse(date, out int start))
                {
                    var end = string.Empty;
                    var count = 0;

                    while (string.IsNullOrEmpty(end) || string.Compare(end, DateTime.Now.ToString(format)) <= 0)
                    {
                        if (string.IsNullOrEmpty(end) == false
                            && end.Substring(2).CompareTo(excluding.Substring(2)) > 0
                            && end.Substring(2).CompareTo(theDate.Substring(2)) < 0)
                            for (int i = 0; i < 0x1C; i++)
                                count++;

                        yield return Temporary.CallUpTheChartAsync(new Catalog.Request.Charts
                        {
                            Code = code,
                            Start = (start - 1 + 0x12C * count++).ToString(nFormat),
                            End = end = (start - 1 + 0x12C * count).ToString(nFormat)
                        }).Result;
                    }
                }
            }
        }
        protected internal long StartProgress(string code)
        {
            if (string.IsNullOrEmpty(code) == false)
            {
                Temporary = new Temporary(code.Length);
                var revise = Temporary.CallUpTheRevisedStockPrice(code).Result;
                var modify = revise != null && revise.Count > 0 ? new Catalog.Request.ConfirmRevisedStockPrice[revise.Count] : null;
                var index = 0;

                foreach (var queue in FindTheOldestDueDate(code))
                    if (queue != null && queue.Count > 0)
                    {
                        var enumerable = queue.OrderBy(o => o.Date);
                        var before = enumerable.First().Date.Substring(0, 6);

                        while (revise != null && revise.Count > 0)
                        {
                            var param = revise.Dequeue();

                            if (param.Date.CompareTo(Days.Count > 0 ? Days.Max(o => o.Date).Substring(2) : before) > 0)
                            {
                                if (revise.Count == 0)
                                {
                                    modify[index] = param;

                                    break;
                                }
                                var peek = revise.Peek();

                                if (param.Rate != peek.Rate)
                                    modify[index++] = param;
                            }
                        }
                        if (Days.Count > 0)
                        {
                            foreach (var day in Days.OrderBy(o => o.Date))
                                if (string.Compare(day.Date.Substring(2), before) < 0)
                                {
                                    SendConsecutive convey;

                                    if (modify != null && int.TryParse(day.Price, out int price))
                                    {
                                        var rate = 1D;

                                        foreach (var param in Array.FindAll(modify, o => string.IsNullOrEmpty(o.Date) == false && o.Date.CompareTo(day.Date.Substring(2)) > 0))
                                            rate *= param.Rate;

                                        convey = new SendConsecutive(day.Date, GetStartingPrice((int)((1 + rate * 1e-2) * price), Market), day.Volume);
                                    }
                                    else
                                        convey = new SendConsecutive(day);

                                    Send?.Invoke(this, convey);
                                }
                            Days.Clear();
                        }
                        foreach (var consecutive in enumerable)
                        {
                            SendConsecutive convey;

                            if (modify != null && int.TryParse(consecutive.Price, out int price))
                            {
                                var rate = 1D;

                                foreach (var param in Array.FindAll(modify, o => string.IsNullOrEmpty(o.Date) == false && o.Date.CompareTo(consecutive.Date.Substring(0, 6)) > 0))
                                    rate *= param.Rate;

                                convey = new SendConsecutive(consecutive.Date, GetStartingPrice((int)((1 + rate * 1e-2) * price), Market), consecutive.Volume);
                            }
                            else
                                convey = new SendConsecutive(consecutive);

                            Send?.Invoke(this, convey);
                        }
                    }
            }
            return GC.GetTotalMemory(true);
        }
        protected internal abstract bool Market
        {
            get; set;
        }
        public event EventHandler<SendConsecutive> Send;
        public Holding(TrendFollowingBasicFutures strategics)
        {
            TF = strategics;
            var catalog = strategics.SetCatalog(strategics);
            Consecutive = new Consecutive[catalog.Length];

            for (int i = 0; i < catalog.Length; i++)
                Consecutive[i] = new Consecutive(catalog[i], this);
        }
        public Holding(SatisfyConditionsAccordingToTrends strategics)
        {
            SC = strategics;
            consecutive = new Consecutive(strategics, this);
        }
        public Holding(TrendToCashflow strategics)
        {
            TC = strategics;
            consecutive = new Consecutive(strategics, this);
        }
        public Holding(TrendsInValuation strategics)
        {
            TV = strategics;
            consecutive = new Consecutive(strategics, this);
        }
        public Holding(TrendsInStockPrices strategics)
        {
            TS = strategics;
            consecutive = new Consecutive(strategics, this);
        }
        public Holding(ScenarioAccordingToTrend strategics)
        {
            ST = strategics;
            consecutive = new Consecutive(strategics, this);
        }
        public abstract string Code
        {
            get; set;
        }
        public abstract int Quantity
        {
            get; set;
        }
        public abstract int BuyPrice
        {
            protected internal get; set;
        }
        public abstract int SellPrice
        {
            protected internal get; set;
        }
        public abstract int Cash
        {
            get; protected internal set;
        }
        public abstract dynamic Purchase
        {
            get; set;
        }
        public abstract dynamic Current
        {
            get; set;
        }
        public abstract dynamic Bid
        {
            get; set;
        }
        public abstract dynamic Offer
        {
            get; set;
        }
        public abstract long Revenue
        {
            get; set;
        }
        public abstract double Rate
        {
            get; set;
        }
        public abstract double Base
        {
            get; protected internal set;
        }
        public abstract double Secondary
        {
            get; protected internal set;
        }
        public abstract bool WaitOrder
        {
            get; set;
        }
        public abstract dynamic FindStrategics
        {
            get;
        }
        public abstract Dictionary<string, dynamic> OrderNumber
        {
            get;
        }
        public virtual string FindStrategicsCode(string code) => Temporary.CodeStorage.First(o => o.Code.Equals(code)).Name;
        public abstract void OnReceiveEvent(string[] param);
        public abstract void OnReceiveBalance(string[] param);
        public abstract void OnReceiveConclusion(string[] param);
        public virtual int GetStartingPrice(int price, bool info)
        {
            switch (price)
            {
                case int n when n >= 0 && n < 0x3E8:
                    return price;

                case int n when n >= 0x3E8 && n < 0x1388:
                    return (price / 5 + 1) * 5;

                case int n when n >= 0x1388 && n < 0x2710:
                    return (price / 0xA + 1) * 0xA;

                case int n when n >= 0x2710 && n < 0xC350:
                    return (price / 0x32 + 1) * 0x32;

                case int n when n >= 0x186A0 && n < 0x7A120 && info:
                    return (price / 0x1F4 + 1) * 0x1F4;

                case int n when n >= 0x7A120 && info:
                    return (price / 0x3E8 + 1) * 0x3E8;

                default:
                    return (price / 0x64 + 1) * 0x64;
            }
        }
        public virtual int GetQuoteUnit(int price, bool info)
        {
            switch (price)
            {
                case int n when n >= 0 && n < 0x3E8:
                    return 1;

                case int n when n >= 0x3E8 && n < 0x1388:
                    return 5;

                case int n when n >= 0x1388 && n < 0x2710:
                    return 0xA;

                case int n when n >= 0x2710 && n < 0xC350:
                    return 0x32;

                case int n when n >= 0x186A0 && n < 0x7A120 && info:
                    return 0x1F4;

                case int n when n >= 0x7A120 && info:
                    return 0x3E8;

                default:
                    return 0x64;
            }
        }
        public virtual Tuple<List<ConvertConsensus>, List<ConvertConsensus>> Consensus
        {
            get; set;
        }
        public abstract event EventHandler<SendSecuritiesAPI> SendBalance;
        public abstract event EventHandler<SendHoldingStocks> SendStocks;
        internal Consecutive[] Consecutive
        {
            get;
        }
        internal abstract Dictionary<DateTime, double> EstimatedPrice
        {
            get; set;
        }
        protected internal abstract DateTime NextOrderTime
        {
            get; set;
        }
        protected internal double MarginRate
        {
            get; private set;
        }
        protected internal uint TransactionMultiplier
        {
            get; private set;
        }
        protected internal TrendFollowingBasicFutures TF
        {
            get;
        }
        protected internal TrendsInStockPrices TS
        {
            get;
        }
        protected internal TrendToCashflow TC
        {
            get;
        }
        protected internal TrendsInValuation TV
        {
            get;
        }
        protected internal ScenarioAccordingToTrend ST
        {
            get;
        }
        protected internal SatisfyConditionsAccordingToTrends SC
        {
            get;
        }
        protected internal Color AdjustTheColorAccordingToTheCurrentSituation(bool wait, int count)
        {
            if (wait)
            {
                if (count > 0)
                    return Color.Gold;

                return Color.Snow;
            }
            else
                return Color.Maroon;
        }
        protected internal DateTime MeasureTheDelayTime(double delay, DateTime time) => time.AddMilliseconds(delay);
        protected internal DateTime MeasureTheDelayTime(int delay, DateTime time) => time.AddSeconds(delay);
        Temporary Temporary
        {
            get; set;
        }
        uint GetTransactionMultiplier(string code)
        {
            if (code[1].Equals('0'))
                switch (code[2])
                {
                    case '1':
                        return 0x3D090;

                    case '5':
                        return 0xC350;

                    case '6':
                        return 0x2710;

                    default:
                        return 0;
                }
            MarginRate *= 0.1;

            return 0xA;
        }
        const string excluding = "191230";
        const string theDate = "192001";
        const string nFormat = "D6";
        const string format = "yyMMdd";
        internal readonly Consecutive consecutive;
    }
    enum TR
    {
        SONBT001 = 0,
        SONBT002 = 1,
        SONBT003 = 2,
        CONET801 = 3,
        CONET002 = 4,
        CONET003 = 5
    }
    public enum OpenOrderType
    {
        신규매수 = 1,
        신규매도 = 2,
        매수취소 = 3,
        매도취소 = 4,
        매수정정 = 5,
        매도정정 = 6,
        예약매수 = 7,
        예약매도 = 8
    }
}