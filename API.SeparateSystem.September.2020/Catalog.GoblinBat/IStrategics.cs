namespace ShareInvest.Catalog
{
    public interface IStrategics
    {
        string Code
        {
            get; set;
        }
    }
    public enum LongShort
    {
        Day = 'D',
        Minute = 'M'
    }
    public enum Trend
    {
        Day = 'd',
        Minute = 'm'
    }
    public enum Setting
    {
        Short = 'S',
        Long = 'L',
        Both = 'B',
        Reservation = 'R'
    }
}