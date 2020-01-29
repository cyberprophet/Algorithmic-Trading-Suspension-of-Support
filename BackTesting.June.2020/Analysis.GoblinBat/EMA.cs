namespace ShareInvest.Analysis
{
    internal partial struct EMA
    {
        private double K(int count, int period)
        {
            return count > period ? 2 / (double)(period + 1) : 2 / (double)(count + 1);
        }
        internal double Make(int period, int count, double price, double before)
        {
            double k = K(count, period);

            return price * k + before * (1 - k);
        }
        internal double Make(double price)
        {
            return price;
        }
    }
}