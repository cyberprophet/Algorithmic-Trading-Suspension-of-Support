using System;

namespace ShareInvest.EventHandler
{
    public class State : EventArgs
    {
        public bool OnReceive
        {
            get; private set;
        }
        public string SellOrderCount
        {
            get; private set;
        }
        public string Quantity
        {
            get; private set;
        }
        public string BuyOrderCount
        {
            get; private set;
        }
        public string ScreenNumber
        {
            get; private set;
        }
        public string Max
        {
            get; private set;
        }
        public State(bool receive, int sell, int quantity, int buy, uint number)
        {
            OnReceive = receive;
            SellOrderCount = sell.ToString();
            Quantity = quantity.ToString();
            BuyOrderCount = buy.ToString();
            ScreenNumber = number.ToString();
        }
        public State(bool receive, int sell, int quantity, int buy, string avg, double max)
        {
            OnReceive = receive;
            SellOrderCount = sell.ToString();
            Quantity = quantity.ToString();
            BuyOrderCount = buy.ToString();
            ScreenNumber = avg != null ? avg.ToString() : string.Empty;
            Max = max.ToString("F2");
        }
    }
}