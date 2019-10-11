using System;

namespace ShareInvest.EventHandler
{
    public class Identify : EventArgs
    {
        public Identify(string confirm)
        {
            Confirm = confirm;
        }
        public string Confirm
        {
            get; private set;
        }
    }
}