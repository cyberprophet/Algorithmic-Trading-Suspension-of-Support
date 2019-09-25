using System;
using System.Collections.Generic;
using System.Linq;
using ShareInvest.BackTest;
using ShareInvest.EventHandler;
using ShareInvest.Secondary;
using ShareInvest.Secret;

namespace ShareInvest.Strategy
{
    public class Tick : Scalping
    {
        public event EventHandler<SaveEvent> SendSave;

        private readonly IArcanum ia;
        private readonly string[] remaining =
        {
            "190911151959"
        };
        public Tick()
        {
            b = new BollingerBands();
            sma = new double[b.MidPeriod];
            trend_width = new List<double>(32768);
            short_ema = new List<double>(32768);
            long_ema = new List<double>(32768);
            ia = new Arcanum();

            SendSave += Storage;
        }
        public void Analysis(object sender, TickEvent e)
        {
            MakeMA(e.check, e.price);

            int quotient, repeat = 0, quantity, sc = short_ema.Count, lc = long_ema.Count, wc = trend_width.Count;
            double bo = 0, up = 0, ma = 0, sd, gap, width_gap, sb = sc > 1 ? short_ema[sc - 2] : 0, lb = lc > 1 ? long_ema[lc - 2] : 0, se = sc > 0 ? short_ema.Last() : 0, le = lc > 0 ? long_ema.Last() : 0;

            i = lc > 0 ? new EMA() : new Indicator();

            if (count > b.MidPeriod)
            {
                ma = b.MovingAverage(b.MidPeriod, sma);
                sd = b.StandardDeviation(b.MidPeriod, ma, sma);
                up = b.UpperLimit(ma, sd);
                bo = b.BottomLine(ma, sd);
            }
            if (e.check == false)
            {
                short_ema[sc - 1] = i.Make(i.ShortPeriod, sc, e.price, sb);
                long_ema[lc - 1] = i.Make(i.LongPeriod, lc, e.price, lb);
                trend_width[wc - 1] = b.Width(ma, up, bo);
            }
            else
            {
                short_ema.Add(i.Make(i.ShortPeriod, sc, e.price, se));
                long_ema.Add(i.Make(i.LongPeriod, lc, e.price, le));
                trend_width.Add(b.Width(ma, up, bo));
            }
            gap = sc > 1 ? Trend() : 0;
            width_gap = wc > b.MidPeriod ? TrendWidth() : 0;

            if (e.volume > e.secret || e.volume < -e.secret)
            {
                quantity = Order(gap, width_gap);

                if (Math.Abs(e.volume) < Math.Abs(e.volume + quantity) && !e.time.Substring(6, 4).Equals("1545"))
                {
                    ia.MaximumQuantity = ia.BasicAsset / (e.price * tm * margin);
                    quotient = ia.Devide(e.volume);

                    while (Math.Abs(Quantity + quantity) < ia.MaximumQuantity && Math.Abs(repeat) <= quotient)
                        repeat += Operate(e.price, quantity);
                }
            }
            if (e.time.Substring(6, 4).Equals("1545") || Array.Exists(remaining, o => o.Equals(e.time)))
            {
                while (Quantity != 0)
                    Operate(e.price, Quantity > 0 ? -1 : 1);

                Revenue = (long)CumulativeRevenue;
                TodayCommission = Commission - TempCommission;

                if (TodayCommission != 0)
                    SendSave?.Invoke(this, new SaveEvent(e.time.Substring(0, 6), TodayCommission, Revenue - TodayRevenue, CumulativeRevenue - Commission));

                TempCommission = Commission;
                TodayRevenue = Revenue;
            }
        }
        private int Operate(double price, int quantity)
        {
            Quantity += quantity;
            Commission += (uint)Math.Truncate(price * tm * commission);
            PurchasePrice = price;
            Liquidation = price;

            return quantity;
        }
        private int Order(double eg, double wg)
        {
            if (wg != 0 && !double.IsNaN(wg))
                return eg + wg > 0 ? 1 : -1;

            return 0;
        }
        private double TrendWidth()
        {
            int wc = trend_width.Count;

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
        private int count = -1;
    }
}