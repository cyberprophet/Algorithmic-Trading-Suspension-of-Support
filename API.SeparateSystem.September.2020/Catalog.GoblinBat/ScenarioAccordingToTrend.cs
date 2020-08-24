using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
    public struct ScenarioAccordingToTrend : IStrategics
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
        public int Quantity
        {
            get; set;
        }
        public int IntervalInSeconds
        {
            get; set;
        }
        public double ErrorRange
        {
            get; set;
        }
        public double Sales
        {
            get; set;
        }
        public double OperatingProfit
        {
            get; set;
        }
        public double NetIncome
        {
            get; set;
        }
        public bool CheckSales
        {
            get; set;
        }
        public bool CheckOperatingProfit
        {
            get; set;
        }
        public bool CheckNetIncome
        {
            get; set;
        }
        public string Calendar
        {
            get; set;
        }
    }
}