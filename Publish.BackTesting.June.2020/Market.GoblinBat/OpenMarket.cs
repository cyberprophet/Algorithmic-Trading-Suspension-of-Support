using System;

namespace ShareInvest.Market
{
    public class OpenMarket : EventArgs
    {
        public int Open
        {
            get; private set;
        }
        public OpenMarket(int open)
        {
            Open = open;
        }
    }
}