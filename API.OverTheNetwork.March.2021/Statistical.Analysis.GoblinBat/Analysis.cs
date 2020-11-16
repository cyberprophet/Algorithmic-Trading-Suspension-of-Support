using System.Collections.Generic;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Statistical
{
    public abstract class Analysis
    {
        public abstract void AnalyzeTheConclusion(string[] param);
        public abstract void AnalyzeTheQuotes(string[] param);
        public abstract bool Collector
        {
            get; set;
        }
        public abstract string Code
        {
            get; set;
        }
        public abstract dynamic Current
        {
            get; set;
        }
        public abstract dynamic Offer
        {
            get; set;
        }
        public abstract dynamic Bid
        {
            get; set;
        }
        public abstract Queue<Collect> Collection
        {
            get; set;
        }
    }
}