using System.Collections.Generic;

using ShareInvest.GoblinBatContext;
using ShareInvest.OpenAPI;

namespace ShareInvest.Strategy.OpenAPI
{
    class Stocks : CallUpBasicInformation
    {
        internal string Code
        {
            get; private set;
        }
        internal Stocks(string key, string code) : base(key)
        {
            var chart = GetBasicChart(code);
            Code = code;
            OnTime = true;
            Short = new Stack<double>();
            Long = new Stack<double>();
            Trend = new Stack<double>();
            specify = new Catalog.OpenAPI.Specify
            {
                Short = 5,
                Long = 60,
                Trend = 720
            };
            foreach (var dp in chart.Item2)
                DrawChart(dp.Date, dp.Price);
        }
        internal void DrawChart(string time, int price)
        {
            if (GetCheckOnTimeByAPI(time))
            {
                Short.Pop();
                Long.Pop();
                Trend.Pop();
            }
            Short.Push(EMA.Make(specify.Short, Short.Count, price, Short.Peek()));
            Long.Push(EMA.Make(specify.Long, Long.Count, price, Long.Peek()));
            Trend.Push(EMA.Make(specify.Trend, Trend.Count, price, Trend.Peek()));
            double popShort = Short.Pop(), popLong = Long.Pop(), gap = popShort - popLong - (Short.Peek() - Long.Peek());
            Short.Push(popShort);
            Long.Push(popLong);
        }
        void DrawChart(long date, dynamic price)
        {
            if (GetCheckOnTimeByAPI(date.ToString()))
            {
                Short.Pop();
                Long.Pop();
                Trend.Pop();
            }
            Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, price, Short.Peek()) : EMA.Make(price));
            Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, price, Long.Peek()) : EMA.Make(price));
            Trend.Push(Trend.Count > 0 ? EMA.Make(specify.Trend, Trend.Count, price, Trend.Peek()) : EMA.Make(price));
        }
        bool GetCheckOnTimeByAPI(string time)
        {
            if (OnTime && time.Equals(start))
                return OnTime = false;

            if (time.Length > 8)
                return time.Substring(6).Equals(onTime) == false;

            return time.Length == 6;
        }
        bool OnTime
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
        Stack<double> Trend
        {
            get;
        }
        ConnectAPI API => ConnectAPI.GetInstance();
        readonly Catalog.OpenAPI.Specify specify;
        const string start = "090000";
        const string onTime = "090000000";
    }
}