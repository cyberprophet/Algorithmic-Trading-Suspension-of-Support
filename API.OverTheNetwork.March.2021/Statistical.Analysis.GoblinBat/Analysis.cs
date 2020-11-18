using System.Collections.Generic;
using System.Linq;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Statistical
{
    public abstract class Analysis
    {
        public virtual (IEnumerable<Collect>, uint, uint) SortTheRecordedInformation
        {
            get
            {
                uint count = 0;
                var queue = new Queue<Collect>(Collection.Count);
                var temp = new Dictionary<string, uint>(0x400);
                var storage = new Dictionary<uint, string>(0x800);

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
                    if (uint.TryParse(string.Concat(c.Time, index), out uint time))
                        storage[time] = c.Datum;
                }
                foreach (var collect in storage.OrderBy(o => o.Key))
                    queue.Enqueue(new Collect
                    {
                        Time = collect.Key.ToString("D9"),
                        Datum = collect.Value
                    });
                return (queue, storage.Min(o => o.Key), storage.Max(o => o.Key));
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