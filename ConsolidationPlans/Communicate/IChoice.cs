namespace ShareInvest.Communicate
{
    public interface IChoice
    {
        int Futures
        {
            get; set;
        }
        int Volume
        {
            get; set;
        }
        int Revenue
        {
            get; set;
        }
        int StopLoss
        {
            get; set;
        }
        long Assets
        {
            get; set;
        }
        double ShortPeriod
        {
            get; set;
        }
        double LongPeriod
        {
            get; set;
        }
    }
}