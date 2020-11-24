namespace ShareInvest.Catalog.Models
{
    public struct StocksStrategics
    {
        public string Code
        {
            get; set;
        }
        public string Strategics
        {
            get; set;
        }
        public string Date
        {
            get; set;
        }
        public long MaximumInvestment
        {
            get; set;
        }
        public double CumulativeReturn
        {
            get; set;
        }
        public double WeightedAverageDailyReturn
        {
            get; set;
        }
        public double DiscrepancyRateFromExpectedStockPrice
        {
            get; set;
        }
    }
}