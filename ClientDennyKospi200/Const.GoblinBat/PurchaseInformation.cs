using ShareInvest.Interface;

namespace ShareInvest.Const
{
    public class PurchaseInformation : IStrategy
    {
        public string OrdTp
        {
            get; set;
        }
        public string Price
        {
            get; set;
        }
        public string SlbyTP
        {
            get; set;
        }
        public int Qty
        {
            get; set;
        }
    }
}