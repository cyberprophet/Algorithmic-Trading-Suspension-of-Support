namespace ShareInvest.Communication
{
    public interface IStrategySetting
    {
        enum Hedge
        {
            UnUsed = 0,
            Hedge = 1,
            DoubleHedge = 2
        }
        long Capital
        {
            get; set;
        }
        int[] LongDay
        {
            get; set;
        }
        int[] LongTick
        {
            get; set;
        }
        int[] ShortDay
        {
            get; set;
        }
        int[] ShortTick
        {
            get; set;
        }
        int[] Reaction
        {
            get; set;
        }
        int EstimatedTime();
    }
}