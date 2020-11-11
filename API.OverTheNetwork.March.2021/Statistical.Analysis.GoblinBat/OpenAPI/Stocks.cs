using System.Collections.Generic;

using ShareInvest.Catalog.Models;

namespace ShareInvest.Statistical.OpenAPI
{
    public class Stocks : Analysis
    {
        public override string Code
        {
            get; set;
        }
        public override Queue<Collect> Collection
        {
            get; set;
        }
    }
}