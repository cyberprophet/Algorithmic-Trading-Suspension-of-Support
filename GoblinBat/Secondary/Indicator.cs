namespace ShareInvest.Secondary
{
    public class Indicator
    {
        public virtual double Make(int period, int count, double price, double before)
        {
            return price;
        }
        protected double K(int count, int period)
        {
            return count > period ? 2 / (double)(period + 1) : 2 / (double)(count + 1);
        }
        public int ShortPeriod
        {
            get; set;
        }
        public int LongPeriod
        {
            get; set;
        }
        public Indicator()
        {
            ShortPeriod = 5;
            LongPeriod = 60;
        }
    }
}