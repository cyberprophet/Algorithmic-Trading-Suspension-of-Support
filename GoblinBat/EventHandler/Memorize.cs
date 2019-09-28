using System;
using System.Text;

namespace ShareInvest.EventHandler
{
    public class Memorize : EventArgs
    {
        public string date;
        public string price;
        public string volume;
        public string sPrevNext;

        public Memorize(StringBuilder sb)
        {
            string[] arr = sb.ToString().Split(',');

            date = arr[2].Substring(2);
            price = arr[0];
            volume = arr[1];
        }
        public Memorize(string sPrevNext)
        {
            this.sPrevNext = sPrevNext;
        }
    }
}