using System;
using System.Collections.Generic;
using ShareInvest.Chart;
using ShareInvest.EventHandler;
using ShareInvest.Secondary;
using ShareInvest.Secret;

namespace ShareInvest.Analysis
{
    public class Statistics : Conceal
    {
        public Statistics()
        {
            b = new BollingerBands(20, 2);
            ema = new EMA(5, 60);
            sma = new double[b.MidPeriod];
            trend_width = new List<double>(16384);
            short_ema = new List<double>(16384);
            long_ema = new List<double>(16384);

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

            int repeat = 0, quantity, sc = short_ema.Count, lc = long_ema.Count, wc = trend_width.Count;
            double bo = 0, up = 0, ma = 0, sd, gap, width_gap;

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
                if (sc != 0)
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
            gap = sc > 1 ? Trend() : 0;
            width_gap = wc > b.MidPeriod ? TrendWidth(trend_width.Count) : 0;

            if (api != null)
            {
                if (api.Remaining == null)
                    api.RemainingDay();

                if (e.Time.Equals("154458") || e.Time.Equals("154459") || e.Time.Equals("154500") || e.Time.Equals("154454") || e.Time.Equals("154455") || e.Time.Equals("154456") || e.Time.Equals("154457") || (e.Time.Equals("151957") || e.Time.Equals("151958") || e.Time.Equals("151959") || e.Time.Equals("152000")) && api.Remaining.Equals("1"))
                {
                    while (api.Quantity != 0)
                        Order(Operate(api.Quantity > 0 ? -1 : 1));

                    return;
                }
                if (e.Volume > secret || e.Volume < -secret)
                {
                    quantity = Order(gap, width_gap);

                    if (Math.Abs(e.Volume) < Math.Abs(e.Volume + quantity))
                    {
                        MaximumQuantity = (int)(basicAsset / (e.Price * tm * margin));

                        while (Math.Abs(api.Quantity + quantity) < MaximumQuantity)
                            repeat += Operate(quantity);
                    }
                    else
                        while (api.Quantity != 0 && (api.Quantity > 0 && e.Volume < -secret && api.PurchasePrice >= e.Price || api.Quantity < 0 && e.Volume > secret && api.PurchasePrice <= e.Price))
                            repeat += Operate(e.Volume < 0 ? -1 : 1);

                    Order(repeat);

                    return;
                }
                Order(Operate(DetermineProfit(e.Price, e.Volume, secret)));
            }
        }
        private int Operate(int quantity)
        {
            api.Quantity += quantity;

            return quantity;
        }
        private void Order(int repeat)
        {
            if (repeat != 0)
            {
                if (repeat > MaximumQuantity || repeat < -MaximumQuantity)
                {
                    api.OnReceiveOrder(ScreenNo, dic[repeat < 0 ? -1 : 1], MaximumQuantity);

                    Order(repeat > 0 ? repeat - MaximumQuantity : repeat + MaximumQuantity);

                    return;
                }
                api.OnReceiveOrder(ScreenNo, dic[repeat < 0 ? -1 : 1], repeat);
            }
        }
        private int DetermineProfit(double price, int volume, int secret)
        {
            if (api.Quantity > 0 && api.PurchasePrice + 1.5e-1 < price)
                return volume < -secret / 3 ? -1 : 0;

            if (api.Quantity < 0 && api.PurchasePrice - 1.5e-1 > price)
                return volume > secret / 3 ? 1 : 0;

            if (api.Quantity > 0 && api.PurchaseQuantity == true && volume < -secret / 3)
                return -1;

            if (api.Quantity < 0 && api.SellQuantity == true && volume > secret / 3)
                return 1;

            return 0;
        }
        private int Order(double eg, double wg)
        {
            if (wg != 0 && !double.IsNaN(wg))
                return eg > 0 ? 1 : -1;

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
        private int Count
        {
            get
            {
                return count % b.MidPeriod;
            }
        }
        private int MaximumQuantity
        {
            get; set;
        }
        private string ScreenNo
        {
            get
            {
                return (screen++ % 20 + 1000).ToString();
            }
        }
        private readonly Dictionary<int, string> dic = new Dictionary<int, string>()
        {
            {-1, "1"},
            {1, "2"},
        };
        private readonly Futures api;
        private readonly BollingerBands b;
        private readonly EMA ema;
        private readonly List<double> trend_width;
        private readonly List<double> short_ema;
        private readonly List<double> long_ema;
        private readonly double[] sma;

        private int count = -1;
        private int screen;
    }
}