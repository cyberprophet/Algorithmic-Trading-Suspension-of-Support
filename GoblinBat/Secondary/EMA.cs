namespace ShareInvest.Secondary
{
    public class EMA : Indicator
    {
        public override double Make(int period, int count, double price, double before)
        {
            double k = K(count, period);

            return price * k + before * (1 - k);
        }
    }
}