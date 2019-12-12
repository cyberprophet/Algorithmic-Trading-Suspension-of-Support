using System;

namespace ShareInvest.EventHandler
{
    public class Holding : EventArgs
    {
        public Holding(string hold)
        {
            Hold = hold.Split('*');
        }
        public string[] Hold
        {
            get; private set;
        }
    }
}