using ShareInvest.RetrieveOptions;

namespace ShareInvest.Communication
{
    public interface IStrategy : IOptions
    {
        int Percent
        {
            get;
        }
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
        int Base
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
        int Max
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
        double Sigma
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