using System;
using System.Collections.Generic;
using ShareInvest.CallUpDataBase;
using ShareInvest.Strategy;

namespace ShareInvest.Analysis
{
    public class Analysize : CallUpGoblinBat
    {
        public Analysize(Specify specify)
        {
            this.specify = specify;
            Short = new Stack<double>(512);
            Long = new Stack<double>(512);

            foreach (Chart chart in Retrieve.GetInstance(specify.Code).Chart)
                Analysis(chart);
        }
        private void Analysis(Chart ch)
        {
            bool check = false;
            string time = ch.Date.ToString();

            if (specify.Time > 0 && specify.Time < 1440)
                check = time.Length > 8 && GetCheckOnTime(time);

            else if (specify.Time == 1440)
                check = time.Length > 8 && time.Substring(6).Equals("090000000") == false;

            if (check)
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, ch.Price, Short.Peek()) : EMA.Make(ch.Price));
            Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, ch.Price, Long.Peek()) : EMA.Make(ch.Price));
            double popShort = Short.Pop(), popLong = Long.Pop();
            var trend = Short.Count > 1 && Long.Count > 1 ? popShort - popLong - (Short.Peek() - Long.Peek()) > 0 ? 1 : -1 : 0;
            Short.Push(popShort);
            Long.Push(popLong);
        }
        private bool GetCheckOnTime(string time)
        {
            var onTime = time.Substring(6, 6);

            if (onTime.Substring(2, 2).Equals(Check) || Check == null || onTime.Equals("154500"))
            {
                Check = (new TimeSpan(int.Parse(onTime.Substring(0, 2)), int.Parse(onTime.Substring(2, 2)), int.Parse(onTime.Substring(4, 2))) + TimeSpan.FromMinutes(specify.Time)).Minutes.ToString("D2");

                return false;
            }
            return true;
        }
        private string Check
        {
            get; set;
        }
        private EMA EMA
        {
            get;
        }
        private Stack<double> Short
        {
            get;
        }
        private Stack<double> Long
        {
            get;
        }
        private readonly Specify specify;
    }
}