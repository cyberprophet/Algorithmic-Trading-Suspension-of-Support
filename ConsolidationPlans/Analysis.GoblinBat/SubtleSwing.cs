using System;
using System.Collections.Generic;
using ShareInvest.BackTest;
using ShareInvest.Chart;
using ShareInvest.EventHandler;
using ShareInvest.SecondaryIndicators;
using ShareInvest.Secret;

namespace ShareInvest.Analysis
{
    public class SubtleSwing : Conceal
    {
        public SubtleSwing(int reaction, int type)
        {
            this.type = type;
            info = new Information(type);
            b = new BollingerBands(20, 2);
            ema = new EMA(5, 60);
            sma = new double[b.MidPeriod];
            open = new double[3];
            trend_width = new List<double>(32768);
            short_ema = new List<double>(32768);
            long_ema = new List<double>(32768);
            shortDay = new List<double>(512);
            longDay = new List<double>(512);
            act = new Action(() => info.Log(reaction));
            Send += Analysis;

            foreach (string rd in new Daily(type))
            {
                arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(reaction, arr[0], double.Parse(arr[1])));
            }
            foreach (string rd in new Tick(type))
            {
                arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(reaction, arr[0], double.Parse(arr[1]), int.Parse(arr[2])));
            }
            act.BeginInvoke(act.EndInvoke, null);
        }
        public SubtleSwing(int type)
        {
            this.type = type;
            b = new BollingerBands(20, 2);
            ema = new EMA(5, 60);
            sma = new double[b.MidPeriod];
            trend_width = new List<double>(32768);
            short_ema = new List<double>(32768);
            long_ema = new List<double>(32768);
            shortDay = new List<double>(512);
            longDay = new List<double>(512);
            Send += Analysis;

            foreach (string rd in new Daily(type))
            {
                arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(arr[0], double.Parse(arr[1])));
            }
            foreach (string rd in new Tick(type))
            {
                arr = rd.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                Send?.Invoke(this, new Datum(arr[0], double.Parse(arr[1]), int.Parse(arr[2])));
            }
            Send -= Analysis;
            arr = SetSecret(type).Split('^');
            Secret = int.Parse(arr[0]);
            api = Futures.Get();
            api.Send += Analysis;
        }
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
                if (e.Volume > Secret || e.Volume < -Secret)
                {
                    quantity = Order(sc > 1 ? Trend() : 0, wc > b.MidPeriod ? TrendWidth(trend_width.Count) : 0, trend);

                    if (Math.Abs(e.Volume) < Math.Abs(e.Volume + quantity) && Math.Abs(api.Quantity + quantity) < (int)(basicAsset / (e.Price * (type > 0 ? ktm * kqm : tm * margin))))
                        api.OnReceiveOrder(dic[quantity]);

                    return;
                }
                if (api.Remaining == null)
                    api.RemainingDay();

                Time = int.Parse(e.Time);

                if (After == false && Time > 154450)
                {
                    After = true;

                    for (quantity = Math.Abs(api.Quantity); quantity > 0; quantity--)
                        api.OnReceiveOrder(dic[api.Quantity > 0 ? -1 : 1]);

                    return;
                }
                if (api.Quantity != 0 && api.Remaining.Equals("1") && Time > 151945)
                    api.OnReceiveOrder(dic[api.Quantity > 0 ? -1 : 1]);
            }
            else if (e.Reaction > 0)
            {
                if (e.Time.Length > 2 && e.Time.Substring(6, 4).Equals("1545") || Array.Exists(type > 0 ? info.KosdaqRemaining : info.Remaining, o => o.Equals(e.Time)))
                {
                    while (info.Quantity != 0)
                        info.Operate(e.Price, info.Quantity > 0 ? -1 : 1);

                    info.Save(e.Time, open);
                }
                else if ((e.Volume > e.Reaction || e.Volume < -e.Reaction) && StopLoss(trend, e.Price))
                {
                    quantity = Order(sc > 1 ? Trend() : 0, wc > b.MidPeriod ? TrendWidth(trend_width.Count) : 0, trend);

                    if (Math.Abs(e.Volume) < Math.Abs(e.Volume + quantity) && Math.Abs(info.Quantity + quantity) < (int)(basicAsset / (e.Price * (type > 0 ? ktm * kqm : tm * margin))))
                        info.Operate(e.Price, quantity);
                }
            }
        }
        private int Analysis(string time, double price)
        {
            bool check = time.Length == 6 && !time.Equals("090000") ? false : time.Length == 2 ? true : Confirm(time.Substring(0, 6), price);
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
                    open[1] = shortDay[sc - 1];
                    open[2] = longDay[lc - 1];

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

            return lc > 1 ? shortDay[sc - 1] - longDay[lc - 1] - (shortDay[sc - 2] - longDay[lc - 2]) > 0 ? 1 : -1 : 0;
        }
        private bool StopLoss(int trend, double price)
        {
            if (trend > 0 && open[0] > price || trend < 0 && open[0] < price)
            {
                if (info.Quantity != 0)
                    info.Operate(price, info.Quantity > 0 ? -1 : 1);

                return false;
            }
            return true;
        }
        private int Order(double eg, double wg, int trend)
        {
            if (wg != 0 && !double.IsNaN(wg))
                return eg > 0 && trend > 0 ? 1 : eg < 0 && trend < 0 ? -1 : 0;

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
        private bool Confirm(string date, double price)
        {
            if (date.Equals(Register))
                return false;

            Register = date;
            open[0] = price;

            return true;
        }
        private int Time
        {
            get; set;
        }
        private int Count
        {
            get
            {
                return count % b.MidPeriod;
            }
        }
        private bool After
        {
            get; set;
        } = false;
        private static string Register
        {
            get; set;
        }
        private static int Secret
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
        private readonly string[] arr;
        private readonly double[] sma;
        private readonly double[] open;
        private readonly int type;
        private const int basicAsset = 35000000;
        private int count = -1;
        public event EventHandler<Datum> Send;
    }
}