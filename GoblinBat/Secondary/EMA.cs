namespace ShareInvest.Secondary
{
    public class EMA
    {
        private double K(int count, int period)
        {
            return count > period ? 2 / (double)(period + 1) : 2 / (double)(count + 1);
        }
        public EMA(int sp, int lp)
        {
            ShortPeriod = sp;
            LongPeriod = lp;
        }
        public double Make(int period, int count, double price, double before)
        {
            double k = K(count, period);

            return price * k + before * (1 - k);
        }
        public double Make(double price)
        {
            return price;
        }
        public int ShortPeriod
        {
            get; protected set;
        }
        public int LongPeriod
        {
            get; protected set;
        }
    }
}