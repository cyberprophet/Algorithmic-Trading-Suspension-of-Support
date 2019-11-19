namespace ShareInvest.Options
{
    public class Balance
    {
        public string Date
        {
            get; private set;
        }
        public string Time
        {
            get; private set;
        }
        public int Quantity
        {
            get; private set;
        }
        public double Price
        {
            get; private set;
        }
        public Balance(string date, string time, int quantity, double price)
        {
            Date = date;
            Time = time;
            Quantity = quantity;
            Price = price;
        }
    }
}