using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
    public struct TrendsInValuation : IStrategics
    {
        public string Code
        {
            get; set;
        }
        public string AnalysisType
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
        public int SubtractionalUnit
        {
            get; set;
        }
        public int ReservationSubtractionalQuantity
        {
            get; set;
        }
        public double Subtraction
        {
            get; set;
        }
        public int AdditionalUnit
        {
            get; set;
        }
        public int ReservationAddtionalQuantity
        {
            get; set;
        }
        public double Addition
        {
            get; set;
        }
        public int SubtractionalInterval
        {
            get; set;
        }
        public int TradingSubtractionalQuantity
        {
            get; set;
        }
        public double SubtractionalPosition
        {
            get; set;
        }
        public int AddtionalInterval
        {
            get; set;
        }
        public int TradingAddtionalQuantity
        {
            get; set;
        }
        public double AdditionalPosition
        {
            get; set;
        }
    }
}