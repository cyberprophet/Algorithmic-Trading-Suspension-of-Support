using System;

namespace ShareInvest.Communication
{
    public class Datum : EventArgs
    {
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
        public Datum(string date, double price)
        {
            Time = date.Substring(6, 2);
            Price = price;
        }
        public Datum(string date, double price, int volume)
        {
            Time = date;
            Price = price;
            Volume = volume;
        }
    }
}