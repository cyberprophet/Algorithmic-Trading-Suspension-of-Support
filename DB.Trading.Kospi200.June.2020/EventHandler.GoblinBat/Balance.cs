using System;

namespace ShareInvest.EventHandler
{
    public class Balance : EventArgs
    {
        public string[] Hold
        {
            get; private set;
        }
        public Balance(string[] hold)
        {
            Hold = hold;
        }
    }
}