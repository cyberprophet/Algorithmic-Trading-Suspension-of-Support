namespace ShareInvest.Interface
{
    public interface IStrategy : IAccount
    {
        string OrdTp
        {
            get;
        }
        string Price
        {
            get; set;
        }
        string Code
        {
            get;
        }
        int Qty
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
    }
}