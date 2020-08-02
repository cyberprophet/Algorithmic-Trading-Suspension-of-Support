using System;
using System.Collections.Generic;
using System.Globalization;

using ShareInvest.Analysis.SecondaryIndicators;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Analysis
{
    class Consecutive
    {
        internal Stack<double> Short
        {
            get;
        }
        internal Stack<double> Long
        {
            get;
        }
        internal Consecutive(TrendFollowingBasicFutures strategics, Holding ho)
        {
            Short = new Stack<double>();
            Long = new Stack<double>();
            tf = strategics;
            this.ho = ho;
            ho.Send += OnReceiveDrawChart;
        }
        internal void Dispose() => ho.Send -= OnReceiveDrawChart;
        internal void Connect(Holding holding) => holding.Send += OnReceiveDrawChart;
        void OnReceiveDrawChart(object sender, SendConsecutive e)
        {
            if (GetCheckOnDate(e.Date))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(Short.Count > 0 ? EMA.Make(tf.Short, Short.Count, e.Price, Short.Peek()) : EMA.Make(e.Price));
            Long.Push(Long.Count > 0 ? EMA.Make(tf.Long, Long.Count, e.Price, Long.Peek()) : EMA.Make(e.Price));
        }
        bool GetCheckOnDate(string date)
        {
            if (tf.Minute == 0x5A0)
            {
                if (date.Length > 8 && int.TryParse(date.Substring(6, 2), out int hour))
                    CME = hour > 17 || hour < 5;

                return date.Length > 8 && date.Substring(6).Equals(onTime) == false;
            }
            else
                return date.Length > 8 && GetCheckOnTime(date);
        }
        bool GetCheckOnTime(string time)
        {
            string check = time.Substring(6, 6), on = time.Substring(6);

            if ((string.IsNullOrEmpty(Check) || Check.Equals(check.Substring(0, 4)) || GetCheckOnDeadline(check) || on.Equals(onTime) || on.Equals(nTime)) && DateTime.TryParseExact(check, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
            {
                Check = (date + TimeSpan.FromMinutes(tf.Minute)).ToString(hm);

                return false;
            }
            return true;
        }
        bool GetCheckOnDeadline(string time)
        {
            if (tf.Code.Length == 6)
            {
                return false;
            }
            else
                return time.Equals("154500");
        }
        bool CME
        {
            get; set;
        }
        string Check
        {
            get; set;
        }
        EMA EMA
        {
            get;
        }
        readonly TrendFollowingBasicFutures tf;
        readonly Holding ho;
        const string onTime = "090000000";
        const string nTime = "180000000";
        const string format = "HHmmss";
        const string hm = "HHmm";
    }
}