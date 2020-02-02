using System;
using ShareInvest.Interface.Struct;

namespace ShareInvest.EventHandler
{
    public class Datum : EventArgs
    {
        public Chart Chart
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
        public Datum(Chart chart)
        {
            Chart = chart;
        }
        public Datum(string[] arr)
        {
            Time = arr[0];
            Volume = int.Parse(arr[6]);
            Price = double.Parse(arr[1].Contains("-") ? arr[1].Substring(1) : arr[1]);
        }
    }
}