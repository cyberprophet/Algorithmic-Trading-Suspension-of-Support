using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ShareInvest.BackTest
{
    public class Information
    {
        public void Operate(double price, int quantity)
        {
            if (quantity != 0)
            {
                Quantity += quantity;
                Commission += (int)((type.Contains("Kospi200") ? (quantity > 0 ? price + 5e-2 : price - 5e-2) * tm : (quantity > 0 ? price + 1e-1 : price - 1e-1) * km) * commission);
                Liquidation = price;
                PurchasePrice = type.Contains("Kospi200") ? quantity > 0 ? price + 5e-2 : price - 5e-2 : quantity > 0 ? price + 1e-1 : price - 1e-1;
                Amount = Quantity;
                CumulativeRevenue += (int)(Liquidation * (type.Contains("Kospi200") ? tm : km));
            }
        }
        public void Save(string time)
        {
            Revenue = CumulativeRevenue - Commission;
            TodayCommission = Commission - TempCommission;

            if (TodayCommission != 0)
                list.Add(string.Concat(DateTime.ParseExact(time.Substring(0, 6), "yyMMdd", CultureInfo.CurrentCulture).ToString("yy-MM-dd"), ',', TodayCommission, ',', Revenue - TodayRevenue, ',', CumulativeRevenue - Commission));

            TempCommission = Commission;
            TodayRevenue = Revenue;
        }
        public void Log(int param)
        {
            string path = string.Concat(Environment.CurrentDirectory, type, DateTime.Now.ToString("yyMMdd"), @"\"), file = string.Concat(param, ".csv");

            try
            {
                di = new DirectoryInfo(path);

                if (di.Exists == false)
                    di.Create();

                using (sw = new StreamWriter(path + file))
                {
                    foreach (string val in list)
                        if (val.Length > 0)
                            sw.WriteLine(val);
                }
                list.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public Information(int type)
        {
            this.type = type > 0 ? @"\Log\Kosdaq150\" : @"\Log\Kospi200\";
        }
        public string[] Remaining
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
        private double PurchasePrice
        {
            get
            {
                return purchase;
            }
            set
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
        private readonly List<string> list = new List<string>(64);
        private readonly string type;
        private const int tm = 250000;
        private const int km = 10000;
        private const double commission = 3e-5;
        private DirectoryInfo di;
        private StreamWriter sw;
        private double purchase;
        private double liquidation;
    }
}