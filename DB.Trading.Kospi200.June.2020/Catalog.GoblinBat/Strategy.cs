using System.Collections.Generic;

namespace ShareInvest.Catalog
{
    public struct Strategy
    {
        public long Assets
        {
            get; set;
        }
        public string Code
        {
            get; set;
        }
        public Dictionary<string, string> Contents
        {
            get; set;
        }
    }
}