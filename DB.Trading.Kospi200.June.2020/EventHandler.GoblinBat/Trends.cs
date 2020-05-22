using System;
using System.Collections.Generic;

namespace ShareInvest.EventHandler
{
    public class Trends : EventArgs
    {
        public string Volume
        {
            get; private set;
        }
        public Dictionary<string, string> Trend
        {
            get; private set;
        }
        public Trends(Dictionary<string, string> trend, int volume)
        {
            Trend = trend;
            Volume = volume.ToString("N0");
        }
        public Trends(Dictionary<string, string> trend, string avg)
        {
            Trend = trend;
            Volume = avg;
        }
    }
}