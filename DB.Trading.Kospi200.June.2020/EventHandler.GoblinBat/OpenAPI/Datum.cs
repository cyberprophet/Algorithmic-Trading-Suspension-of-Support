using System;

using ShareInvest.Catalog;

namespace ShareInvest.EventHandler.OpenAPI
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
        public Datum(string[] arr)
        {
            if (int.TryParse(arr[6], out int volume) && double.TryParse(arr[1].Contains("-") ? arr[1].Substring(1) : arr[1], out double price))
            {
                Volume = volume;
                Price = price;
            }
            Time = arr[0];
        }
        public Datum(Chart chart) => Chart = chart;
    }
}