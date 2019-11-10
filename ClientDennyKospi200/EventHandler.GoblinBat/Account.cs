using System;

namespace ShareInvest.EventHandler
{
    public class Account : EventArgs
    {
        public string[] AccountCategory
        {
            get; private set;
        }
        public string Selection
        {
            get; private set;
        }
        public Account(string account)
        {
            if (account.Contains("-"))
            {
                Selection = account;

                return;
            }
            AccountCategory = account.Split(';');
        }
    }
}