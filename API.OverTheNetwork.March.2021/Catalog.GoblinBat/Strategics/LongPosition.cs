using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
	public struct LongPosition : IStrategics
	{
		public string Code
		{
			get; set;
		}
		public string Account
		{
			get; set;
		}
		public ulong Overweight
		{
			get; set;
		}
		public double Underweight
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
	}
}