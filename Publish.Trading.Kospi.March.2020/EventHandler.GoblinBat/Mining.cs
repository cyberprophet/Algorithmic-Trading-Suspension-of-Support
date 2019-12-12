using System;

namespace ShareInvest.EventHandler
{
    public class Mining : EventArgs
    {
        public int Tab
        {
            get; private set;
        }
        public Mining(int tab)
        {
            Tab = tab;
        }
    }
}