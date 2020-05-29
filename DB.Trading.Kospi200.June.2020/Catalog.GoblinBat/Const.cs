namespace ShareInvest.Catalog
{
    public struct Const
    {
        public static int Tick => tick;
        public static int TransactionMultiplier => transaction;
        public static double MarginRate => margin;
        public static double MarginRate200402 => margin200402;
        public static double Commission => commission;
        public static double ErrorRate => rate;
        const int transaction = 250000;
        const int tick = 12500;
        const double rate = 5e-2;
        const double commission = 3e-5;
        const double margin200402 = 16.2e-2;
        const double margin = 7.65e-2;
    }
}