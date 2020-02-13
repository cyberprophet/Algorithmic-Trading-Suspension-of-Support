using System;

namespace ShareInvest.EventHandler
{
    public class State : EventArgs
    {
        public bool OnReceive
        {
            get; private set;
        }
        public string OrderCount
        {
            get; private set;
        }
        public string Quantity
        {
            get; private set;
        }
        public State(bool receive, int count, int quantity)
        {
            OnReceive = receive;
            OrderCount = count.ToString();
            Quantity = quantity.ToString();
        }
    }
}