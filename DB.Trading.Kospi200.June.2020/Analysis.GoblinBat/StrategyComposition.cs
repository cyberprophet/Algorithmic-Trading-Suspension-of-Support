namespace ShareInvest.Analysis
{
    public struct StrategyComposition
    {
        public string Code
        {
            get; set;
        }
        public string Strategy
        {
            get; set;
        }
        public long Assets
        {
            get; set;
        }
        public int Time
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