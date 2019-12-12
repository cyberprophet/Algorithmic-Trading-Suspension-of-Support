namespace ShareInvest.Communication
{
    public interface IStrategySetting
    {
        long Capital
        {
            get; set;
        }
        int[] Hedge
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