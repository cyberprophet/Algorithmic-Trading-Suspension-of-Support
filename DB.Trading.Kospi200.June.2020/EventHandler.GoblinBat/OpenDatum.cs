using System;
using ShareInvest.Catalog;

namespace ShareInvest.EventHandler
{
    public class OpenDatum : EventArgs
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
        public OpenDatum(Chart chart)
        {
            Chart = chart;
        }
        public OpenDatum(string[] arr)
        {
            Time = arr[0];
            Volume = int.Parse(arr[6]);
            Price = double.Parse(arr[1].Contains("-") ? arr[1].Substring(1) : arr[1]);
        }
    }
}