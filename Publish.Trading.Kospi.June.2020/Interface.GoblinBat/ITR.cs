namespace ShareInvest.Interface
{
    public interface ITR
    {
        string ID
        {
            get;
        }
        string Value
        {
            get; set;
        }
        string RQName
        {
            get; set;
        }
        string TrCode
        {
            get;
        }
        int PrevNext
        {
            get; set;
        }
        string ScreenNo
        {
            get;
        }
    }
}