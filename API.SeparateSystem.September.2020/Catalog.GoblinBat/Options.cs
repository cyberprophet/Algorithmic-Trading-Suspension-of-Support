using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
    public struct Options : ICharts
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