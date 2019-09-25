using System;
using System.Collections;
using System.Collections.Generic;
using ShareInvest.DataMining;
using ShareInvest.EventHandler;
using ShareInvest.Secondary;

namespace ShareInvest.BackTest
{
    public class Scalping : Save
    {
        private IEnumerable ie;
        private Situation s;

        public event EventHandler<DayEvent> SendDay;
        public event EventHandler<TickEvent> SendTick;

        public void StartProgress(int param)
        {
            ie = new EnumerateDay();

            foreach (string val in ie)
            {
                string[] arr = val.Split(',');

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                double price = double.Parse(arr[1]);

                s = new MakeDay(arr[0].Substring(6, 2));

                SendDay?.Invoke(this, new DayEvent(s.Check, price));
            }
            ie = new EnumerateTick();
            act = new Action(() => Log(param));

            foreach (string val in ie)
            {
                string[] arr = val.Split(',');

                s = new TimeCheck();

                if (arr[1].Contains("-"))
                    arr[1] = arr[1].Substring(1);

                double price = double.Parse(arr[1]);

                SendTick?.Invoke(this, new TickEvent(s.Make(arr[0].Substring(9, 1)), arr[0], price, int.Parse(arr[2]), param));

                s = new MakeDay(arr[0].Substring(4, 2));

                SendDay?.Invoke(this, new DayEvent(s.Check, price));
            }
            act.BeginInvoke(act.EndInvoke, null);
        }
        protected const int tm = 250000;
        protected const char separator = '\\';
        protected const double commission = 3e-5;
        protected const double margin = 7.5e-2;

        protected double[] sma;
        protected List<double> trend_width;
        protected List<double> short_ema;
        protected List<double> long_ema;
        protected List<double> short_macd;
        protected List<double> long_macd;
        protected List<double> signal_macd;
        protected BollingerBands b;
        protected Indicator i;

        protected double PurchasePrice
        {
            get
            {
                return purchase_price;
            }
            set
            {
                if (Math.Abs(amount) < Math.Abs(Quantity) && Quantity != 0)
                {
                    purchase_price = (purchase_price * Math.Abs(amount) + value) / Math.Abs(Quantity);
                    amount = Quantity;
                }
            }
        }
        protected double CumulativeRevenue
        {
            get; set;
        }
        protected double Liquidation
        {
            get
            {
                return liquidation;
            }
            set
            {
                if (amount > Quantity && Quantity > -1)
                {
                    liquidation = value - purchase_price;
                    amount = Quantity;
                    CumulativeRevenue += Math.Round(liquidation * tm);
                }
                else if (amount < Quantity && Quantity < 1)
                {
                    liquidation = purchase_price - value;
                    amount = Quantity;
                    CumulativeRevenue += Math.Round(liquidation * tm);
                }
            }
        }
        protected static int TodayTrend
        {
            get; set;
        }
        protected long Revenue
        {
            get
            {
                return revenue;
            }
            set
            {
                revenue = value - Commission;
            }
        }
        protected long TodayRevenue
        {
            get; set;
        }
        protected uint Commission
        {
            get; set;
        }
        protected ulong BasicAsset
        {
            get; set;
        }
        protected int Quantity
        {
            get; set;
        }
        protected uint TodayCommission
        {
            get; set;
        }
        protected uint TempCommission
        {
            get; set;
        }
        protected int Short
        {
            get; private set;
        }
        protected int Long
        {
            get; private set;
        }
        protected int Signal
        {
            get; private set;
        }
        private int amount;
        private long revenue;
        private double purchase_price;
        private double liquidation;

        public Scalping()
        {
            Short = 12;
            Long = 26;
            Signal = 9;
        }
    }
}