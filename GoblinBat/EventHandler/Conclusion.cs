using System;

namespace ShareInvest.EventHandler
{
    public class Conclusion : EventArgs
    {
        public double Total
        {
            get
            {
                return total;
            }
            set
            {
                total += value;
            }
        }
        public Conclusion(string time, double commission, double price)
        {
            this.time = DateTime.ParseExact(time, "HHmmss", null);
            this.commission = commission;
            purchase = price;
            Total = commission;
        }
        public Conclusion(int quantity, double price, string classification)
        {
            if (quantity - amount < 0)
                Total = (classification.Equals("1") ? price - purchase : purchase - price) * 250000 * amount - quantity;

            amount = quantity;
        }
        public DateTime time;
        public double commission;

        private static int amount;
        private static double total;
        private static double purchase;
    }
}