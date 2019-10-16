namespace ShareInvest.Communicate
{
    public interface IChoice
    {
        int Type
        {
            get;
        }
        int Index
        {
            get;
        }
        int Length
        {
            get;
        }
        string Path
        {
            get;
        }
    }
}