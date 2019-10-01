using System;
using System.Text;

namespace ShareInvest.EventHandler
{
    public class Datum : EventArgs
    {
        public bool Check
        {
            get; private set;
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
        public Datum(string check, double price)
        {
            Check = Confirm(check.Substring(6, 2));
            Price = price;
        }
        public Datum(string time, double price, int volume)
        {
            Check = Confirm(time.Substring(9, 1));
            Time = time;
            Price = price;
            Volume = volume;
        }
        public Datum(StringBuilder sb)
        {
            arr = sb.ToString().Split(',');

            if (arr[1].Contains("-"))
                arr[1] = arr[1].Substring(1);

            Check = Confirm(arr[0].Substring(0, 4));
            Time = arr[0];
            Price = double.Parse(arr[1]);
            Volume = int.Parse(arr[6]);
        }
        public Datum(bool check, double price)
        {
            Check = check;
            Price = price;
        }
        private bool Confirm(string date)
        {
            if (date.Equals(Register))
                return false;

            Register = date;

            return true;
        }
        private static string Register
        {
            get; set;
        }
        private readonly string[] arr;
    }
}