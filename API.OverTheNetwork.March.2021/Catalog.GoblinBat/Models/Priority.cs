namespace ShareInvest.Catalog.Models
{
    public struct Priority
    {
        public string Code
        {
            get; set;
        }
        public dynamic Current
        {
            get; set;
        }
        public dynamic Offer
        {
            get; set;
        }
        public dynamic Bid
        {
            get; set;
        }
    }
}