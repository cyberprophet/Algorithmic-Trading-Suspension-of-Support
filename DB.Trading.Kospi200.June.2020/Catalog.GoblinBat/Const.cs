namespace ShareInvest.Catalog
{
    public struct Const
    {
        public static int TransactionMultiplier
        {
            get
            {
                return transaction;
            }
        }
        public static double MarginRate
        {
            get
            {
                return margin;
            }
        }
        public static double MarginRate200402
        {
            get
            {
                return margin200402;
            }
        }
        public static double Commission
        {
            get
            {
                return commission;
            }
        }
        public static double ErrorRate
        {
            get
            {
                return rate;
            }
        }
        private const int transaction = 250000;
        private const double rate = 5e-2;
        private const double commission = 3e-5;
        private const double margin200402 = 16.2e-2;
        private const double margin = 7.65e-2;
    }
}