namespace ShareInvest.Interface
{
    public interface IStrategy
    {
        string OrdTp
        {
            get;
        }
        string Price
        {
            get;
        }
        string SlbyTP
        {
            get;
        }
        int Qty
        {
            get;
        }
        enum OrderType
        {
            지정가 = 1,
            조건부지정가 = 2,
            시장가 = 3,
            최유리지정가 = 4,
            지정가IOC = 5,
            지정가FOK = 6,
            시장가IOC = 7,
            시장가FOK = 8,
            최유리지정가IOC = 9,
            최유리지정가FOK = 'A'
        }
    }
}