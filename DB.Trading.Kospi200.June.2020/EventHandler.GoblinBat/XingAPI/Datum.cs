using System;

namespace ShareInvest.EventHandler.XingAPI
{
    public class Datum : EventArgs
    {
        public Datum(string time, double price, int volume)
        {
            Time = time;
            Volume = volume;
            Price = price;
        }
        public Datum(string time, string price)
        {
            Time = time;

            if (double.TryParse(price, out double current))
                Price = current;
        }
        public string Time
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
    }
}