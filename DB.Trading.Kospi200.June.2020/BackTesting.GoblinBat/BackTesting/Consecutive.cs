using System;
using System.Collections.Generic;
using System.Globalization;

using ShareInvest.Catalog.XingAPI;

namespace ShareInvest.Strategy.Statistics
{
    class Consecutive
    {
        internal Consecutive(BackTesting bt, Specify specify)
        {
            this.bt = bt;
            this.specify = specify;
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
            }
            if (specify.Time == 1440)
            {
                var date = e.Date.ToString().Substring(6);

                if (date.Equals(end))
                    bt.SetStatisticalStorage(e.Date.ToString().Substring(0, 6), e.Price, !specify.RollOver);

                if ((bt.SellOrder.Count > 0 || bt.BuyOrder.Count > 0) && (date.Equals(onTime) || date.Equals(nTime)))
                {
                    bt.SellOrder.Clear();
                    bt.BuyOrder.Clear();
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
        readonly BackTesting bt;
        readonly Specify specify;
        const string nTime = "180000000";
        const string onTime = "090000000";
        const string format = "HHmmss";
        const string hm = "HHmm";
        const string end = "154500000";
    }
}