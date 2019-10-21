using ShareInvest.Communicate;

namespace ShareInvest.Const
{
    public class SpecifyKosdaq150 : IStrategy
    {
        public SpecifyKosdaq150()
        {
            Type = 24;
            TransactionMultiplier = 10000;
            BasicAssets = 5000000;
            MarginRate = 1.665e-1;
            Commission = 3e-5;
            ErrorRate = 1e-1;
        }
        public int Type
        {
            get; private set;
        }
        public int Reaction
        {
            get; set;
        }
        public int ShortMinPeriod
        {
            get; set;
        }
        public int LongMinPeriod
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
        public int TransactionMultiplier
        {
            get; private set;
        }
        public long BasicAssets
        {
            get; private set;
        }
        public bool Division
        {
            get; set;
        }
        public double MarginRate
        {
            get; private set;
        }
        public double Commission
        {
            get; private set;
        }
        public double ErrorRate
        {
            get; private set;
        }
        public string Strategy
        {
            get; set;
        }
    }
}