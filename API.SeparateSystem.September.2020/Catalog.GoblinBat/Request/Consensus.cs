namespace ShareInvest.Catalog.Request
{
    public struct Consensus
    {
        public string Code
        {
            get; set;
        }
        public string Strategics
        {
            get; set;
        }
        public string Date
        {
            get; set;
        }
        public double FirstQuarter
        {
            get; set;
        }
        public double SecondQuarter
        {
            get; set;
        }
        public double ThirdQuarter
        {
            get; set;
        }
        public double Quarter
        {
            get; set;
        }
        public double TheNextYear
        {
            get; set;
        }
        public double TheYearAfterNext
        {
            get; set;
        }
    }
}