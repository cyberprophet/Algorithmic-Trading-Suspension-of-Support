namespace ShareInvest.Catalog
{
    public struct TrendFollowingBasicFutures : IStrategics
    {
        public string Code
        {
            get; set;
        }
        public bool RollOver
        {
            get; set;
        }
        public int DayShort
        {
            get; set;
        }
        public int DayLong
        {
            get; set;
        }
        public int Minute
        {
            get; set;
        }
        public int MinuteShort
        {
            get; set;
        }
        public int MinuteLong
        {
            get; set;
        }
        public int ReactionShort
        {
            get; set;
        }
        public int ReactionLong
        {
            get; set;
        }
        public int QuantityShort
        {
            get; set;
        }
        public int QuantityLong
        {
            get; set;
        }
    }
}