namespace ShareInvest.Catalog.OpenAPI
{
    public struct SendOrder
    {
        public string RQName
        {
            get; set;
        }
        public string ScreenNo
        {
            get; set;
        }
        public string AccNo
        {
            get; set;
        }
        public int OrderType
        {
            get; set;
        }
        public string Code
        {
            get; set;
        }
        public int Qty
        {
            get; set;
        }
        public int Price
        {
            get; set;
        }
        public string HogaGb
        {
            get; set;
        }
        public string OrgOrderNo
        {
            get; set;
        }
    }  
}