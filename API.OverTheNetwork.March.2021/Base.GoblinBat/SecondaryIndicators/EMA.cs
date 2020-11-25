namespace ShareInvest.SecondaryIndicators
{
    public static class EMA
    {
        public static double Make(int period, int count, double price, double before)
        {
            var k = K(count, period);

            return price * k + before * (1 - k);
        }
        public static double Make(int period, int count, int price, double before)
        {
            var k = K(count, period);

            return price * k + before * (1 - k);
        }
        public static double Make(int count, double revenue, double before)
        {
            var k = 2 / (double)(1 + count);

            return revenue * k + before * (1 - k);
        }
        public static double Make(double price) => price;
        public static double Make(int price) => price;
        static double K(int count, int period) => count > period ? 2 / (double)(period + 1) : 2 / (double)(count + 1);
    }
}