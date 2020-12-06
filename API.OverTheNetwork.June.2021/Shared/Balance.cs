namespace ShareInvest
{
	public class Balance
	{
		public Balance(string[] param)
		{
			if (int.TryParse(param[2], out int quantity) && long.TryParse(param[5], out long revenue)
				&& double.TryParse(param[3], out double purchase) && double.TryParse(param[6], out double rate))
			{
				Market = param[0].Length == 8 && param[0][1] == '0';
				Name = param[1];
				Quantity = quantity;
				Purchase = Market ? purchase : (int)purchase;
				Revenue = revenue;
				Rate = rate;
			}
		}
		public Balance(string name) => Name = name;
		public string Name
		{
			get; set;
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
		public bool Market
		{
			get;
		}
	}
}