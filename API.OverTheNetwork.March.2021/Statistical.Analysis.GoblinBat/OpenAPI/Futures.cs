using System.Collections.Generic;
using System.Threading.Tasks;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Statistical.OpenAPI
{
    public class Futures : Analysis
    {
        public override Task<object> AnalyzeTheDataAsync(Conclusion param)
        {
            return null;
        }
        public override Task<object> AnalyzeTheDataAsync(Quotes param)
        {
            return null;
        }
        public override string Code
        {
            get; set;
        }
        public override dynamic Current
        {
            get; set;
        }
        public override dynamic Offer
        {
            get; set;
        }
        public override dynamic Bid
        {
            get; set;
        }
        public override Queue<Collect> Collection
        {
            get; set;
        }
    }
}