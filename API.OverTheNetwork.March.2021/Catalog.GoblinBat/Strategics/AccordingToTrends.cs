using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
	public struct AccordingToTrends : IStrategics
	{
		public string Code
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
	}
}