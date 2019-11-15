namespace ShareInvest.Communication
{
    public class Specify : IStrategy
    {
        public int Hedge
        {
            get; set;
        }
        public int Reaction
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
        public int TransactionMultiplier
        {
            get;
        } = 250000;
        public long BasicAssets
        {
            get; set;
        }
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
        public string Strategy
        {
            get; set;
        }
        public string PathLog
        {
            get; set;
        }
    }
}