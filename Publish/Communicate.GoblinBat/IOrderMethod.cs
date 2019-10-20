namespace ShareInvest.Communicate
{
    public interface IOrderMethod
    {
        string SlbyTP
        {
            get; set;
        }
        string OrdTp
        {
            get;
        }
        string Price
        {
            get; set;
        }
    }
}