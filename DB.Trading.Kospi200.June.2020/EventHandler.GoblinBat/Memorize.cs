using System;
using System.Text;

namespace ShareInvest.EventHandler
{
    public class Memorize : EventArgs
    {
        public string Date
        {
            get; private set;
        }
        public string Price
        {
            get; private set;
        }
        public string Volume
        {
            get; private set;
        }
        public string SPrevNext
        {
            get; private set;
        }
        public string Code
        {
            get; private set;
        }
        public Memorize(StringBuilder sb)
        {
            string[] arr = sb.ToString().Split(';');

            Date = arr[2].Substring(2);
            Price = arr[0];
            Volume = arr[1];
        }
        public Memorize(string sPrevNext, string code)
        {
            SPrevNext = sPrevNext;
            Code = code;
        }
        public Memorize(string[] arr)
        {
            Date = arr[0];
            Price = arr[1];

            if (arr.Length > 2)
                Volume = arr[2];
        }
    }
}