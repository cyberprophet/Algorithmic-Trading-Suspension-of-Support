using System;

namespace ShareInvest.EventHandler.BackTesting
{
    public class Quotes : EventArgs
    {
        public string Time
        {
            get; set;
        }
        public double SellPrice
        {
            get; set;
        }
        public double BuyPrice
        {
            get; set;
        }
        public int SellQuantity
        {
            get; set;
        }
        public int BuyQuantity
        {
            get; set;
        }
        public int SellAmount
        {
            get; set;
        }
        public int BuyAmount
        {
            get; set;
        }
        public Quotes(string time, string sp, string bp, string sq, string bq, string sa, string ba)
        {
            if (double.TryParse(sp, out double sell) && double.TryParse(bp, out double buy) && int.TryParse(sa, out int st) && int.TryParse(ba, out int bt) && int.TryParse(sq, out int sqty) && int.TryParse(bq, out int bqty))
            {
                SellPrice = sell;
                BuyPrice = buy;
                SellQuantity = sqty;
                BuyQuantity = bqty;
                SellAmount = st;
                BuyAmount = bt;
            }
            Time = time;
        }
    }
}