namespace ShareInvest.Catalog.Models
{
	public struct GroupDetail
	{
		public string Code
		{
			get; set;
		}
		public string Date
		{
			get; set;
		}
		public string Title
		{
			get; set;
		}
		public int Current
		{
			get; set;
		}
		public int Page
		{
			get; set;
		}
		public double Rate
		{
			get; set;
		}
		public double Compare
		{
			get; set;
		}
		public int[] Tick
		{
			get; set;
		}
		public double[] Inclination
		{
			get; set;
		}
		public double Percent
		{
			get; set;
		}
		public string Index
		{
			get; set;
		}
	}
}