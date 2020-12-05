using Newtonsoft.Json;

namespace ShareInvest
{
	public class Balance
	{
		public Balance(string[] param)
		{
			if (int.TryParse(param[2], out int quantity) && long.TryParse(param[5], out long revenue)
				&& double.TryParse(param[6], out double rate) && double.TryParse(param[4], out double current))
			{
				Name = param[1];
				Quantity = quantity;
				Purchase = param[0].Length == 6 ? (int.TryParse(param[3], out int stocks) ? stocks : 0) : (double.TryParse(param[3], out double futures) ? futures : 0D);
				Revenue = revenue;
				Rate = rate;
				Price = param[0].Length == 8 && param[0][1] == '0' ? current : (int)current;
			}
		}
		[JsonConstructor]
		public Balance()
		{

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
		public dynamic Price
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