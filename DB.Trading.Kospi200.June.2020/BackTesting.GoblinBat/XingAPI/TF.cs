using System;
using System.Collections.Generic;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.XingAPI;

namespace ShareInvest.Strategy.XingAPI
{
    public class TF
    {
        protected internal TF(Specify specify)
        {
            this.specify = specify;
            Short = new Stack<double>(512);
            Long = new Stack<double>(512);

            foreach (Catalog.Chart chart in Retrieve.Chart)
                Analysize(chart);
        }
        protected internal Stack<double> Short
        {
            get;
        }
        protected internal Stack<double> Long
        {
            get;
        }
        protected internal int Volume
        {
            get; set;
        }
        protected internal bool OnTime
        {
            get; set;
        }
        protected internal string Check
        {
            get; set;
        }
        protected internal ConnectAPI API => ConnectAPI.GetInstance();
        protected internal virtual void OnReceiveTrend(int volume) => Console.WriteLine(volume);
        protected internal void Analysize(Catalog.Chart chart)
        {
            if (GetCheckOnTime(chart.Date))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, chart.Price, Short.Peek()) : EMA.Make(chart.Price));
            Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, chart.Price, Long.Peek()) : EMA.Make(chart.Price));

            if (specify.Time == 1440 && chart.Volume != 0 && GetCheckTime(chart.Date.ToString()))
                OnReceiveTrend(chart.Volume);
        }
        protected internal bool GetCheckOnTime(long time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
                return time.ToString().Length > 8 && GetCheckOnTime(time.ToString());

            else if (specify.Time == 1440)
                return time.ToString().Length > 8 && time.ToString().Substring(6).Equals(onTime) == false;

            return false;
        }
        protected internal bool GetCheckTime(string time)
        {
            if (time.Substring(6).Equals(onTime))
                return false;

            if (time.Substring(6, 6).Equals(end))
                return false;

            return true;
        }
        bool GetCheckOnTime(string time)
        {
            var onTime = time.Substring(6, 6);

            if (onTime.Substring(2, 2).Equals(Check) || Check == null || onTime.Equals(end) || onTime.Equals(start))
            {
                Check = (new TimeSpan(int.Parse(onTime.Substring(0, 2)), int.Parse(onTime.Substring(2, 2)), int.Parse(onTime.Substring(4, 2))) + TimeSpan.FromMinutes(specify.Time)).Minutes.ToString("D2");

                return false;
            }
            return true;
        }
        EMA EMA
        {
            get;
        }
        const string onTime = "090000000";
        protected internal const string end = "154500";
        protected internal const string start = "090000";
        protected internal readonly Specify specify;
    }
    enum Classification
    {
        Sell = '1',
        Buy = '2'
    }
}