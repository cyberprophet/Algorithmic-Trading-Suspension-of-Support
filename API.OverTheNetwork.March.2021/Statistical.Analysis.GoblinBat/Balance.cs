namespace ShareInvest.Statistical
{
    public class Balance
    {
        public Balance(string[] param)
        {
            if (int.TryParse(param[2], out int quantity) && long.TryParse(param[5], out long revenue) && double.TryParse(param[6], out double rate))
            {
                Name = param[1];
                Quantity = quantity;
                Purchase = param[0].Length == 6 ? (int.TryParse(param[3], out int stocks) ? stocks : 0) : (double.TryParse(param[3], out double futures) ? futures : 0D);
                Revenue = revenue;
                Rate = rate;
            }
        }
        public string Name
        {
            get; private set;
        }
        public int Quantity
        {
            get; set;
        }
        public dynamic Purchase
        {
            get; set;
        }
        public long Revenue
        {
            get; set;
        }
        public double Rate
        {
            get; set;
        }
    }
}