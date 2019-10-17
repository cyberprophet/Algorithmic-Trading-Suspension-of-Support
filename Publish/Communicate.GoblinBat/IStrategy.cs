namespace ShareInvest.Communicate
{
    public interface IStrategy
    {
        int Type
        {
            get;
        }
        int Reaction
        {
            get;
        }
        int ShortMinPeriod
        {
            get;
        }
        int LongMinPeriod
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
        int TransactionMultiplier
        {
            get;
        }
        long BasicAssets
        {
            get;
        }
        bool Division
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
    }
}