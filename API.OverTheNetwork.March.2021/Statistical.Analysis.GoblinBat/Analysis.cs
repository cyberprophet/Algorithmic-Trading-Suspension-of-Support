using System.Collections.Generic;
using System.Linq;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Statistical
{
    public abstract class Analysis
    {
        public virtual bool Market
        {
            get; set;
        }
        public virtual dynamic SellPrice
        {
            protected internal get; set;
        }
        public virtual dynamic BuyPrice
        {
            protected internal get; set;
        }
        public virtual double MarginRate
        {
            get; set;
        }
        public virtual (IEnumerable<Collect>, uint, uint, string) SortTheRecordedInformation
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
                    string index = 0.ToString("D3"), peek = Collection.Count > 0 ? Collection.Peek().Time : c.Time;

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
                var max = storage.Max(o => o.Key);

                return (queue, storage.Min(o => o.Key), max, max > 152959 && Code.Length == 6 || max > 154459 && Code.Length == 8 ?
                    storage[max].Split(';')[0][1..] : string.Empty);
            }
        }
        public virtual int GetQuoteUnit(int price, bool info) => price switch
        {
            int n when n >= 0 && n < 0x3E8 => 1,
            int n when n >= 0x3E8 && n < 0x1388 => 5,
            int n when n >= 0x1388 && n < 0x2710 => 0xA,
            int n when n >= 0x2710 && n < 0xC350 => 0x32,
            int n when n >= 0x186A0 && n < 0x7A120 && info => 0x1F4,
            int n when n >= 0x7A120 && info => 0x3E8,
            _ => 0x64,
        };
        public virtual int GetStartingPrice(int price, bool info) => price switch
        {
            int n when n >= 0 && n < 0x3E8 => price,
            int n when n >= 0x3E8 && n < 0x1388 => (price / 5 + 1) * 5,
            int n when n >= 0x1388 && n < 0x2710 => (price / 0xA + 1) * 0xA,
            int n when n >= 0x2710 && n < 0xC350 => (price / 0x32 + 1) * 0x32,
            int n when n >= 0x186A0 && n < 0x7A120 && info => (price / 0x1F4 + 1) * 0x1F4,
            int n when n >= 0x7A120 && info => (price / 0x3E8 + 1) * 0x3E8,
            _ => (price / 0x64 + 1) * 0x64,
        };
        public abstract void AnalyzeTheConclusion(string[] param);
        public abstract void AnalyzeTheQuotes(string[] param);
        public abstract bool Collector
        {
            get; set;
        }
        public abstract bool Wait
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
        public abstract Balance Balance
        {
            get; set;
        }
        public abstract Interface.IStrategics Strategics
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