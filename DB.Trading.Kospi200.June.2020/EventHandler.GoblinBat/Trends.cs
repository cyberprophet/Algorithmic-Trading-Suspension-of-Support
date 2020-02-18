using System;
using System.Collections.Generic;

namespace ShareInvest.EventHandler
{
    public class Trends : EventArgs
    {
        public Dictionary<string, string> Trend
        {
            get; private set;
        }
        public Trends(Dictionary<string, string> trend)
        {
            Trend = trend;
        }
    }
}