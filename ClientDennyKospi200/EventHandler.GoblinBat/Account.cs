using System;

namespace ShareInvest.EventHandler
{
    public class Account : EventArgs
    {
        public string[] AccountCategory
        {
            get; private set;
        }
        public Account(string account)
        {
            AccountCategory = account.Split(';');
        }
    }
}