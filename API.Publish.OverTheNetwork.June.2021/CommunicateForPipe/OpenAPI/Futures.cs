using System;
using System.Collections.Generic;

using ShareInvest.EventHandler;

namespace ShareInvest.SecondaryIndicators.OpenAPI
{
	public class Futures : Analysis
	{
		public override event EventHandler<SendSecuritiesAPI> Send;
		public override event EventHandler<SendConsecutive> Consecutive;
		public override int OnReceiveConclusion(Dictionary<string, string> conclusion)
		{
			var cash = 0;

			return cash;
		}
		public override void OnReceiveEvent(string time, string price, string volume)
		{
			if (int.TryParse(volume, out int vol))
			{
				var consecutive = new SendConsecutive(vol, price, time);
				Consecutive?.Invoke(this, consecutive);

				if (Current != consecutive.Price)
				{
					Revenue = (long)((consecutive.Price - Purchase) * Quantity);
					Rate = consecutive.Price / (double)Purchase - 1;
					Current = consecutive.Price;
				}
			}
		}
		public override void OnReceiveDrawChart(object sender, SendConsecutive e)
		{
			Send?.Invoke(this, new SendSecuritiesAPI(e.Date));
		}
		public override bool GetCheckOnDate(string date)
		{
			var line = date.Equals(DateLine);
			DateLine = date;

			return line;
		}
		public override string Code
		{
			get; set;
		}
		public override string Name
		{
			get; set;
		}
		public override string Account
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
		public override double Rate
		{
			get; set;
		}
		public override long Revenue
		{
			get; set;
		}
		public override bool Wait
		{
			get; set;
		}
		public override Interface.Strategics Strategics
		{
			get; set;
		}
		public override Interface.IStrategics Classification
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
		public override Stack<double> Long
		{
			get; set;
		}
		public override Stack<double> Short
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
		protected internal override string DateLine
		{
			get; set;
		}
		protected internal override double Gap
		{
			get; set;
		}
		protected internal override double Peek
		{
			get; set;
		}
		protected internal override DateTime NextOrderTime
		{
			get; set;
		}
	}
}