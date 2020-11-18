using System.Collections.Generic;
using System.Linq;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Statistical
{
    public abstract class Analysis
    {
        public virtual void SortTheRecordedInformation()
        {
            uint count = 0;
            var temp = new Dictionary<string, uint>();

            while (Collection.Count > 0)
            {
                var c = Collection.Dequeue();
                string index = 0.ToString("D3"), peek = Collection.Peek().Time;

                if (c.Time.CompareTo(peek) == 0)
                {
                    temp[c.Time] = count;
                    index = count++.ToString("D3");
                }
                else
                {
                    temp[c.Time] = count;
                    index = count.ToString("D3");
                    count = temp.Any(o => o.Key.Equals(peek)) ? temp.First(o => o.Key.Equals(peek)).Value + 1 : 0;
                }
                var time = string.Concat(c.Time, index);
            }
        }
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
        protected internal Dictionary<string, string> Record
        {
            get; set;
        }
    }
}