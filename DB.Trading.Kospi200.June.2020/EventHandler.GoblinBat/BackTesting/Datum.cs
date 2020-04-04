using System;

namespace ShareInvest.EventHandler.BackTesting
{
    public class Datum : EventArgs
    {
        public long Date
        {
            get; private set;
        }
        public double Price
        {
            get; private set;
        }
        public Datum(string date, string price)
        {
            if (long.TryParse(date, out long time) && double.TryParse(price, out double close))
            {
                Date = time;
                Price = close;
            }
        }
    }
}