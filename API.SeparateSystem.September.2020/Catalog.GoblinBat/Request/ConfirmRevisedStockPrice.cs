namespace ShareInvest.Catalog.Request
{
    public struct ConfirmRevisedStockPrice
    {
        public string Date
        {
            get; set;
        }
        public double Rate
        {
            get; set;
        }
        public string Price
        {
            get; set;
        }
        public string Revise
        {
            get; set;
        }
    }
}