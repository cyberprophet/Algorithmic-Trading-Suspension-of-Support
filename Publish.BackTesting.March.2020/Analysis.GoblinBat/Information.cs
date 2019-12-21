using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using ShareInvest.Communication;
using ShareInvest.Log.Message;
using ShareInvest.Options;

namespace ShareInvest.BackTesting.Analysis
{
    public class Information
    {
        public void Operate(double price, int quantity, string time)
        {
            if (quantity != 0)
            {
                if (st.Hedge > 0)
                    hedge.Operate(Quantity > 0 && quantity < 0 || Quantity < 0 && quantity > 0, time, price, quantity);

                Quantity += quantity;
                Commission += (int)((quantity > 0 ? price + st.ErrorRate : price - st.ErrorRate) * st.TransactionMultiplier * st.Commission);
                Liquidation = price;
                PurchasePrice = quantity > 0 ? price + st.ErrorRate : price - st.ErrorRate;
                Amount = Quantity;
                CumulativeRevenue += (long)(Liquidation * st.TransactionMultiplier);
            }
        }
        public void Save(string time, double price)
        {
            Revenue = CumulativeRevenue - Commission;
            list.Add(string.Concat(DateTime.ParseExact(time.Substring(0, 6), "yyMMdd", CultureInfo.CurrentCulture).ToString("yy-MM-dd"), ',', (long)(Quantity.Equals(0) ? 0 - hedge.OptionsRevenue : (Quantity > 0 ? price - PurchasePrice : PurchasePrice - price) * st.TransactionMultiplier * Math.Abs(Quantity) - hedge.OptionsRevenue), ',', Revenue - TodayRevenue, ',', CumulativeRevenue - Commission));
            TodayRevenue = Revenue;
        }
        public void Log()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(st.PathLog);

                if (di.Exists == false)
                    di.Create();

                using (StreamWriter sw = new StreamWriter(string.Concat(st.PathLog, @"\", st.Strategy, ".csv")))
                {
                    foreach (string val in list)
                        if (val.Length > 0)
                            sw.WriteLine(val);
                }
                list.Clear();
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Exception", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
        }
        public Information(IStrategy st)
        {
            this.st = st;
            hedge = new Hedge(st, new Dictionary<string, uint>());
        }
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
        private readonly List<string> list = new List<string>(128);
        private readonly Hedge hedge;
        private readonly IStrategy st;
        private double purchase;
        private double liquidation;
    }
}