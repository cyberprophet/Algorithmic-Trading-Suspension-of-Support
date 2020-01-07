using ShareInvest.Interface;

namespace ShareInvest.Const
{
    public class Specify : IStatistics
    {
        public int Base
        {
            get; set;
        }
        public int Sigma
        {
            get; set;
        }
        public int Percent
        {
            get; set;
        }
        public int Max
        {
            get; set;
        }
        public int Quantity
        {
            get; set;
        }
        public int Time
        {
            get; set;
        }
        public int ShortDayPeriod
        {
            get; set;
        }
        public int LongDayPeriod
        {
            get; set;
        }
        public int ShortTickPeriod
        {
            get; set;
        }
        public int LongTickPeriod
        {
            get; set;
        }
        public int Reaction
        {
            get; set;
        }
        public int HedgeType
        {
            get; set;
        }
        public int TransactionMultiplier
        {
            get;
        } = 250000;
        public double MarginRate
        {
            get;
        } = 7.65e-2;
        public double Commission
        {
            get;
        } = 3e-5;
        public double ErrorRate
        {
            get;
        } = 5e-2;
    }
}