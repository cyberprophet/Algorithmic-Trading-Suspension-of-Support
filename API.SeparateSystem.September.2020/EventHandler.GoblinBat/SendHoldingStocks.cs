using System;

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
        public SendHoldingStocks(string code, int quantity, dynamic purchase, dynamic current, long revenue, double rate)
        {
            Code = code;
            Quantity = quantity;
            Purchase = purchase;
            Current = current;
            Revenue = revenue;
            Rate = rate;
        }
    }
}