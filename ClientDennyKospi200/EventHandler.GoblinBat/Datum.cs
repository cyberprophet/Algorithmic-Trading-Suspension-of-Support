using System;
using System.Text;

namespace ShareInvest.EventHandler
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
            Time = date;
            Price = price;
        }
        public Datum(string date, double price, int volume)
        {
            Time = date;
            Price = price;
            Volume = volume;
        }
        public Datum(StringBuilder sb)
        {
            string[] arr = sb.ToString().Split(';');

            Time = arr[0];
            Volume = int.Parse(arr[6]);

            if (arr[1].Contains("-"))
                arr[1] = arr[1].Substring(1);

            Price = double.Parse(arr[1]);
        }
    }
}