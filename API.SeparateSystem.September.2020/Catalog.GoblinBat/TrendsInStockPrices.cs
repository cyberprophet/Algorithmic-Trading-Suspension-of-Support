namespace ShareInvest.Catalog
{
    public struct TrendsInStockPrices : IStrategics
    {
        public string Code
        {
            get; set;
        }
        public int Short
        {
            get; set;
        }
        public int Long
        {
            get; set;
        }
        public int Trend
        {
            get; set;
        }
        public double RealizeProfit
        {
            get; set;
        }
        public double AdditionalPurchase
        {
            get; set;
        }
        public int Quantity
        {
            get; set;
        }
        public int QuoteUnit
        {
            get; set;
        }
        public LongShort LongShort
        {
            get; set;
        }
        public Trend TrendType
        {
            get; set;
        }
        public Setting Setting
        {
            get; set;
        }
    }
}