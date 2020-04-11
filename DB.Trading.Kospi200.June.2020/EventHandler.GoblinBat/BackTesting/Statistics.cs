using System;
using ShareInvest.Catalog.XingAPI;

namespace ShareInvest.EventHandler.BackTesting
{
    public class Statistics : EventArgs
    {
        public Specify[] Specify
        {
            get; private set;
        }
        public Statistics(Specify[] specifies)
        {
            Specify = specifies;
        }
    }
}