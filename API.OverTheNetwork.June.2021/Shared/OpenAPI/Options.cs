using System;
using System.Collections.Generic;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI
{
	public class Options : Analysis
	{
		public override event EventHandler<SendConsecutive> Send;
		public override Catalog.Models.Balance OnReceiveBalance<T>(T param) where T : struct
		{
			if (param is Catalog.OpenAPI.Derivatives bal
				&& double.TryParse(bal.Offer[0] is '-' ? bal.Offer[1..] : bal.Offer, out double offer)
				&& double.TryParse(bal.Bid[0] is '-' ? bal.Bid[1..] : bal.Bid, out double bid)
				&& double.TryParse(bal.Purchase[0] is '-' ? bal.Purchase[1..] : bal.Purchase, out double unit)
				&& double.TryParse(bal.Unit, out double transaction) && int.TryParse(bal.Quantity, out int amount)
				&& double.TryParse(bal.Current[0] is '-' ? bal.Current[1..] : bal.Current, out double price))
			{
				var classification = bal.TradingClassification[0] is '1' ? -1 : 1;
				Current = price;
				Balance.Quantity = amount * classification;
				Balance.Purchase = unit;
				Balance.Revenue = (long)((price - unit) * classification * amount * transaction);
				Balance.Rate = price / unit - 1;
				Bid = bid;
				Offer = offer;
				Wait = true;
			}
			return new Catalog.Models.Balance
			{
				Code = Code,
				Name = Balance.Name,
				Quantity = Balance.Quantity.ToString("N0"),
				Purchase = Balance.Purchase.ToString("N2"),
				Current = Current.ToString("N2"),
				Revenue = Balance.Revenue.ToString("C0"),
				Rate = Balance.Rate.ToString("P2")
			};
		}
		public override void OnReceiveConclusion<T>(T param) where T : struct
		{
			if (param is Catalog.OpenAPI.Conclusion con
				&& double.TryParse(con.CurrentPrice[0] is '-' ? con.CurrentPrice[1..] : con.CurrentPrice, out double current))
			{
				var remove = true;
				Current = current;

				switch (con.OrderState)
				{
					case conclusion:
						if (OrderNumber.Remove(con.OrderNumber))
							remove = false;

						break;

					case acceptance when con.UnsettledQuantity[0] is not '0':
						if (double.TryParse(con.OrderPrice, out double price) && price > 0)
							OrderNumber[con.OrderNumber] = price;

						break;

					case confirmation when con.OrderClassification.EndsWith(cancellantion) || con.OrderClassification.EndsWith(correction):
						remove = OrderNumber.Remove(con.OriginalOrderNumber);
						break;
				}
				Wait = remove;
			}
		}
		public override (IEnumerable<Collect>, uint, uint, string) SortTheRecordedInformation => base.SortTheRecordedInformation;
		public override bool Collector
		{
			get; set;
		}
		public override bool Wait
		{
			get; set;
		}
		public override string Code
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
		public override double Capital
		{
			get; protected internal set;
		}
		public override Balance Balance
		{
			get; set;
		}
		public override Interface.IStrategics Strategics
		{
			get; set;
		}
		public override Queue<Collect> Collection
		{
			get; set;
		}
		public override Dictionary<string, dynamic> OrderNumber
		{
			get; set;
		}
		public override Stack<double> Short
		{
			protected internal get; set;
		}
		public override Stack<double> Long
		{
			protected internal get; set;
		}
		public override Stack<double> Trend
		{
			protected internal get; set;
		}
		protected internal override DateTime NextOrderTime
		{
			get; set;
		}
		protected internal override string DateChange
		{
			get; set;
		}
		protected internal override bool GetCheckOnDate(string date) => throw new NotImplementedException();
		protected internal override bool GetCheckOnDeadline(string time) => throw new NotImplementedException();
		public override void AnalyzeTheConclusion(string[] param)
		{
			if (int.TryParse(param[^1], out int volume))
				Send?.Invoke(this, new SendConsecutive(new Catalog.Strategics.Charts
				{
					Date = param[0],
					Price = param[1],
					Volume = volume
				}));
		}
		public override void AnalyzeTheQuotes(string[] param)
		{
			//?.Invoke(this, new SendConsecutive(param));
		}
		public override void OnReceiveMatrix(object sender, SendConsecutive e)
		{

		}
		public override void OnReceiveDrawChart(object sender, SendConsecutive e)
		{

		}
	}
}