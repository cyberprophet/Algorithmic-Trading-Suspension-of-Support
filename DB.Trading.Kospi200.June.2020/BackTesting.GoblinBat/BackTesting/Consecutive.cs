using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ShareInvest.Catalog;

namespace ShareInvest.Strategy.Statistics
{
    class Consecutive
    {
        internal Consecutive(BackTesting bt, Catalog.XingAPI.Specify specify)
        {
            this.bt = bt;
            this.specify = specify;
            this.judge = specify.Strategy.Length == 2 && int.TryParse(specify.Strategy, out int judge) ? judge : int.MaxValue;
            Short = new Stack<double>(256);
            Long = new Stack<double>(256);
            bt.SendDatum += Analysize;
        }
        void Analysize(object sender, EventHandler.BackTesting.Datum e)
        {
            if (GetCheckOnTime(e.Date))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, e.Price, Short.Peek()) : EMA.Make(e.Price));
            Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, e.Price, Long.Peek()) : EMA.Make(e.Price));

            if (e.Date.ToString().Length > 8)
            {
                double popShort = Short.Pop(), popLong = Long.Pop();
                bt.Max(popShort - popLong - (Short.Peek() - Long.Peek()), specify.Time);
                Short.Push(popShort);
                Long.Push(popLong);
                bt.TradingJudge[specify.Time] = popShort;

                if (specify.Time == 1440)
                    switch (e.Date.ToString().Substring(6))
                    {
                        case end:
                            bt.SetStatisticalStorage(e.Date.ToString().Substring(0, 6), e.Price, !specify.RollOver);
                            break;

                        case onTime:
                        case nTime:
                            if (bt.SellOrder.Count > 0 || bt.BuyOrder.Count > 0)
                            {
                                bt.SellOrder.Clear();
                                bt.BuyOrder.Clear();
                            }
                            break;

                        default:
                            if (int.TryParse(e.Date.ToString().Substring(6, 6), out int time) && (time < 090001 && time > 045959) == false && (time > 153459 && time < 180000) == false && string.IsNullOrEmpty(bt.Classification) == false)
                            {
                                var judge = bt.Judge.OrderBy(o => o.Key);
                                var trend = judge.First().Value;

                                if ((bt.Classification.Equals(buy) ? e.Volume + trend > e.Volume : e.Volume + trend < e.Volume) && GetJudgeTheTrading(e.Price, e.Volume))
                                {
                                    if (bt.Judge.Count > 2)
                                    {
                                        var num = 0;

                                        foreach (var kv in judge)
                                        {
                                            if (kv.Key == judge.First().Key)
                                                continue;

                                            if (kv.Key == 1440)
                                                break;

                                            if (bt.Classification.Equals(buy) && kv.Value < 0 || bt.Classification.Equals(sell) && kv.Value > 0)
                                                num++;
                                        }
                                        if (num == 8 && bt.SetConclusion(e.Date, e.Price, bt.Classification))
                                            return;
                                    }
                                    if (bt.Judge.Count == 2 && bt.SetConclusion(e.Date, e.Price, bt.Classification))
                                        return;
                                }
                                if (bt.Judge.Count > 2 && this.judge % 2 == 0)
                                {
                                    var num = 0;

                                    if (bt.Quantity > 0 && e.Volume < -this.judge && e.Volume + trend < e.Volume)
                                    {
                                        foreach (var kv in judge)
                                        {
                                            if (kv.Key == judge.First().Key)
                                                continue;

                                            if (kv.Key == 1440)
                                                break;

                                            if (bt.Classification.Equals(buy) && kv.Value > 0)
                                                num++;
                                        }
                                        if (num == 8 && bt.SetConclusion(e.Date, e.Price, sell))
                                            return;
                                    }
                                    else if (bt.Quantity < 0 && e.Volume > this.judge && e.Volume + trend > e.Volume)
                                    {
                                        foreach (var kv in judge)
                                        {
                                            if (kv.Key == judge.First().Key)
                                                continue;

                                            if (kv.Key == 1440)
                                                break;

                                            if (bt.Classification.Equals(sell) && kv.Value < 0)
                                                num++;
                                        }
                                        if (num == 8 && bt.SetConclusion(e.Date, e.Price, buy))
                                            return;
                                    }
                                }
                            }
                            break;
                    }
            }
        }
        bool GetCheckOnTime(long time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
                return time.ToString().Length > 8 && GetCheckOnTime(time.ToString());

            else if (specify.Time == 1440)
                return time.ToString().Length > 8 && time.ToString().Substring(6).Equals(onTime) == false;

            return false;
        }
        bool GetCheckOnTime(string time)
        {
            var onTime = time.Substring(6, 6);

            if ((onTime.Substring(0, 4).Equals(Check) || string.IsNullOrEmpty(Check) || onTime.Equals(end.Substring(0, 6)) || time.Substring(6).Equals(Consecutive.onTime) || time.Substring(6).Equals(nTime)) && DateTime.TryParseExact(onTime, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
            {
                Check = (date + TimeSpan.FromMinutes(specify.Time)).ToString(hm);

                return false;
            }
            return true;
        }
        bool GetJudgeTheTrading(double price, int quantity)
        {
            var max = specify.Assets / (price * Const.TransactionMultiplier * specify.MarginRate);

            switch (bt.Classification)
            {
                case buy:
                    if (max - bt.Quantity > 1)
                        return judge < quantity;

                    break;

                case sell:
                    if (max + bt.Quantity > 1)
                        return -judge > quantity;

                    break;
            }
            return false;
        }
        Stack<double> Short
        {
            get;
        }
        Stack<double> Long
        {
            get;
        }
        EMA EMA
        {
            get;
        }
        string Check
        {
            get; set;
        }
        readonly int judge;
        readonly BackTesting bt;
        readonly Catalog.XingAPI.Specify specify;
        const string nTime = "180000000";
        const string onTime = "090000000";
        const string format = "HHmmss";
        const string hm = "HHmm";
        const string end = "154500000";
        internal const string sell = "1";
        internal const string buy = "2";
    }
}