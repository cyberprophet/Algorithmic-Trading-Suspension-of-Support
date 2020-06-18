using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
        internal int BuyPrice
        {
            private get; set;
        }
        internal Stocks(string key, string code) : base(key)
        {
            while (string.IsNullOrEmpty(chart.Item1) || chart.Item2 == null || chart.Item2.Count == 0)
            {
                if (string.IsNullOrEmpty(chart.Item1) || chart.Item2 == null || chart.Item2.Count == 0)
                    chart = GetBasicChart(code);

                if (Chart == null && string.IsNullOrEmpty(chart.Item1) == false)
                {
                    var temp = GetBasicChart(code, chart.Item1);
                    Chart = temp.Item1;

                    if (temp.Item2 <= 0)
                        chart.Item2.Clear();
                }
                if ((specify.Trend > 0 && specify.Long > 0 && specify.Short > 0) == false)
                    specify = new Catalog.OpenAPI.Specify
                    {
                        Short = 5,
                        Long = 70,
                        Trend = 720
                    };
                if (Chart == null || string.IsNullOrEmpty(chart.Item1) || chart.Item2 == null || chart.Item2.Count == 0)
                    Thread.Sleep(random.Next(0x2BF20));
            }
            Short = new Stack<double>();
            Long = new Stack<double>();
            Trend = new Stack<double>();

            while (chart.Item2.Count > 0)
            {
                var dp = chart.Item2.Dequeue();
                DrawChart(dp.Date, dp.Price);
            }
            foreach (var chart in Chart)
                while (chart.Value.Count > 0)
                {
                    var dp = chart.Value.Dequeue();
                    DrawChart(dp.Date, dp.Price);
                }
            Code = code;
            OnTime = true;
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

            if (specify.Short < Short.Count && specify.Long < Long.Count && specify.Trend < Trend.Count && price < Trend.Peek() && gap > 0 && (price <= Price || Price == 0) && price <= BuyPrice)
                Price = API.OnReceiveOrder(Code, price);
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
            if (OnTime)
                return OnTime = false;

            if (time.Length > 8)
            {
                var date = time.Substring(0, 6);
                var change = string.IsNullOrEmpty(DateChange) == false && date.Equals(DateChange);
                DateChange = date;

                return change;
            }
            return time.Length == 6;
        }
        string DateChange
        {
            get; set;
        }
        bool OnTime
        {
            get; set;
        }
        int Price
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
        IOrderedEnumerable<KeyValuePair<string, Queue<Catalog.OpenAPI.Chart>>> Chart
        {
            get;
        }
        ConnectAPI API => ConnectAPI.GetInstance();
        readonly Random random = new Random();
        readonly Catalog.OpenAPI.Specify specify;
        readonly (string, Queue<Catalog.OpenAPI.Chart>) chart;
    }
}