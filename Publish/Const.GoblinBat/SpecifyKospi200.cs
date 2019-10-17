using ShareInvest.Communicate;

namespace ShareInvest.Const
{
    public class SpecifyKospi200 : IStrategy
    {
        public SpecifyKospi200()
        {
            Type = 0;
            TransactionMultiplier = 250000;
            BasicAssets = 35000000;
            MarginRate = 7.65e-2;
            Commission = 3e-5;
            ErrorRate = 5e-2;
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