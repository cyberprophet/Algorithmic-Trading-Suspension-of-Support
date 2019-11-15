namespace ShareInvest.Communication
{
    public interface IStrategy
    {
        int Hedge
        {
            get;
        }
        int Reaction
        {
            get;
        }
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
        int TransactionMultiplier
        {
            get;
        }
        long BasicAssets
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
        string Strategy
        {
            get;
        }
        string PathLog
        {
            get;
        }
    }
}