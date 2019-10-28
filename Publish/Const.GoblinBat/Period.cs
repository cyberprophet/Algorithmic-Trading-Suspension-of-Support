using ShareInvest.Communicate;

namespace ShareInvest.Const
{
    public class Period : IPeriod
    {
        public int[] ShortTick
        {
            get; private set;
        } = { 15 };
        public int[] LongTick
        {
            get; private set;
        } = { 120 };
        public int[] ShortMinute
        {
            get; private set;
        } = { 2, 3, 5 };
        public int[] LongMinute
        {
            get; private set;
        } = { 10, 15, 20 };
        public int[] ShortDay
        {
            get; private set;
        } = { 2, 3 };
        public int[] LongDay
        {
            get; private set;
        } = { 15, 20 };
    }
}