using System;
using System.Collections.Generic;
using ShareInvest.EventHandler;
using ShareInvest.Interface.Struct;
using ShareInvest.OpenAPI;

namespace ShareInvest.Strategy
{
    public class Trading
    {
        public Trading(Specify specify)
        {
            this.specify = specify;
            Short = new Stack<double>(512);
            Long = new Stack<double>(512);
            SendDatum += Analysize;

            foreach (Chart chart in Retrieve.GetInstance(specify.Code).Chart)
                SendDatum?.Invoke(this, new Datum(chart));

            SendDatum -= Analysize;
            api = ConnectAPI.GetInstance();
            quotes = new Quotes(specify, api);
            api.SendDatum += Analysize;
        }
        private void Analysize(object sender, Datum e)
        {
            if (api == null)
            {
                if (GetCheckOnTime(e.Chart.Date))
                {
                    Short.Pop();
                    Long.Pop();
                }
                Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, e.Chart.Price, Short.Peek()) : EMA.Make(e.Chart.Price));
                Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, e.Chart.Price, Long.Peek()) : EMA.Make(e.Chart.Price));

                return;
            }
            if (GetCheckOnTimeByAPI(e.Time))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(EMA.Make(specify.Short, Short.Count, e.Price, Short.Peek()));
            Long.Push(EMA.Make(specify.Long, Long.Count, e.Price, Long.Peek()));
            double popShort = Short.Pop(), popLong = Long.Pop();
            quotes.SetTrendFollowing(specify.Assets / (specify.Code.Length == 8 ? e.Price * Const.TransactionMultiplier * Const.MarginRate : e.Price) - Math.Abs(api.Quantity) - api.OrderNumber.Count, popShort - popLong - (Short.Peek() - Long.Peek()));
            Short.Push(popShort);
            Long.Push(popLong);
        }
        private bool GetCheckOnTimeByAPI(string time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
            {
                if (time.Substring(2, 2).Equals(Check) || Check == null || time.Equals("154500"))
                {
                    Check = (new TimeSpan(int.Parse(time.Substring(0, 2)), int.Parse(time.Substring(2, 2)), int.Parse(time.Substring(4, 2))) + TimeSpan.FromMinutes(specify.Time)).Minutes.ToString("D2");

                    return false;
                }
                return true;
            }
            else if (OnTime == false && specify.Time == 1440 && time.Equals("090000"))
                return OnTime = true;

            return false;
        }
        private bool GetCheckOnTime(long time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
                return time.ToString().Length > 8 && GetCheckOnTime(time.ToString());

            else if (specify.Time == 1440)
                return time.ToString().Length > 8 && time.ToString().Substring(6).Equals("090000000") == false;

            return false;
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
        private bool OnTime
        {
            get; set;
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
        private readonly Quotes quotes;
        private readonly ConnectAPI api;
        public event EventHandler<Datum> SendDatum;
    }
}