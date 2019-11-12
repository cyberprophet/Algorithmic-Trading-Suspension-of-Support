namespace ShareInvest.Interface
{
    public interface IStrategy : IAccount, IStatistics
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
    }
}