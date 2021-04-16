namespace ShareInvest.Catalog.Strategics
{
	public struct Condition
	{
		public string Code
		{
			get; set;
		}
		public string Name
		{
			get; set;
		}
		public string Theme
		{
			get; set;
		}
		public string Title
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
		public double HighRate
		{
			get; set;
		}
		public int Low
		{
			get; set;
		}
		public double LowRate
		{
			get; set;
		}
	}
}