using System.Collections.Generic;

using ShareInvest.Interface;

namespace ShareInvest
{
	public abstract class Analysis
	{
		public abstract string Code
		{
			get; set;
		}
		public abstract string Name
		{
			get; set;
		}
		public abstract string Memo
		{
			get; set;
		}
		public abstract int Quantity
		{
			get; set;
		}
		public abstract double Purchase
		{
			get; set;
		}
		public abstract dynamic Current
		{
			get; set;
		}
		public abstract dynamic Offer
		{
			get; set;
		}
		public abstract dynamic Bid
		{
			get; set;
		}
		public abstract double MarketMarginRate
		{
			get; set;
		}
		public abstract bool Wait
		{
			get; set;
		}
		public abstract IStrategics Strategics
		{
			get; set;
		}
		public abstract Dictionary<string, dynamic> OrderNumber
		{
			get; set;
		}
		public abstract Stack<double> Trend
		{
			get; set;
		}
		internal abstract dynamic SellPrice
		{
			get; set;
		}
		internal abstract dynamic BuyPrice
		{
			get; set;
		}
	}
}