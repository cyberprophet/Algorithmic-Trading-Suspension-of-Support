using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

using ShareInvest.Analysis.SecondaryIndicators;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;
using ShareInvest.Interface;

namespace ShareInvest.Analysis
{
    class Consecutive
    {
        Stack<double> Short
        {
            get;
        }
        Stack<double> Long
        {
            get;
        }
        Stack<double> Trend
        {
            get;
        }
        internal Consecutive(ScenarioAccordingToTrend strategics, Holding ho)
        {
            Short = new Stack<double>();
            Long = new Stack<double>();
            Trend = new Stack<double>();
            st = strategics;
            Compare = double.NaN;
            this.strategics = strategics;
            this.ho = ho;
            ho.Send += OnReceiveDrawChart;
        }
        internal Consecutive(TrendsInStockPrices strategics, Holding ho)
        {
            Short = new Stack<double>();
            Long = new Stack<double>();
            Trend = new Stack<double>();
            ts = strategics;
            this.strategics = strategics;
            this.ho = ho;
            ho.Send += OnReceiveDrawChart;
        }
        internal Consecutive(TrendFollowingBasicFutures strategics, Holding ho)
        {
            Short = new Stack<double>();
            Long = new Stack<double>();
            tf = strategics;
            this.strategics = strategics;
            this.ho = ho;
            ho.Send += OnReceiveDrawChart;
        }
        internal void Dispose() => ho.Send -= OnReceiveDrawChart;
        internal void Connect(XingAPI.HoldingStocks hs)
        {
            hs.SendConsecutive += OnReceiveDrawChart;
            OnTime = true;
            Check = string.Empty;
        }
        internal void Connect(OpenAPI.HoldingStocks hs)
        {
            hs.SendConsecutive += OnReceiveDrawChart;
            OnTime = true;
            Check = string.Empty;
        }
        void OnReceiveDrawChart(object sender, SendConsecutive e)
        {
            int tShort, tLong, tMinute, trend;

            switch (strategics)
            {
                case TrendsInStockPrices _:
                    tShort = ts.Short;
                    tLong = ts.Long;
                    trend = ts.Trend;

                    switch (ts)
                    {
                        case TrendsInStockPrices sp when sp.LongShort.Equals(LongShort.Minute) && sp.TrendType.Equals(Interface.Trend.Minute) || sp.LongShort.Equals(LongShort.Day) && sp.TrendType.Equals(Interface.Trend.Day):
                            if (GetCheckOnDate(e.Date, sp.LongShort.Equals(LongShort.Minute) && sp.TrendType.Equals(Interface.Trend.Minute) ? 1 : 0x5A0))
                            {
                                Short.Pop();
                                Long.Pop();
                                Trend.Pop();
                            }
                            break;

                        case TrendsInStockPrices sp when sp.LongShort.Equals(LongShort.Day) && sp.TrendType.Equals(Interface.Trend.Minute):
                            if (GetCheckOnDate(e.Date, 1))
                                Trend.Pop();

                            if (GetCheckOnDate(e.Date, 0x5A0))
                            {
                                Short.Pop();
                                Long.Pop();
                            }
                            break;

                        case TrendsInStockPrices sp when sp.LongShort.Equals(LongShort.Minute) && sp.TrendType.Equals(Interface.Trend.Day):
                            if (GetCheckOnDate(e.Date, 0x5A0))
                                Trend.Pop();

                            if (GetCheckOnDate(e.Date, 1))
                            {
                                Short.Pop();
                                Long.Pop();
                            }
                            break;
                    }
                    Trend.Push(Trend.Count > 0 ? EMA.Make(trend, Trend.Count, e.Price, Trend.Peek()) : EMA.Make(e.Price));
                    tMinute = (int)ts.TrendType;
                    break;

                case TrendFollowingBasicFutures _:
                    if (GetCheckOnDate(e.Date, tf.Minute))
                    {
                        Short.Pop();
                        Long.Pop();
                    }
                    tShort = tf.Short;
                    tLong = tf.Long;
                    tMinute = tf.Minute;
                    break;

                case ScenarioAccordingToTrend _:
                    tShort = st.Short;
                    tLong = st.Long;
                    trend = st.Trend;

                    if (e.Date.Length > 6 && double.IsNaN(Compare) && Trend.Count > 0 && string.IsNullOrEmpty(st.Calendar) == false && (e.Date.Length == 8 ? e.Date.Substring(2) : e.Date.Substring(0, 6)).CompareTo(st.Calendar) >= 0)
                    {
                        Compare = Trend.Pop();
                        Trend.Clear();

                        if (int.TryParse(e.Date.Length == 8 ? e.Date.Substring(2, 4) : e.Date.Substring(0, 4), out int closest))
                        {
                            var baseDate = int.MaxValue;
                            var temp = string.Empty;
                            var list = new List<ConvertConsensus>(ho.Consensus.Item1);
                            list.AddRange(ho.Consensus.Item2);

                            foreach (var parse in list.OrderByDescending(o => o.Date))
                                if (int.TryParse(parse.Date.Substring(0, 5).Replace(".", string.Empty), out int date) && Math.Abs(date - closest) < baseDate)
                                {
                                    baseDate = Math.Abs(date - closest);
                                    temp = parse.Date;
                                }
                            ho.EstimatedPrice = new Security(temp, list, st).EstimateThePrice(e.Date, Compare);
                        }
                    }
                    if (GetCheckOnDate(e.Date, 0x5A0))
                    {
                        Short.Pop();
                        Long.Pop();

                        if (double.IsNaN(Compare))
                            Trend.Pop();
                    }
                    if (double.IsNaN(Compare))
                        Trend.Push(Trend.Count > 0 ? EMA.Make(trend, Trend.Count, e.Price, Trend.Peek()) : EMA.Make(e.Price));

                    tMinute = st.IntervalInSeconds;
                    break;

                default:
                    return;
            }
            Short.Push(Short.Count > 0 ? EMA.Make(tShort, Short.Count, e.Price, Short.Peek()) : EMA.Make(e.Price));
            Long.Push(Long.Count > 0 ? EMA.Make(tLong, Long.Count, e.Price, Long.Peek()) : EMA.Make(e.Price));

            if (e.Volume != 0 && e.Date.Length != 8 && Short.Count > 1 && Long.Count > 1)
            {
                double popShort = Short.Pop(), popLong = Long.Pop(), gap = popShort - popLong - (Short.Peek() - Long.Peek());
                Short.Push(popShort);
                Long.Push(popLong);

                switch (sender)
                {
                    case OpenAPI.HoldingStocks os when strategics is TrendsInStockPrices:
                        os.OnReceiveTrendsInStockPrices(gap, Trend.Peek());
                        break;

                    case XingAPI.HoldingStocks xs when strategics is TrendFollowingBasicFutures:
                        xs.OnReceiveTrendFollowingBasicFutures(gap, tMinute);
                        break;

                    case HoldingStocks hs:
                        hs.OnReceiveTrendsInStockPrices(e, gap, Short.Peek(), Long.Peek(), Trend.Count > 0 ? Trend.Peek() : CalculateTheEstimatedPrice(e.Date));
                        break;
                }
            }
        }
        double CalculateTheEstimatedPrice(string date)
        {
            if (DateTime.TryParseExact(date.Substring(0, 6), memorize, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dt) && (TempStorage == null || TempStorage.CompareTo(dt) < 0))
            {
                TempStorage = dt;
                Compare = ho.EstimatedPrice.Last(o => o.Key.Year == dt.Year && o.Key.Month == dt.Month && o.Key.Day == dt.Day).Value;
            }
            return Compare;
        }
        bool GetCheckOnDate(string date, int minute)
        {
            if (date.Length == 6)
            {
                if (minute < 0x5A0 && (date.Substring(0, 4).Equals(Check) || string.IsNullOrEmpty(Check)) && DateTime.TryParseExact(date, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime time))
                {
                    Check = (time + TimeSpan.FromMinutes(minute)).ToString(hm);

                    return false;
                }
                else if (minute == 0x5A0 && OnTime)
                {


                    return OnTime = false;
                }
                return OnTime == false;
            }
            else if (minute == 0x5A0)
            {
                if (date.Length > 8 && int.TryParse(date.Substring(6, 2), out int hour))
                    CME = hour > 17 || hour < 5;

                return date.Length > 8 && (strategics.Code.Length == 6 ? GetCheckOnDeadline(date) : date.Substring(6).Equals(onTime) == false);
            }
            else
                return date.Length > 8 && GetCheckOnTime(date, minute);
        }
        bool GetCheckOnTime(string time, int minute)
        {
            string check = time.Substring(6, 6), on = time.Substring(6);

            if ((string.IsNullOrEmpty(Check) || Check.Equals(check.Substring(0, 4)) || GetCheckOnDeadline(check) || on.Equals(onTime) || on.Equals(nTime)) && DateTime.TryParseExact(check, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
            {
                if (strategics.Code.Length == 6 && string.IsNullOrEmpty(Check) == false && string.Compare(check.Substring(0, 4), end) > 0)
                    Check = string.Empty;

                else if (strategics.Code.Length == 6 && string.Compare(check.Substring(0, 4), end) > 0)
                    return string.IsNullOrEmpty(Check);

                Check = (date + TimeSpan.FromMinutes(minute)).ToString(hm);

                return false;
            }
            return true;
        }
        bool GetCheckOnDeadline(string time)
        {
            if (time.Length > 6 && strategics.Code.Length == 6)
            {
                var date = time.Substring(0, 6);
                var change = string.IsNullOrEmpty(DateChange) == false && date.Equals(DateChange);
                DateChange = date;

                return change;
            }
            else
                return time.Equals("154500");
        }
        bool CME
        {
            get; set;
        }
        bool OnTime
        {
            get; set;
        }
        double Compare
        {
            get; set;
        }
        string Check
        {
            get; set;
        }
        string DateChange
        {
            get; set;
        }
        DateTime TempStorage
        {
            get; set;
        }
        EMA EMA
        {
            get;
        }
        [Conditional("DEBUG")]
        void SendMessage(string date, string price, int volume, double[] param)
        {
            var sb = new StringBuilder();

            foreach (var str in param)
                sb.Append(str).Append(" ");

            if (date.Substring(6).Equals("090000000"))
                Console.WriteLine("D_" + date + " P_" + price + " V_" + volume + " E_" + sb);
        }
        readonly ScenarioAccordingToTrend st;
        readonly TrendsInStockPrices ts;
        readonly TrendFollowingBasicFutures tf;
        readonly IStrategics strategics;
        readonly Holding ho;
        const string onTime = "090000000";
        const string nTime = "180000000";
        const string format = "HHmmss";
        const string hm = "HHmm";
        const string memorize = "yyMMdd";
        const string end = "1529";
    }
}