namespace ShareInvest.Communicate
{
    public interface IPeriod
    {
        enum Period
        {
            ShortTick = 0,
            LongTick = 1,
            ShortMinute = 2,
            LongMinute = 3,
            ShortDay = 4,
            LongDay = 5
        }
        int[] ShortTick
        {
            get;
        }
        int[] LongTick
        {
            get;
        }
        int[] ShortMinute
        {
            get;
        }
        int[] LongMinute
        {
            get;
        }
        int[] ShortDay
        {
            get;
        }
        int[] LongDay
        {
            get;
        }
    }
}