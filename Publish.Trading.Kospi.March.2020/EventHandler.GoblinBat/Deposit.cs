using System;
using System.Text;

namespace ShareInvest.EventHandler
{
    public class Deposit : EventArgs
    {
        public Deposit(StringBuilder sb)
        {
            ArrayDeposit = sb.ToString().Split(';');
        }
        public string[] ArrayDeposit
        {
            get; private set;
        }
    }
}