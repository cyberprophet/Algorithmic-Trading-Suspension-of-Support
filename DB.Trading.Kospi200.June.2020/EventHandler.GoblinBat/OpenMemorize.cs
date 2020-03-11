using System;
using System.Text;

namespace ShareInvest.EventHandler
{
    public class OpenMemorize : EventArgs
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
        public OpenMemorize(StringBuilder sb)
        {
            string[] arr = sb.ToString().Split(';');
            bool days = arr[0].Equals(string.Empty) && arr[4].Length == 8;

            Date = days ? arr[4] : arr[2].Substring(2);
            Price = days ? arr[1] : arr[0];
            Volume = days ? arr[2] : arr[1];
        }
        public OpenMemorize(string sPrevNext, string code)
        {
            SPrevNext = sPrevNext;
            Code = code;
        }
        public OpenMemorize(string[] arr)
        {
            Date = arr[0];
            Price = arr[1];

            if (arr.Length > 2)
                Volume = arr[2];
        }
        public OpenMemorize(string clear)
        {
            SPrevNext = clear;
        }
    }
}