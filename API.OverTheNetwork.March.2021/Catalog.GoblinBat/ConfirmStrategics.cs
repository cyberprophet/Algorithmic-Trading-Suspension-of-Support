namespace ShareInvest.Catalog
{
    public struct ConfirmStrategics
    {
        public string Strategics
        {
            get; set;
        }
        public string Code
        {
            get; set;
        }
        public string Date
        {
            get; set;
        }
    }
    public enum RevisedStockPrice
    {
        유상증자 = 1,
        무상증자 = 2,
        배당락 = 4,
        액면분할 = 8,
        액면병합 = 0x10,
        기업합병 = 0x20,
        감자 = 0x40,
        권리락 = 0x100
    }
}