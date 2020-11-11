using System.Collections.Generic;

namespace ShareInvest.Statistical
{
    public abstract class Analysis
    {
        public abstract string Code
        {
            get; set;
        }
        public abstract Queue<Catalog.Models.Collect> Collection
        {
            get; set;
        }
    }
}