using System;
using System.Collections.Generic;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.XingAPI;

namespace ShareInvest.Strategy.XingAPI
{
    public class TF
    {
        internal protected TF(Specify specify)
        {
            this.specify = specify;
            Short = new Stack<double>(512);
            Long = new Stack<double>(512);

            foreach (Catalog.Chart chart in Retrieve.Chart)
                Analysize(chart);
        }
        internal protected ConnectAPI API
        {
            get
            {
                return ConnectAPI.GetInstance();
            }
        }
        internal protected Stack<double> Short
        {
            get;
        }
        internal protected Stack<double> Long
        {
            get;
        }
        internal protected bool OnTime
        {
            get; set;
        }
        internal protected string Check
        {
            get; set;
        }
        internal protected string On
        {
            get
            {
                return (DateTime.Now.Hour == 15 && DateTime.Now.Minute < 45 || DateTime.Now.Hour < 15) && DateTime.Now.Hour > 4 ? start : cme;
            }
        }
        internal protected void Analysize(Catalog.Chart chart)
        {
            if (GetCheckOnTime(chart.Date))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, chart.Price, Short.Peek()) : EMA.Make(chart.Price));
            Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, chart.Price, Long.Peek()) : EMA.Make(chart.Price));
        }
        internal protected bool GetCheckOnTime(string time)
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
        private const string cme = "180000";
        private const string onTime = "090000000";
        private const string end = "154500";
        private const string start = "090000";
        internal protected readonly Specify specify;
    }
}