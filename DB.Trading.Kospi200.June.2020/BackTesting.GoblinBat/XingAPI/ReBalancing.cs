using System;
using ShareInvest.EventHandler.XingAPI;
using ShareInvest.Catalog.XingAPI;
using System.Globalization;

namespace ShareInvest.Strategy.XingAPI
{
    public class ReBalancing : TF
    {
        protected internal override void OnReceiveTrend(int volume) => API.Volume += volume;
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
            if (Retrieve.QuotesEnumerable != null)
                foreach (var qe in Retrieve.QuotesEnumerable)
                    foreach (var quotes in qe.Value)
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
            API.TradingJudge[specify.Time] = popShort;

            if (specify.Time == 1440 && GetCheckTime(e.Time))
                OnReceiveTrend(e.Volume);
        }
        bool GetCheckOnTimeByAPI(string time)
        {
            if (specify.Time > 0 && specify.Time < 1440 && (time.Substring(0, 4).Equals(Check) || Check.Equals(string.Empty)) && DateTime.TryParseExact(time, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
            {
                Check = (date + TimeSpan.FromMinutes(specify.Time)).ToString(hm);

                return false;
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
        new bool GetCheckTime(string time)
        {
            if (time.Equals(start) || time.Equals(end))
                return false;

            return true;
        }
        EMA EMA
        {
            get;
        }
    }
}