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
        public int Volume
        {
            get; private set;
        }
        public Datum(string date, string price, string volume)
        {
            if (long.TryParse(date, out long time) && double.TryParse(price, out double close) && int.TryParse(volume, out int accumulate))
            {
                Date = time;
                Price = close;
                Volume = accumulate;
            }
        }
        public Datum(long date, double price, int volume)
        {
            Date = date;
            Price = price;
            Volume = volume;
        }
    }
}