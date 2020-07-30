using ShareInvest.Catalog;

namespace ShareInvest.Emergency
{
    public struct Stocks : ICharts
    {
        public string Code
        {
            get; set;
        }
        public string Retention
        {
            get; set;
        }
        public string Date
        {
            get; set;
        }
        public string Price
        {
            get; set;
        }
        public int Volume
        {
            get; set;
        }
    }
}