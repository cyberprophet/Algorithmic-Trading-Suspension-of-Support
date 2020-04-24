using System;
using System.Collections.Generic;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.GoblinBatContext;
using ShareInvest.Verify;
using ShareInvest.XingAPI;

namespace ShareInvest.Strategy.XingAPI
{
    public class TF : CallUpBasicInformation
    {
        protected internal TF(Specify specify) : base(KeyDecoder.GetKey())
        {
            this.specify = specify;
            Short = GetBasicChart(new Stack<double>(512), specify, specify.Short);
            Long = GetBasicChart(new Stack<double>(512), specify, specify.Long);

            if (Short.Count == 0 || Long.Count == 0)
            {
                sCollection = true;
                lCollection = true;
                var charts = new Queue<Models.Charts>(256);

                if (Short.Count > 0)
                {
                    Short.Clear();
                    sCollection = false;
                    LongValue = new Dictionary<string, double>();
                }
                else if (Long.Count > 0)
                {
                    Long.Clear();
                    lCollection = false;
                    ShortValue = new Dictionary<string, double>();
                }
                else
                {
                    ShortValue = new Dictionary<string, double>();
                    LongValue = new Dictionary<string, double>();
                }
                foreach (Catalog.Chart chart in Retrieve.Chart)
                    Analysize(chart);

                if (sCollection)
                    foreach (var kv in ShortValue)
                        charts.Enqueue(new Models.Charts
                        {
                            Code = specify.Code,
                            Time = (int)specify.Time,
                            Base = specify.Short,
                            Date = kv.Key,
                            Value = kv.Value
                        });
                if (lCollection)
                    foreach (var kv in LongValue)
                        charts.Enqueue(new Models.Charts
                        {
                            Code = specify.Code,
                            Time = (int)specify.Time,
                            Base = specify.Long,
                            Date = kv.Key,
                            Value = kv.Value
                        });
                if (charts.Count > 0 && SetBasicChart(charts))
                    charts.Clear();
            }
        }
        protected internal Stack<double> Short
        {
            get;
        }
        protected internal Stack<double> Long
        {
            get;
        }
        protected internal int Volume
        {
            get; set;
        }
        protected internal bool OnTime
        {
            get; set;
        }
        protected internal string Check
        {
            get; set;
        }
        protected internal ConnectAPI API => ConnectAPI.GetInstance();
        protected internal virtual void OnReceiveTrend(int volume) => Console.WriteLine(volume);
        protected internal void Analysize(Catalog.Chart chart)
        {
            var input = GetCheckOnTime(chart.Date);

            if (input)
            {
                Short.Pop();
                Long.Pop();
            }
            double st = Short.Count > 0 ? Short.Peek() : chart.Price, lt = Long.Count > 0 ? Long.Peek() : chart.Price;

            if (input == false && (lCollection || sCollection))
            {
                var date = chart.Date.ToString();
                date = date.Length == 8 ? date.Substring(2) : date;

                switch (specify.Time)
                {
                    case 1440:
                        date = date.Substring(0, 6);
                        break;

                    default:
                        if (date.Length > 8)
                            date = date.Substring(0, 10);

                        break;
                }
                if (sCollection)
                    ShortValue[date] = st;

                if (lCollection)
                    LongValue[date] = lt;
            }
            Short.Push(Short.Count > 0 ? EMA.Make(specify.Short, Short.Count, chart.Price, st) : EMA.Make(chart.Price));
            Long.Push(Long.Count > 0 ? EMA.Make(specify.Long, Long.Count, chart.Price, lt) : EMA.Make(chart.Price));

            if (specify.Time == 1440 && chart.Volume != 0 && GetCheckTime(chart.Date.ToString()))
                OnReceiveTrend(chart.Volume);
        }
        protected internal bool GetCheckOnTime(long time)
        {
            if (specify.Time > 0 && specify.Time < 1440)
                return time.ToString().Length > 8 && GetCheckOnTime(time.ToString());

            else if (specify.Time == 1440)
                return time.ToString().Length > 8 && time.ToString().Substring(6).Equals(onTime) == false;

            return false;
        }
        protected internal bool GetCheckTime(string time)
        {
            if (time.Substring(6).Equals(onTime))
                return false;

            if (time.Substring(6, 6).Equals(end))
                return false;

            return true;
        }
        bool GetCheckOnTime(string time)
        {
            var onTime = time.Substring(6, 6);

            if (onTime.Substring(2, 2).Equals(Check) || Check == null || onTime.Equals(end) || onTime.Equals(start))
            {
                Check = (new TimeSpan(int.Parse(onTime.Substring(0, 2)), int.Parse(onTime.Substring(2, 2)), int.Parse(onTime.Substring(4, 2))) + TimeSpan.FromMinutes(specify.Time)).Minutes.ToString("D2");

                return false;
            }
            return true;
        }
        EMA EMA
        {
            get;
        }
        Dictionary<string, double> ShortValue
        {
            get;
        }
        Dictionary<string, double> LongValue
        {
            get;
        }
        const string onTime = "090000000";
        readonly bool sCollection;
        readonly bool lCollection;
        protected internal const string end = "154500";
        protected internal const string start = "090000";
        protected internal readonly Specify specify;
    }
    enum Classification
    {
        Sell = '1',
        Buy = '2'
    }
}