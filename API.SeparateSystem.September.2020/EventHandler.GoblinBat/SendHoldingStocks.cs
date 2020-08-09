using System;
using System.Drawing;

namespace ShareInvest.EventHandler
{
    public class SendHoldingStocks : EventArgs
    {
        public string Time
        {
            get; private set;
        }
        public string Code
        {
            get; private set;
        }
        public int Quantity
        {
            get; private set;
        }
        public dynamic Purchase
        {
            get; private set;
        }
        public dynamic Current
        {
            get; private set;
        }
        public long Revenue
        {
            get; private set;
        }
        public double Rate
        {
            get; private set;
        }
        public double Base
        {
            get; private set;
        }
        public double Secondary
        {
            get; private set;
        }
        public Color Color
        {
            get; private set;
        }
        public object Strategics
        {
            get; private set;
        }
        public SendHoldingStocks(string date, int price, double sShort, double sLong, double trend, long revenue, long quantity)
        {
            Time = date.Substring(0, 10);
            Current = price;
            Base = sShort;
            Secondary = sLong;
            Rate = trend;
            Revenue = revenue;
            Strategics = quantity;
        }
        public SendHoldingStocks(string code, int quantity, dynamic purchase, dynamic current, long revenue, double rate, double basic, double secondary, Color color)
        {
            Code = code;
            Quantity = quantity;
            Purchase = purchase;
            Current = current;
            Revenue = revenue;
            Rate = rate;
            Base = basic;
            Secondary = secondary;
            Color = color;
        }
        public SendHoldingStocks(Catalog.TrendFollowingBasicFutures tf) => Strategics = tf;
        public SendHoldingStocks(Catalog.TrendsInStockPrices ts) => Strategics = ts;
        public SendHoldingStocks(Size size) => Strategics = size;
    }
}