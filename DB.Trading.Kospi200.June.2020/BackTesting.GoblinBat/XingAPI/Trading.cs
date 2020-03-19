using System;
using ShareInvest.Catalog;
using ShareInvest.EventHandler.XingAPI;
using ShareInvest.XingAPI;

namespace ShareInvest.Strategy.XingAPI
{
    public class Trading : Trend
    {
        public Trading(IEvents<Datum> xing, Specify specify) : base(specify)
        {
            foreach (Catalog.XingAPI.Quotes quotes in Retrieve.Quotes)
                if (quotes.Price != null)
                    Analysize(new Chart
                    {
                        Date = long.Parse(quotes.Time),
                        Price = double.Parse(quotes.Price),
                        Volume = int.Parse(quotes.Volume)
                    });
            if (specify.Time == 1440)
                OnTime = true;

            else
                Check = string.Empty;

            xing.Send += Analysize;
        }
        protected ConnectAPI API
        {
            get
            {
                return ConnectAPI.GetInstance();
            }
        }
        private void Analysize(object sender, Datum e)
        {
            if (GetCheckOnTimeByAPI(e.Time))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(EMA.Make(specify.Short, Short.Count, e.Price, Short.Peek()));
            Long.Push(EMA.Make(specify.Long, Long.Count, e.Price, Long.Peek()));
            double popShort = Short.Pop(), popLong = Long.Pop();
            var trend = popShort - popLong - (Short.Peek() - Long.Peek());
            Short.Push(popShort);
            Long.Push(popLong);
            API.Trend[specify.Strategy] = string.Concat(trend.ToString("F2"), " (", specify.Time == 1440 ? "Day" : Check, ")");
        }
        private bool GetCheckOnTimeByAPI(string time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
            {
                if (time.Substring(2, 2).Equals(Check) || Check.Equals(string.Empty))
                {
                    Check = (new TimeSpan(int.Parse(time.Substring(0, 2)), int.Parse(time.Substring(2, 2)), int.Parse(time.Substring(4, 2))) + TimeSpan.FromMinutes(specify.Time)).Minutes.ToString("D2");

                    return false;
                }
                return true;
            }
            else if (OnTime && specify.Time == 1440 && time.Equals(onTime))
            {
                API.OnReceiveBalance = true;

                return OnTime = false;
            }
            return true;
        }
        private EMA EMA
        {
            get;
        }
        private const string onTime = "090000";
    }
}