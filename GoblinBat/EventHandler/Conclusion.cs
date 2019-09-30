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
        public Conclusion(string time, int commission, double price)
        {
            this.time = DateTime.ParseExact(time, "HHmmss", null);
            this.commission = commission;

            purchase = price;
            Total = -commission;
        }
        public Conclusion(int quantity, double price, string classification)
        {
            if (quantity - amount < 0)
                Total = (int)((classification.Equals("1") ? price - purchase : purchase - price) * 250000 * (amount - quantity));

            amount = quantity;
        }
        public DateTime time = DateTime.Now;
        public double commission;

        private static int amount;
        private static int total;
        private static double purchase;
    }
}