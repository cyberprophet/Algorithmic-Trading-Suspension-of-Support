using System;
using System.Collections.Generic;

namespace ShareInvest.Catalog
{
    public struct ConvertConsensus
    {
        public Tuple<List<ConvertConsensus>, List<ConvertConsensus>> PresumeToConsensus(IEnumerable<ConvertConsensus> consensus)
        {
            var quarterly = new List<ConvertConsensus>();
            var yearly = new List<ConvertConsensus>();
            var temp = string.Empty;

            foreach (var classification in consensus)
                if (classification.Date.Substring(0, 5).Equals(temp) == false)
                {
                    (classification.Quarter.Equals("0") ? yearly : quarterly).Add(classification);
                    temp = classification.Date.Substring(0, 5);
                }
            return new Tuple<List<ConvertConsensus>, List<ConvertConsensus>>(yearly, quarterly);
        }
        public string Code
        {
            get; set;
        }
        public string Date
        {
            get; set;
        }
        public string Quarter
        {
            get; set;
        }
        public long Sales
        {
            get; set;
        }
        public double YoY
        {
            get; set;
        }
        public long Op
        {
            get; set;
        }
        public long Np
        {
            get; set;
        }
        public int Eps
        {
            get; set;
        }
        public int Bps
        {
            get; set;
        }
        public double Per
        {
            get; set;
        }
        public double Pbr
        {
            get; set;
        }
        public string Roe
        {
            get; set;
        }
        public string Ev
        {
            get; set;
        }
    }
}