using ShareInvest.Communicate;

namespace ShareInvest.Const
{
    public class MarketOrder : IOrderMethod
    {
        public string SlbyTP
        {
            get; set;
        }
        public string OrdTp
        {
            get; private set;
        }
        public string Price
        {
            get; set;
        }
        public MarketOrder()
        {
            OrdTp = "3";
            Price = "";
        }
    }
}