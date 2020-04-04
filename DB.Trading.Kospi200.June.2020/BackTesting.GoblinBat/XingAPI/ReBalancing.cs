using System;
using ShareInvest.EventHandler.XingAPI;
using ShareInvest.Catalog.XingAPI;

namespace ShareInvest.Strategy.XingAPI
{
    public class ReBalancing : TF
    {
        protected internal ReBalancing(Specify specify) : base(specify)
        {
            foreach (var quotes in Retrieve.Quotes)
                if (quotes.Price != null && quotes.Volume != null && long.TryParse(quotes.Time, out long time) && double.TryParse(quotes.Price, out double price) && int.TryParse(quotes.Volume, out int volume))
                    Analysize(new Catalog.Chart
                    {
                        Date = time,
                        Price = price,
                        Volume = volume
                    });
            if (specify.Time == 1440)
                OnTime = true;

            else
                Check = string.Empty;

            ((Catalog.IEvents<Datum>)API.reals[1]).Send += Analysize;
        }
        void Analysize(object sender, Datum e)
        {
            if (GetCheckOnTimeByAPI(e.Time))
            {
                Short.Pop();
                Long.Pop();
            }
            Short.Push(EMA.Make(specify.Short, Short.Count, e.Price, Short.Peek()));
            Long.Push(EMA.Make(specify.Long, Long.Count, e.Price, Long.Peek()));
            double popShort = Short.Pop(), popLong = Long.Pop();
            API.Max(popShort - popLong - (Short.Peek() - Long.Peek()), specify, Check);
            Short.Push(popShort);
            Long.Push(popLong);
        }
        bool GetCheckOnTimeByAPI(string time)
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
            else if (OnTime && specify.Time == 1440)
            {
                API.OnReceiveBalance = true;
                OnTime = false;

                if (time.Equals(start))
                    return false;
            }
            return true;
        }
        EMA EMA
        {
            get;
        }
    }
}