namespace ShareInvest.Statistical
{
    public class Balance
    {
        public Balance(string[] param)
        {
            if (int.TryParse(param[2], out int quantity) && long.TryParse(param[5], out long revenue) && double.TryParse(param[6], out double rate))
            {
                Code = param[0];
                Name = param[1];
                Quantity = quantity;
                Purchase = param[0].Length == 6 ? (int.TryParse(param[3], out int stocks) ? stocks : 0) : (double.TryParse(param[3], out double futures) ? futures : 0D);
                Current = param[0].Length == 6 ? (int.TryParse(param[3], out int price) ? price : 0) : (double.TryParse(param[3], out double current) ? current : 0D);
                Revenue = revenue;
                Rate = rate;
            }
        }
        public string Code
        {
            get; private set;
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
        public dynamic Current
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