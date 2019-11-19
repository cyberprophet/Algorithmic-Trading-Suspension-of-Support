namespace ShareInvest.Options
{
    public class Revenue
    {
        public string Code
        {
            get; set;
        }
        public double Price
        {
            get; set;
        }
        public int Quantity
        {
            get; set;
        }
        public Revenue(string code, double price, int quantity)
        {
            Code = code;
            Price = price;
            Quantity = quantity;
        }
    }
}