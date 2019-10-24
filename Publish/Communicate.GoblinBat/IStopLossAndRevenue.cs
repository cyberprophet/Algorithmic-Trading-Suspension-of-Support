namespace ShareInvest.Communicate
{
    public interface IStopLossAndRevenue
    {
        enum StopLossAndRevenue
        {
            UnUsed = 0,
            OnlyStopLoss = 1,
            OnlyRevenue = 2,
            UseAll = 3
        }
        bool Activate
        {
            get; set;
        }
        int StopLoss
        {
            get;
        }
        int Revenue
        {
            get;
        }
        int SetActivate(int quantity, double price, double purchase);
    }
}