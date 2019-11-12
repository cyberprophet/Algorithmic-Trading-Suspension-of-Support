namespace ShareInvest.Interface
{
    public interface IStrategy
    {
        string OrdTp
        {
            get;
        }
        string Price
        {
            get;
        }
        string SlbyTP
        {
            get;
        }
        int Qty
        {
            get;
        }
    }
}