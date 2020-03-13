using System;

namespace ShareInvest.EventHandler.XingAPI
{
    public class Datum : EventArgs
    {
        public Datum(string time, string price, string volume)
        {
            Time = time;

            if (double.TryParse(price, out double p))
                Price = p;

            if (int.TryParse(volume, out int v))
                Volume = v;
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