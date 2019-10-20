using ShareInvest.Communicate;

namespace ShareInvest.Const
{
    public class MostFavorableOrder : IOrderMethod
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
        public MostFavorableOrder()
        {
            OrdTp = "9";
        }
    }
}