using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ShareInvest.AutoMessageBox;
using ShareInvest.Communicate;

namespace ShareInvest.BackTest
{
    public class Information
    {
        public void Operate(double price, int quantity)
        {
            if (quantity != 0)
            {
                Quantity += quantity;
                Commission += (int)((quantity > 0 ? price + st.ErrorRate : price - st.ErrorRate) * st.TransactionMultiplier * st.Commission);
                Liquidation = price;
                PurchasePrice = quantity > 0 ? price + st.ErrorRate : price - st.ErrorRate;
                Amount = Quantity;
                CumulativeRevenue += (int)(Liquidation * st.TransactionMultiplier);
            }
        }
        public void Save(string time)
        {
            Revenue = CumulativeRevenue - Commission;
            TodayCommission = Commission - TempCommission;
            list.Add(string.Concat(DateTime.ParseExact(time.Substring(0, 6), "yyMMdd", CultureInfo.CurrentCulture).ToString("yy-MM-dd"), ',', TodayCommission, ',', Revenue - TodayRevenue, ',', CumulativeRevenue - Commission));
            TempCommission = Commission;
            TodayRevenue = Revenue;
        }
        public void Log()
        {
            try
            {
                di = new DirectoryInfo(st.PathLog);

                if (di.Exists == false)
                    di.Create();

                using (sw = new StreamWriter(string.Concat(st.PathLog, st.Strategy, ".csv")))
                {
                    foreach (string val in list)
                        if (val.Length > 0)
                            sw.WriteLine(val);
                }
                list.Clear();
            }
            catch (Exception ex)
            {
                Box.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", 3750);
                Environment.Exit(0);
            }
        }
        public Information(IStrategy st)
        {
            this.st = st;
        }
        public string[] Kosdaq
        {
            get; private set;
        } =
            {
                "190911151957"
            };
        public string[] Kospi
        {
            get; private set;
        } =
            {
                "190911151959"
            };
        public int Quantity
        {
            get; private set;
        }
        public double PurchasePrice
        {
            get
            {
                return purchase;
            }
            private set
            {
                if (Math.Abs(Amount) < Math.Abs(Quantity) && Quantity != 0)
                {
                    purchase = (purchase * Math.Abs(Amount) + value) / Math.Abs(Quantity);
                    Amount = Quantity;
                }
                else if (Quantity == 0)
                    purchase = 0;
            }
        }
        private double Liquidation
        {
            get
            {
                return liquidation;
            }
            set
            {
                if (Amount > Quantity && Quantity > -1)
                    liquidation = value - PurchasePrice;

                else if (Amount < Quantity && Quantity < 1)
                    liquidation = PurchasePrice - value;

                else
                    liquidation = 0;
            }
        }
        private long CumulativeRevenue
        {
            get; set;
        }
        private long Revenue
        {
            get; set;
        }
        private long TodayRevenue
        {
            get; set;
        }
        private int Amount
        {
            get; set;
        }
        private int Commission
        {
            get; set;
        }
        private int TodayCommission
        {
            get; set;
        }
        private int TempCommission
        {
            get; set;
        }
        private readonly List<string> list = new List<string>(128);
        private readonly IStrategy st;
        private DirectoryInfo di;
        private StreamWriter sw;
        private double purchase;
        private double liquidation;
    }
}