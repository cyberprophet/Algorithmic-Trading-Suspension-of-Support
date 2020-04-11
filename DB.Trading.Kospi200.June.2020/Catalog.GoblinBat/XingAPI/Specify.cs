namespace ShareInvest.Catalog.XingAPI
{
    public struct Specify
    {
        public ulong Assets
        {
            get; set;
        }
        public string Code
        {
            get; set;
        }
        public double Commission
        {
            get; set;
        }
        public double MarginRate
        {
            get; set;
        }
        public bool RollOver
        {
            get; set;
        }
        public string Strategy
        {
            get; set;
        }
        public uint Time
        {
            get; set;
        }
        public int Short
        {
            get; set;
        }
        public int Long
        {
            get; set;
        }
    }
}