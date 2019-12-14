namespace ShareInvest.Communication
{
    public interface IAsset
    {
        string Account
        {
            get;
        }
        long Assets
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
        int Base
        {
            get;
        }
        int Sigma
        {
            get;
        }
        int Percent
        {
            get;
        }
        int Max
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
        string[] Temp
        {
            get;
        }
    }
}