using System;

namespace ShareInvest.EventHandler
{
    public class Deposit : EventArgs
    {
        public Deposit(string[] param)
        {
            DetailDeposit = param;
        }
        public string[] DetailDeposit
        {
            get; private set;
        }
    }
}