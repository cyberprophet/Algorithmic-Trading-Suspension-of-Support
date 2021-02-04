using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
	public struct SatisfyConditionsAccordingToTrends : IStrategics
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
		public int ReservationSellUnit
		{
			get; set;
		}
		public int ReservationSellQuantity
		{
			get; set;
		}
		public double ReservationSellRate
		{
			get; set;
		}
		public int ReservationBuyUnit
		{
			get; set;
		}
		public int ReservationBuyQuantity
		{
			get; set;
		}
		public double ReservationBuyRate
		{
			get; set;
		}
		public double TradingSellInterval
		{
			get; set;
		}
		public int TradingSellQuantity
		{
			get; set;
		}
		public double TradingSellRate
		{
			get; set;
		}
		public double TradingBuyInterval
		{
			get; set;
		}
		public int TradingBuyQuantity
		{
			get; set;
		}
		public double TradingBuyRate
		{
			get; set;
		}
		public long Date
		{
			get; set;
		}
	}
}