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
            var temporary = new Temporary(code.Length);

            if (code.Length == 8 && Temporary.CodeStorage != null && Temporary.CodeStorage.Any(o => o.Code.StartsWith(code.Substring(0, 3)) && o.Code.EndsWith(code.Substring(5))))
            {
                var stack = new Stack<Codes>();
                var days = temporary.CallUpTheChartAsync(code);

                foreach (var arg in Temporary.CodeStorage.Where(o => o.Code.StartsWith(code.Substring(0, 3)) && o.Code.EndsWith(code.Substring(5))).OrderByDescending(o => o.MaturityMarketCap.Length == 8 ? o.MaturityMarketCap.Substring(2) : o.MaturityMarketCap))
                    stack.Push(arg);

                Days = days.Result;

                while (stack.Count > 0)
                    yield return temporary.CallUpTheChartAsync(stack.Pop()).Result;
            }
            else if (code.Length == 6 && Temporary.CodeStorage != null && Temporary.CodeStorage.Any(o => o.Code.Equals(code)))
            {
                string sDate = temporary.FindTheChartStartsAsync(code).Result, date = sDate.Substring(0, 6);
                Days = new Queue<Charts>();
                Market = Temporary.CodeStorage.First(o => o.Code.Equals(code)).MarginRate == 1;

                foreach (var day in temporary.CallUpTheChartAsync(code).Result)
                    if (string.Compare(day.Date.Substring(2), date) < 0)
                        Days.Enqueue(day);

                if (int.TryParse(date, out int start))
                {
                    var end = string.Empty;
                    var count = 0;

                    while (string.IsNullOrEmpty(end) || string.Compare(end, DateTime.Now.ToString(format)) <= 0)
                    {
                        if (end.CompareTo(excluding) > 0 && end.CompareTo(theDate) < 0)
                            for (int i = 0; i < 0x1C; i++)
                                count++;

                        yield return temporary.CallUpTheChartAsync(new Catalog.Request.Charts
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
            foreach (var queue in FindTheOldestDueDate(code))
            {
                var enumerable = queue.OrderBy(o => o.Date);

                if (Days.Count > 0)
                {
                    var before = enumerable.First().Date.Substring(0, 6);

                    foreach (var day in Days.OrderBy(o => o.Date))
                        if (string.Compare(day.Date.Substring(2), before) < 0)
                            Send?.Invoke(this, new SendConsecutive(day));

                    Days.Clear();
                }
                foreach (var consecutive in enumerable)
                    Send?.Invoke(this, new SendConsecutive(consecutive));
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
        public Holding(TrendsInStockPrices strategics)
        {
            TS = strategics;
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
        public abstract event EventHandler<SendSecuritiesAPI> SendBalance;
        public abstract event EventHandler<SendHoldingStocks> SendStocks;
        internal Consecutive[] Consecutive
        {
            get;
        }
        protected internal TrendFollowingBasicFutures TF
        {
            get;
        }
        protected internal TrendsInStockPrices TS
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
        const string excluding = "191230";
        const string theDate = "192001";
        const string nFormat = "D6";
        const string format = "yyMMdd";
        internal readonly Consecutive consecutive;
        protected internal const string conclusion = "체결";
        protected internal const string acceptance = "접수";
        protected internal const string confirmation = "확인";
        protected internal const string cancellantion = "취소";
        protected internal const string correction = "정정";
        protected internal const uint transactionMutiplier = 0x3D090;
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
        매도정정 = 6
    }
}