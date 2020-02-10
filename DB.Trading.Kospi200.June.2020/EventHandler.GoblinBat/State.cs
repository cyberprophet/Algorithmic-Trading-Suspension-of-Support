using System;

namespace ShareInvest.EventHandler
{
    public class State : EventArgs
    {
        public string OnReceive
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
        public State(int receive, int count, int quantity)
        {
            OnReceive = receive.ToString();
            OrderCount = count.ToString();
            Quantity = quantity.ToString();
        }
    }
}