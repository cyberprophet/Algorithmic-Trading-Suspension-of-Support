namespace ShareInvest.Catalog
{
    public struct Const
    {
        public static int TransactionMultiplier
        {
            get
            {
                return 250000;
            }
        }
        public static double MarginRate
        {
            get
            {
                return 7.65e-2;
            }
        }
        public static double Commission
        {
            get
            {
                return 3e-5;
            }
        }
        public static double ErrorRate
        {
            get
            {
                return 5e-2;
            }
        }
    }
}