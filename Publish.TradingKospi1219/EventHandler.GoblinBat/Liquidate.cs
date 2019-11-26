using System;
using ShareInvest.Interface;

namespace ShareInvest.EventHandler
{
    public class Liquidate : EventArgs
    {
        public IStrategy EnCash
        {
            get; private set;
        }
        public Liquidate(IStrategy liquidate)
        {
            EnCash = liquidate;
        }
    }
}