using System;
using System.Text;

namespace ShareInvest.EventHandler
{
    public class Datum : EventArgs
    {
        private readonly string[] arr;

        public bool check;
        public string time;
        public double price;
        public int volume;

        public Datum(string check, double price)
        {
            this.check = Confirm(check.Substring(6, 2));
            this.price = price;
        }
        public Datum(string time, double price, int volume)
        {
            check = Confirm(time.Substring(9, 1));
            this.time = time;
            this.price = price;
            this.volume = volume;
        }
        public Datum(StringBuilder sb)
        {
            arr = sb.ToString().Split(',');

            if (arr[1].Contains("-"))
                arr[1] = arr[1].Substring(1);

            check = Confirm(arr[0].Substring(0, 4));
            time = arr[0];
            price = double.Parse(arr[1]);
            volume = int.Parse(arr[6]);
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
    }
}