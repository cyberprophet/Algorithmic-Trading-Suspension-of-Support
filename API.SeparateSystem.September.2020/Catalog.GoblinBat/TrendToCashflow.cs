using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
    public struct TrendToCashflow : IStrategics
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
        public int Unit
        {
            get; set;
        }
        public int ReservationQuantity
        {
            get; set;
        }
        public double ReservationRevenue
        {
            get; set;
        }
        public double Addition
        {
            get; set;
        }
        public int Interval
        {
            get; set;
        }
        public int TradingQuantity
        {
            get; set;
        }
        public double PositionRevenue
        {
            get; set;
        }
        public double PositionAddition
        {
            get; set;
        }
    }
}