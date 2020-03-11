using System;
using System.Collections.Generic;

namespace ShareInvest.EventHandler
{
    public class OpenTrends : EventArgs
    {
        public string Volume
        {
            get; private set;
        }
        public Dictionary<string, string> Trend
        {
            get; private set;
        }
        public OpenTrends(Dictionary<string, string> trend, int volume)
        {
            Trend = trend;
            Volume = volume.ToString("N0");
        }
    }
}