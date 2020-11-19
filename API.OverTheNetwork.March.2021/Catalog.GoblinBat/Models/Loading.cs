namespace ShareInvest.Catalog.Models
{
    public struct Loading
    {
        public string Code
        {
            get; set;
        }
        public int Year
        {
            get; set;
        }
        public int Month
        {
            get; set;
        }
        public int Day
        {
            get; set;
        }
        public uint Start
        {
            get; set;
        }
        public uint End
        {
            get; set;
        }
        public string Price
        {
            get; set;
        }
        public long Length
        {
            get; set;
        }
    }
}