using System;

namespace ShareInvest.EventHandler
{
    public class TickWidthGap : EventArgs
    {
        public double ShortLongGap
        {
            get; private set;
        }
        public double PriceShortGap
        {
            get; private set;
        }
        public double Price
        {
            get; set;
        }
        public TickWidthGap(double price, double slGap, double psGap)
        {
            ShortLongGap = slGap;
            PriceShortGap = psGap;
            Price = price;
        }
    }
}