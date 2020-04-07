using System;
using System.Collections.Generic;
using ShareInvest.GoblinBatContext;
using ShareInvest.Catalog;

namespace ShareInvest.Strategy
{
    class Analysize : CallUpGoblinBat
    {
        internal Analysize(Specify specify, string key) : base(key)
        {
            this.specify = specify;
            Short = new Stack<double>(512);
            Long = new Stack<double>(512);
            info = new Information(key);
        }
        void OnReceiveAnalysize(Chart ch)
        {
            if (GetCheckOnTime(ch.Date))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, ch.Price, Short.Peek()) : EMA.Make(ch.Price));
            Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, ch.Price, Long.Peek()) : EMA.Make(ch.Price));
            double popShort = Short.Pop(), popLong = Long.Pop();
            int i, quantity = Short.Count > 1 && Long.Count > 1 ? popShort - popLong - (Short.Peek() - Long.Peek()) > 0 ? 1 : -1 : 0;
            var max = specify.Assets / (specify.Code.Length == 8 ? ch.Price * Const.TransactionMultiplier * Const.MarginRate200402 : ch.Price);
            Short.Push(popShort);
            Long.Push(popLong);

            if (ch.Date > 99999999 && ch.Date.ToString().Substring(6, 4).Equals("1545"))
            {
                info.Save(ch, specify);

                return;
            }
            if (ch.Date > 99999999 && info.Quantity != 0 && GetRemainingDate(specify.Code, ch.Date))
            {
                for (i = Math.Abs(info.Quantity); i > 0; i--)
                    info.Operate(ch, info.Quantity > 0 ? -1 : 1);

                return;
            }
            if (ch.Date > 99999999 && Math.Abs(info.Quantity + quantity) < max)
                info.Operate(ch, quantity);

            else if (ch.Date > 99999999 && Math.Abs(info.Quantity) > max)
                info.Operate(ch, info.Quantity > 0 ? -1 : 1);
        }
        bool GetCheckOnTime(long time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
                return time.ToString().Length > 8 && GetCheckOnTime(time.ToString());

            else if (specify.Time == 1440)
                return time.ToString().Length > 8 && time.ToString().Substring(6).Equals("090000000") == false;

            return false;
        }
        bool GetCheckOnTime(string time)
        {
            var onTime = time.Substring(6, 6);

            if (onTime.Substring(2, 2).Equals(Check) || Check == null || onTime.Equals("154500"))
            {
                Check = (new TimeSpan(int.Parse(onTime.Substring(0, 2)), int.Parse(onTime.Substring(2, 2)), int.Parse(onTime.Substring(4, 2))) + TimeSpan.FromMinutes(specify.Time)).Minutes.ToString("D2");

                return false;
            }
            return true;
        }
        string Check
        {
            get; set;
        }
        EMA EMA
        {
            get;
        }
        Stack<double> Short
        {
            get;
        }
        Stack<double> Long
        {
            get;
        }
        readonly Specify specify;
        readonly Information info;
    }
}