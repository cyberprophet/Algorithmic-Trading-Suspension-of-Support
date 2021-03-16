using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
	public struct Scenario : IStrategics
	{
		public string Code
		{
			get; set;
		}
		public string Account
		{
			get; set;
		}
		public int Short
		{
			get; set;
		}
		public int Long
		{
			get; set;
		}
		public int Trend
		{
			get; set;
		}
		public long Date
		{
			get; set;
		}
		public long Maximum
		{
			get; set;
		}
		public double Target
		{
			get; set;
		}
		public int Hope
		{
			get; set;
		}
	}
}