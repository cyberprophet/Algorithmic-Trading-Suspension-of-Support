using System;

namespace ShareInvest.EventHandler
{
    public class Deposit : EventArgs
    {
        public string[] DetailDeposit
        {
            get; private set;
        }
        public Deposit(string[] param) => DetailDeposit = param;
    }
}