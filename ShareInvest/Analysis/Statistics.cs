using System;
using System.Collections.Generic;
using ShareInvest.BackTest;
using ShareInvest.Chart;
using ShareInvest.EventHandler;
using ShareInvest.Secondary;
using ShareInvest.Secret;

namespace ShareInvest.Analysis
{
    public class Statistics : Conceal
    {
        public Statistics(int reaction)
        {
            info = new Information();
            b = new BollingerBands(20, 2);
            ema = new EMA(5, 60);
            sma = new double[b.MidPeriod];
            trend_width = new List<double>(32768);
            short_ema = new List<double>(32768);
            long_ema = new List<double>(32768);
            shortDay = new List<double>(512);
            longDay = new List<double>(512);
            act = new Action(() => info.Log(reaction));

            Send += Analysis;

            foreach (string rd in new Daily())
            {
                string[] arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(reaction, arr[0], double.Parse(arr[1])));
            }
            foreach (string rd in new Tick())
            {
                string[] arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(reaction, arr[0], double.Parse(arr[1]), int.Parse(arr[2])));
            }
            act.BeginInvoke(act.EndInvoke, null);
        }
        public Statistics()
        {
            b = new BollingerBands(20, 2);
            ema = new EMA(5, 60);
            sma = new double[b.MidPeriod];
            trend_width = new List<double>(32768);
            short_ema = new List<double>(32768);
            long_ema = new List<double>(32768);
            shortDay = new List<double>(512);
            longDay = new List<double>(512);

            Send += Analysis;

            foreach (string rd in new Daily())
            {
                string[] arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(arr[0], double.Parse(arr[1])));
            }
            foreach (string rd in new Tick())
            {
                string[] arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(arr[0], double.Parse(arr[1]), int.Parse(arr[2])));
            }
            Send -= Analysis;

            api = Futures.Get();

            api.Send += Analysis;
        }
        public event EventHandler<Datum> Send;

        private void Analysis(object sender, Datum e)
        {
            MakeMA(e.Check, e.Price);

            int quantity, sc = short_ema.Count, lc = long_ema.Count, wc = trend_width.Count, trend = Analysis(e.Time, e.Price);
            double bo = 0, up = 0, ma = 0, sd;

            if (count > b.MidPeriod - 1)
            {
                ma = b.MovingAverage(b.MidPeriod, sma);
                sd = b.StandardDeviation(b.MidPeriod, ma, sma);
                up = b.UpperLimit(ma, sd);
                bo = b.BottomLine(ma, sd);
            }
            if (e.Check == false)
            {
                short_ema[sc - 1] = ema.Make(ema.ShortPeriod, sc, e.Price, sc > 1 ? short_ema[sc - 2] : 0);
                long_ema[lc - 1] = ema.Make(ema.LongPeriod, lc, e.Price, lc > 1 ? long_ema[lc - 2] : 0);
                trend_width[wc - 1] = b.Width(ma, up, bo);
            }
            else
            {
                if (sc > 0)
                {
                    short_ema.Add(ema.Make(ema.ShortPeriod, sc, e.Price, sc > 0 ? short_ema[sc - 1] : 0));
                    long_ema.Add(ema.Make(ema.LongPeriod, lc, e.Price, lc > 0 ? long_ema[lc - 1] : 0));
                }
                else
                {
                    short_ema.Add(ema.Make(e.Price));
                    long_ema.Add(ema.Make(e.Price));
                }
                trend_width.Add(b.Width(ma, up, bo));
            }
            if (api != null)
            {
                if (e.Volume > secret || e.Volume < -secret)
                {
                    quantity = Order(sc > 1 ? Trend() : 0, wc > b.MidPeriod ? TrendWidth(trend_width.Count) : 0, trend);

                    if (Math.Abs(e.Volume) < Math.Abs(e.Volume + quantity) && Math.Abs(api.Quantity + quantity) < (int)(basicAsset / (e.Price * tm * margin)))
                        api.OnReceiveOrder(dic[quantity > 0 ? 1 : -1]);

                    return;
                }
                if (api.Remaining == null)
                    api.RemainingDay();

                if (e.Time.Equals("154458") || e.Time.Equals("154459") || e.Time.Equals("154500") || e.Time.Equals("154454") || e.Time.Equals("154455") || e.Time.Equals("154456") || e.Time.Equals("154457") || (e.Time.Equals("151957") || e.Time.Equals("151958") || e.Time.Equals("151959") || e.Time.Equals("152000")) && api.Remaining.Equals("1"))
                    for (quantity = Math.Abs(api.Quantity); quantity > 0; quantity--)
                        api.OnReceiveOrder(dic[api.Quantity > 0 ? -1 : 1]);
            }
            else if (e.Reaction > 0)
            {
                if (e.Time.Length > 2 && e.Time.Substring(6, 4).Equals("1545") || Array.Exists(info.Remaining, o => o.Equals(e.Time)))
                {
                    while (info.Quantity != 0)
                        info.Operate(e.Price, info.Quantity > 0 ? -1 : 1);

                    info.Save(e.Time);
                }
                else if (e.Volume > e.Reaction || e.Volume < -e.Reaction)
                {
                    quantity = Order(sc > 1 ? Trend() : 0, wc > b.MidPeriod ? TrendWidth(trend_width.Count) : 0, trend);

                    if (Math.Abs(e.Volume) < Math.Abs(e.Volume + quantity) && Math.Abs(info.Quantity + quantity) < (int)(basicAsset / (e.Price * tm * margin)))
                        info.Operate(e.Price, quantity > 0 ? 1 : -1);
                }
            }
        }
        private int Analysis(string time, double price)
        {
            bool check = time.Length == 6 && !time.Equals("090000") ? false : time.Length == 2 ? true : Confirm(time.Substring(0, 6));
            int sc = shortDay.Count, lc = longDay.Count;

            if (check == false)
            {
                shortDay[sc - 1] = ema.Make(ema.ShortPeriod, sc, price, sc > 1 ? shortDay[sc - 2] : 0);
                longDay[lc - 1] = ema.Make(ema.LongPeriod, lc, price, lc > 1 ? longDay[lc - 2] : 0);

                return shortDay[sc - 1] - longDay[lc - 1] - (shortDay[sc - 2] - longDay[lc - 2]) > 0 ? 1 : -1;
            }
            if (check)
            {
                if (sc > 0)
                {
                    shortDay.Add(ema.Make(ema.ShortPeriod, sc, price, sc > 0 ? shortDay[sc - 1] : 0));
                    longDay.Add(ema.Make(ema.LongPeriod, lc, price, lc > 0 ? longDay[lc - 1] : 0));

                    sc = shortDay.Count;
                    lc = longDay.Count;

                    return shortDay[sc - 1] - longDay[lc - 1] - (shortDay[sc - 2] - longDay[lc - 2]) > 0 ? 1 : -1;
                }
                else
                {
                    shortDay.Add(ema.Make(price));
                    longDay.Add(ema.Make(price));
                }
            }
            sc = shortDay.Count;
            lc = longDay.Count;

            return sc > 1 ? shortDay[sc - 1] - longDay[lc - 1] - (shortDay[sc - 2] - longDay[lc - 2]) > 0 ? 1 : -1 : 0;
        }
        private int Order(double eg, double wg, int trend)
        {
            if (wg != 0 && !double.IsNaN(wg))
                return eg > 0 ? 1 + trend : -1 + trend;

            return 0;
        }
        private double TrendWidth(int wc)
        {
            return trend_width[wc - 1] - trend_width[wc - 2];
        }
        private void MakeMA(bool check, double price)
        {
            if (check == true)
                count++;

            sma[Count] = price;
        }
        private double Trend()
        {
            int sc = short_ema.Count, lc = long_ema.Count;

            return short_ema[sc - 1] - long_ema[lc - 1] - (short_ema[sc - 2] - long_ema[lc - 2]);
        }
        private bool Confirm(string date)
        {
            if (date.Equals(Register))
                return false;

            Register = date;

            return true;
        }
        private int Count
        {
            get
            {
                return count % b.MidPeriod;
            }
        }
        private static string Register
        {
            get; set;
        }
        private readonly Dictionary<int, string> dic = new Dictionary<int, string>()
        {
            {-1, "1"},
            {1, "2"},
        };
        private readonly Action act;
        private readonly Information info;
        private readonly Futures api;
        private readonly BollingerBands b;
        private readonly EMA ema;
        private readonly List<double> trend_width;
        private readonly List<double> short_ema;
        private readonly List<double> long_ema;
        private readonly List<double> shortDay;
        private readonly List<double> longDay;
        private readonly double[] sma;

        private int count = -1;
    }
}