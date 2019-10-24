namespace ShareInvest.Communicate
{
    public interface IStrategy : IStopLossAndRevenue, IPath
    {
        enum Futures
        {
            Kospi200 = 0,
            Kosdaq150 = 24
        }
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
        IStopLossAndRevenue.StopLossAndRevenue Stop
        {
            get;
        }
    }
}