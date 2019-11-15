namespace ShareInvest.Interface
{
    public interface IStatistics
    {
        int ShortDayPeriod
        {
            get;
        }
        int LongDayPeriod
        {
            get;
        }
        int ShortTickPeriod
        {
            get;
        }
        int LongTickPeriod
        {
            get;
        }
        int Reaction
        {
            get;
        }
        int HedgeType
        {
            get;
        }
        int TransactionMultiplier
        {
            get;
        }
        double MarginRate
        {
            get;
        }
        double Commission
        {
            get;
        }
        double ErrorRate
        {
            get;
        }
        enum Hedge
        {
            UnUsed = 0,
            Hedge = 1,
            DoubleHedge = 2
        }
    }
}