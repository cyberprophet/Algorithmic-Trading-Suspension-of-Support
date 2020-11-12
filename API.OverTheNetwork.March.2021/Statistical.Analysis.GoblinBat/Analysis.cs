using System.Collections.Generic;
using System.Threading.Tasks;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Statistical
{
    public abstract class Analysis
    {
        public abstract Task<object> AnalyzeTheDataAsync(Conclusion param);
        public abstract Task<object> AnalyzeTheDataAsync(Quotes param);
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