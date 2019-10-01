using System;

namespace ShareInvest.EventHandler
{
    public class Conclusion : EventArgs
    {
        public int Total
        {
            get
            {
                return total;
            }
            private set
            {
                total += value;
            }
        }
        public double Commission
        {
            get; private set;
        }
        public DateTime Time
        {
            get; private set;
        }
        public Conclusion(string time, int commission, double price)
        {
            Time = DateTime.ParseExact(time, "HHmmss", null);
            Commission = commission;
            purchase = price;
            Total = -commission;
        }
        public Conclusion(int quantity, double price, string classification)
        {
            if (quantity - amount < 0)
                Total = (int)((classification.Equals("1") ? price - purchase : purchase - price) * 250000 * (amount - quantity));

            Time = DateTime.Now;
            amount = quantity;
        }
        private static int amount;
        private static int total;
        private static double purchase;
    }
}