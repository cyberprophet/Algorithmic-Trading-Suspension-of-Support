using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
	public struct Rotation : IStrategics
	{
		public string Account
		{
			get; set;
		}
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
		public long Date
		{
			get; set;
		}
		public long Accumulate
		{
			get; set;
		}
		public string Liquidation
		{
			get; set;
		}
		public long PerDay
		{
			get; set;
		}
		public double AlphaRevenue
		{
			get; set;
		}
		public double BetaRevenue
		{
			get; set;
		}
		public double Revenue
		{
			get; set;
		}
		public double AlphaStopLoss
		{
			get; set;
		}
		public double BetaStopLoss
		{
			get; set;
		}
		public double StopLoss
		{
			get; set;
		}
	}
}