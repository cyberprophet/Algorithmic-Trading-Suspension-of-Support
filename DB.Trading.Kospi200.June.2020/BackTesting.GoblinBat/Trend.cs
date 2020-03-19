using System;
using System.Collections.Generic;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy
{
    public class Trend
    {
        public Trend(Specify specify)
        {
            this.specify = specify;
            Short = new Stack<double>(512);
            Long = new Stack<double>(512);

            foreach (Chart chart in Retrieve.Chart)
                Analysize(chart);
        }
        protected Stack<double> Short
        {
            get;
        }
        protected Stack<double> Long
        {
            get;
        }
        protected bool OnTime
        {
            get; set;
        }
        protected string Check
        {
            get; set;
        }
        protected void Analysize(Chart chart)
        {
            if (GetCheckOnTime(chart.Date))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, chart.Price, Short.Peek()) : EMA.Make(chart.Price));
            Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, chart.Price, Long.Peek()) : EMA.Make(chart.Price));
        }
        protected bool GetCheckOnTime(string time)
        {
            var onTime = time.Substring(6, 6);

            if (onTime.Substring(2, 2).Equals(Check) || Check == null || onTime.Equals(end) || onTime.Equals(start))
            {
                Check = (new TimeSpan(int.Parse(onTime.Substring(0, 2)), int.Parse(onTime.Substring(2, 2)), int.Parse(onTime.Substring(4, 2))) + TimeSpan.FromMinutes(specify.Time)).Minutes.ToString("D2");

                return false;
            }
            return true;
        }
        private bool GetCheckOnTime(long time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
                return time.ToString().Length > 8 && GetCheckOnTime(time.ToString());

            else if (specify.Time == 1440)
                return time.ToString().Length > 8 && time.ToString().Substring(6).Equals(onTime) == false;

            return false;
        }
        private EMA EMA
        {
            get;
        }
        protected readonly Specify specify;
        private const string onTime = "090000000";
        private const string end = "154500";
        private const string start = "090000";
    }
}