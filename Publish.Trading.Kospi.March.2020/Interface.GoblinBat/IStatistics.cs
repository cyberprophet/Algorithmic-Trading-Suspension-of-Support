namespace ShareInvest.Interface
{
    public interface IStatistics
    {
        int Base
        {
            get;
        }
        int Sigma
        {
            get;
        }
        int Percent
        {
            get;
        }
        int Max
        {
            get;
        }
        int Quantity
        {
            get;
        }
        int Time
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
        enum Numeric
        {
            ShortDay = 0,
            ShortTick = 1,
            LongDay = 2,
            LongTick = 3,
            Reaction = 4,
            Hedge = 5
        }
    }
}