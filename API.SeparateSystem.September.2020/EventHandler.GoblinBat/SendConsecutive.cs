using System;

using ShareInvest.Catalog;

namespace ShareInvest.EventHandler
{
    public class SendConsecutive : EventArgs
    {
        public string Date
        {
            get; private set;
        }
        public dynamic Price
        {
            get; private set;
        }
        public int Volume
        {
            get; private set;
        }
        public SendConsecutive(Charts chart)
        {
            if (chart.Price.Contains(".") && double.TryParse(chart.Price, out double fPrice))
                Price = fPrice;

            else if (int.TryParse(chart.Price, out int sPrice))
                Price = sPrice;

            Date = chart.Date;
            Volume = chart.Volume;
        }
        public SendConsecutive(string date, int price, int volume)
        {
            Date = date;
            Price = price;
            Volume = volume;
        }
    }
}