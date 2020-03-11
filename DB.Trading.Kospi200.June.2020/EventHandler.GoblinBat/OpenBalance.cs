using System;

namespace ShareInvest.EventHandler
{
    public class OpenBalance : EventArgs
    {
        public string[] Hold
        {
            get; private set;
        }
        public OpenBalance(string[] hold)
        {
            Hold = hold;
        }
    }
}