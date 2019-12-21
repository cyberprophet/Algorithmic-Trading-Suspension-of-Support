namespace ShareInvest.Communication
{
    public interface IAsset
    {
        enum Variable
        {
            ShortTick = 0,
            ShortDay = 1,
            LongTick = 2,
            LongDay = 3,
            Reaction = 4,
            Hedge = 5,
            Base = 6,
            Sigma = 7,
            Percent = 8,
            Max = 9,
            Quantity = 10,
            Time = 11
        }
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