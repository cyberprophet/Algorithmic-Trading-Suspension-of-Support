namespace ShareInvest.Interface
{
    public interface IConfirm
    {
        string Confirm
        {
            get;
        }
        bool Identify(string id, string name);
    }
}