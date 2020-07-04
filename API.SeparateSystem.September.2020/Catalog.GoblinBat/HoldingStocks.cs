using System;

using ShareInvest.EventHandler;

namespace ShareInvest.Catalog
{
    public class HoldingStocks
    {
        public string Code
        {
            get; set;
        }
        public int Quantity
        {
            get; set;
        }
        public dynamic Purchase
        {
            get; set;
        }
        public dynamic Current
        {
            get; set;
        }
        public long Revenue
        {
            get; set;
        }
        public double Rate
        {
            get; set;
        }
        public void OnReceiveEvent(string[] param)
        {
            if (int.TryParse(param[1], out int current))
            {
                Current = current;
                Revenue = (current - Purchase) * Quantity;
                Rate = current / (double)Purchase - 1;
            }
            Send?.Invoke(this, new SendHoldingStocks(Code, param[0], Quantity, Purchase, Current, Revenue, Rate));
        }
        public event EventHandler<SendHoldingStocks> Send;
    }
}