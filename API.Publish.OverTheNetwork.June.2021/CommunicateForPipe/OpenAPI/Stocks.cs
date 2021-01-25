using System.Collections.Generic;

using ShareInvest.Interface;

namespace ShareInvest.SecondaryIndicators.OpenAPI
{
	public class Stocks : Analysis
	{
		public override string Code
		{
			get; set;
		}
		public override string Name
		{
			get; set;
		}
		public override string Memo
		{
			get; set;
		}
		public override int Quantity
		{
			get; set;
		}
		public override double Purchase
		{
			get; set;
		}
		public override dynamic Current
		{
			get; set;
		}
		public override dynamic Offer
		{
			get; set;
		}
		public override dynamic Bid
		{
			get; set;
		}
		public override double MarketMarginRate
		{
			get; set;
		}
		public override bool Wait
		{
			get; set;
		}
		public override IStrategics Strategics
		{
			get; set;
		}
		public override Dictionary<string, dynamic> OrderNumber
		{
			get; set;
		}
		public override Stack<double> Trend
		{
			get; set;
		}
		internal override dynamic SellPrice
		{
			get; set;
		}
		internal override dynamic BuyPrice
		{
			get; set;
		}
	}
}