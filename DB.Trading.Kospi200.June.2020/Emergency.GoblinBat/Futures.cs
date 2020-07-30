using ShareInvest.Catalog;

namespace ShareInvest.Emergency
{
    public struct Futures : ICharts
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