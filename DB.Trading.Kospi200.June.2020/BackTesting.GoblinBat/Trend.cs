using System;
using System.Collections.Generic;
using ShareInvest.Catalog;
using ShareInvest.XingAPI;

namespace ShareInvest.Strategy
{
    public class Trend
    {
        protected internal Trend(Specify specify)
        {
            this.specify = specify;
            Short = new Stack<double>(512);
            Long = new Stack<double>(512);

            foreach (Chart chart in Retrieve.Chart)
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
        protected internal bool OnTime
        {
            get; set;
        }
        protected internal string Check
        {
            get; set;
        }
        protected internal string On => (DateTime.Now.Hour == 15 && DateTime.Now.Minute < 45 || DateTime.Now.Hour < 15) && DateTime.Now.Hour > 4 ? start : cme;
        protected internal ConnectAPI API => ConnectAPI.GetInstance();
        protected internal void Analysize(Chart chart)
        {
            if (GetCheckOnTime(chart.Date))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, chart.Price, Short.Peek()) : EMA.Make(chart.Price));
            Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, chart.Price, Long.Peek()) : EMA.Make(chart.Price));
        }
        protected internal bool GetCheckOnTime(string time)
        {
            var onTime = time.Substring(6, 6);

            if (onTime.Substring(2, 2).Equals(Check) || Check == null || onTime.Equals(end) || onTime.Equals(start))
            {
                Check = (new TimeSpan(int.Parse(onTime.Substring(0, 2)), int.Parse(onTime.Substring(2, 2)), int.Parse(onTime.Substring(4, 2))) + TimeSpan.FromMinutes(specify.Time)).Minutes.ToString("D2");

                return false;
            }
            return true;
        }
        void StartProgress()
        {

        }
        bool GetCheckOnTime(long time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
                return time.ToString().Length > 8 && GetCheckOnTime(time.ToString());

            else if (specify.Time == 1440)
                return time.ToString().Length > 8 && time.ToString().Substring(6).Equals(onTime) == false;

            return false;
        }
        EMA EMA
        {
            get;
        }
        protected internal readonly Specify specify;
        const string cme = "180000";
        const string onTime = "090000000";
        const string end = "154500";
        const string start = "090000";
    }
}