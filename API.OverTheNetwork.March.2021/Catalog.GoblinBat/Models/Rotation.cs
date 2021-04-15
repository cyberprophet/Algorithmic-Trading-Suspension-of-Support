namespace ShareInvest.Catalog.Models
{
	public struct Rotation
	{
		public string Date
		{
			get; set;
		}
		public string Code
		{
			get; set;
		}
		public int Purchase
		{
			get; set;
		}
		public int High
		{
			get; set;
		}
		public double MaxReturn
		{
			get; set;
		}
		public int Low
		{
			get; set;
		}
		public double MaxLoss
		{
			get; set;
		}
		public int Close
		{
			get; set;
		}
		public double Liquidation
		{
			get; set;
		}
	}
}