namespace ShareInvest.Interface
{
    public interface IAccount
    {
        string AccNo
        {
            get;
        }
        long BasicAssets
        {
            get;
        }
        string Code
        {
            get;
        }
    }
}