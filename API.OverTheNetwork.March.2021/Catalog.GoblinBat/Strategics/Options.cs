namespace ShareInvest.Catalog.Strategics
{
	public struct Options
	{
		public string Code
		{
			get; set;
		}
		public string StrikePrice
		{
			get; set;
		}
		public string Current
		{
			get; set;
		}
		public string ImpliedVolatility
		{
			get; set;
		}
		public string IntrinsicValue
		{
			get; set;
		}
		public string TimeValue
		{
			get; set;
		}
		public string Delta
		{
			get; set;
		}
		public string Gamma
		{
			get; set;
		}
		public string Theta
		{
			get; set;
		}
		public string Vega
		{
			get; set;
		}
		public string Rho
		{
			get; set;
		}
	}
}