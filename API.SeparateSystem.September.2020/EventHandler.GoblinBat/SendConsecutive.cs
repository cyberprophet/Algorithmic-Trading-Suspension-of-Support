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
        public string Price
        {
            get; private set;
        }
        public int Volume
        {
            get; private set;
        }
        public SendConsecutive(Charts chart)
        {
            Date = chart.Date;
            Price = chart.Price;
            Volume = chart.Volume;
        }
    }
}