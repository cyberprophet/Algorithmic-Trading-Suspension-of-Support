namespace ShareInvest.Communicate
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