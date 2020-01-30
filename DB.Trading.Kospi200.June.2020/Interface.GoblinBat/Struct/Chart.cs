namespace ShareInvest.Interface.Struct
{
    public struct Chart
    {
        public long Date
        {
            get; set;
        }
        public dynamic Price
        {
            get; set;
        }
        public int Volume
        {
            get; set;
        }
    }
}