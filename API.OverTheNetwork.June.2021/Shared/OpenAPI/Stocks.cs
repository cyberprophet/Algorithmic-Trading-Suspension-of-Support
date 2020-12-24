using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;
using ShareInvest.SecondaryIndicators;

namespace ShareInvest.OpenAPI
{
	public class Stocks : Analysis
	{
		public override event EventHandler<SendConsecutive> Send;
		public override bool Market
		{
			get; set;
		}
		public override dynamic SellPrice
		{
			protected internal get; set;
		}
		public override dynamic BuyPrice
		{
			protected internal get; set;
		}
		public override Catalog.Models.Balance OnReceiveBalance<T>(T param) where T : struct
		{
			if (param is Catalog.OpenAPI.Balance bal && int.TryParse(bal.Purchase, out int purchase) && int.TryParse(bal.Quantity, out int quantity)
				&& int.TryParse(bal.Current[0] is '-' ? bal.Current[1..] : bal.Current, out int current)
				&& int.TryParse(bal.Offer[0] is '-' ? bal.Offer[1..] : bal.Offer, out int offer)
				&& int.TryParse(bal.Bid[0] is '-' ? bal.Bid[1..] : bal.Bid, out int bid))
			{
				Current = current;
				Balance.Quantity = quantity;
				Balance.Purchase = purchase;
				Balance.Revenue = (current - purchase) * quantity;
				Balance.Rate = current / (double)purchase - 1;
				Bid = bid;
				Offer = offer;
				Wait = true;
			}
			return new Catalog.Models.Balance
			{
				Code = Code,
				Name = Balance.Name,
				Quantity = Balance.Quantity.ToString("N0"),
				Purchase = Balance.Purchase.ToString("N0"),
				Current = Current.ToString("N0"),
				Revenue = Balance.Revenue.ToString("C0"),
				Rate = Balance.Rate.ToString("P2")
			};
		}
		public override void OnReceiveConclusion<T>(T param) where T : struct
		{
			if (param is Catalog.OpenAPI.Conclusion con
				&& int.TryParse(con.CurrentPrice[0] is '-' ? con.CurrentPrice[1..] : con.CurrentPrice, out int current))
			{
				var cash = 0;
				var remove = true;
				Current = current;

				switch (con.OrderState)
				{
					case conclusion:
						if (OrderNumber.Remove(con.OrderNumber))
							remove = false;

						break;

					case acceptance when con.UnsettledQuantity[0] is not '0':
						if (int.TryParse(con.OrderPrice, out int price) && price > 0)
							OrderNumber[con.OrderNumber] = price;

						if (Balance is Balance bal && string.IsNullOrEmpty(bal.Name))
							bal.Name = con.Name.Trim();

						break;

					case confirmation when con.OrderClassification.EndsWith(cancellantion) || con.OrderClassification.EndsWith(correction):
						if (con.OrderClassification.EndsWith(cancellantion) && OrderNumber.TryGetValue(con.OriginalOrderNumber, out dynamic order_price))
							cash = (order_price < Current ? order_price : 0) * (int.TryParse(con.OrderQuantity, out int volume) ? volume : 0);

						remove = OrderNumber.Remove(con.OriginalOrderNumber);
						break;
				}
				Wait = remove;
				ShareInvest.Strategics.Cash += cash;
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
		public override Queue<Collect> Collection
		{
			get; set;
		}
		public override Dictionary<string, dynamic> OrderNumber
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
		public override dynamic Bid
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
		protected internal override double Gap
		{
			get; set;
		}
		protected internal override double Peek
		{
			get; set;
		}
		protected internal override bool GetCheckOnDate(string date) => open_market.Equals(DateChange);
		protected internal override bool GetCheckOnDeadline(string time)
		{
			DateChange = open_market;
			NextOrderTime = DateTime.Now;

			return true;
		}
		public override void AnalyzeTheConclusion(string[] param)
		{
			if (int.TryParse(param[^1], out int volume))
				Send?.Invoke(this, new SendConsecutive(new Catalog.Strategics.Charts
				{
					Date = param[0],
					Price = param[1],
					Volume = volume
				}));
			if (Balance is Balance bal && int.TryParse(param[1][0] is '-' ? param[1][1..] : param[1], out int current))
				if (Current != current)
				{
					bal.Revenue = (long)((current - bal.Purchase) * bal.Quantity);
					bal.Rate = current / (double)bal.Purchase - 1;
					Current = current;
					Client.Local.Instance.PostContext(new Catalog.Models.Balance
					{
						Code = Code,
						Name = bal.Name,
						Quantity = bal.Quantity.ToString("N0"),
						Purchase = bal.Purchase.ToString("N0"),
						Current = current.ToString("N0"),
						Revenue = bal.Revenue.ToString("C0"),
						Rate = bal.Rate.ToString("P2"),
						Trend = Peek.ToString("N0"),
						Separation = Gap.ToString("N2")
					});
				}
		}
		public override void AnalyzeTheQuotes(string[] param)
		{
			if (int.TryParse(param[4][0] is '-' ? param[4][1..] : param[4], out int bid) && int.TryParse(param[1][0] is '-' ? param[1][1..] : param[1], out int offer))
			{
				Offer = offer;
				Bid = bid;
			}
		}
		[SupportedOSPlatform("windows")]
		public override void OnReceiveDrawChart(object sender, SendConsecutive e)
		{
			if (GetCheckOnDate(e.Date))
			{
				Short.Pop();
				Long.Pop();
				Trend.Pop();
			}
			Trend.Push(Trend.Count > 0 ? EMA.Make(Strategics is null ? 0x14 : Strategics.Trend, Trend.Count, (int)e.Price, Trend.Peek()) : EMA.Make((int)e.Price));
			Short.Push(Short.Count > 0 ? EMA.Make(Strategics is null ? 3 : Strategics.Short, Short.Count, (int)e.Price, Short.Peek()) : EMA.Make((int)e.Price));
			Long.Push(Long.Count > 0 ? EMA.Make(Strategics is null ? 0x5A : Strategics.Long, Long.Count, (int)e.Price, Long.Peek()) : EMA.Make((int)e.Price));

			if (GetCheckOnDeadline(e.Date) && Short.Count > 1 && Long.Count > 1 && Trend.Count > 1 && Strategics is Catalog.SatisfyConditionsAccordingToTrends sc)
			{
				if (Balance is null)
				{
					Balance = new Balance
					{
						Market = Code.Length == 8,
						Purchase = 0,
						Quantity = 0,
						Revenue = 0,
						Rate = 0
					};
					SellPrice = 0;
					BuyPrice = 0;
				}
				double popShort = Short.Pop(), popLong = Long.Pop(), gap = popShort - popLong - (Short.Peek() - Long.Peek()), peek = Trend.Peek();
				Short.Push(popShort);
				Long.Push(popLong);
				var interval = new DateTime(NextOrderTime.Year, NextOrderTime.Month, NextOrderTime.Day,
					int.TryParse(e.Date.Substring(0, 2), out int cHour) ? cHour : DateTime.Now.Hour,
						int.TryParse(e.Date.Substring(2, 2), out int cMinute) ? cMinute : DateTime.Now.Minute,
							int.TryParse(e.Date[4..], out int cSecond) ? cSecond : DateTime.Now.Second);

				if (Bid > 0 && sc.TradingBuyQuantity > 0 && Bid < peek * (1 - sc.TradingBuyRate) && gap > 0 && OrderNumber.ContainsValue(Bid) == false
					&& Wait && (sc.TradingBuyInterval == 0 || sc.TradingBuyInterval > 0 && interval.CompareTo(NextOrderTime) > 0))
				{
					Pipe.Server.WriteLine(string.Concat(order,
						ShareInvest.Strategics.SetOrder(sc.Code, (int)OrderType.신규매수, Bid, sc.TradingBuyQuantity, ((int)HogaGb.지정가).ToString("D2"), string.Empty)));
					Wait = false;

					if (sc.TradingBuyInterval > 0)
						NextOrderTime
							= Base.MeasureTheDelayTime(sc.TradingBuyInterval * (Balance.Purchase > 0 && Bid > 0 ? Balance.Purchase / (double)Bid : 1), interval);
				}
				else if (Balance.Quantity > 0)
				{
					if (sc.TradingSellQuantity > 0 && Offer > peek * (1 + sc.TradingSellRate) && Offer > Balance.Purchase + tax * Offer
						&& gap < 0 && OrderNumber.ContainsValue(Offer) == false && Wait
						&& (sc.TradingSellInterval == 0 || sc.TradingSellInterval > 0 && interval.CompareTo(NextOrderTime) > 0))
					{
						Pipe.Server.WriteLine(string.Concat(order,
							ShareInvest.Strategics.SetOrder(sc.Code, (int)OrderType.신규매도, Offer, sc.TradingSellQuantity, ((int)HogaGb.지정가).ToString("D2"), string.Empty)));
						Wait = false;

						if (sc.TradingSellInterval > 0)
							NextOrderTime
								= Base.MeasureTheDelayTime(sc.TradingSellInterval * (Balance.Purchase > 0 && Offer > 0 ? Offer / (double)Balance.Purchase : 1), interval);
					}
					else if (SellPrice > 0 && sc.ReservationSellQuantity > 0 && Offer > SellPrice && OrderNumber.ContainsValue(Offer) == false && Wait)
					{
						for (int i = 0; i < sc.ReservationSellUnit; i++)
							SellPrice += Base.GetQuoteUnit(SellPrice, Market);

						Pipe.Server.WriteLine(string.Concat(order,
							ShareInvest.Strategics.SetOrder(sc.Code, (int)OrderType.신규매도, Offer, sc.ReservationSellQuantity, ((int)HogaGb.지정가).ToString("D2"), string.Empty)));
						Wait = false;
					}
					else if (Bid > 0 && BuyPrice > 0 && sc.ReservationBuyQuantity > 0 && Bid < BuyPrice && OrderNumber.ContainsValue(Bid) == false && Wait)
					{
						for (int i = 0; i < sc.ReservationBuyUnit; i++)
							BuyPrice -= Base.GetQuoteUnit(BuyPrice, Market);

						Pipe.Server.WriteLine(string.Concat(order,
							ShareInvest.Strategics.SetOrder(sc.Code, (int)OrderType.신규매수, Bid, sc.ReservationBuyQuantity, ((int)HogaGb.지정가).ToString("D2"), string.Empty)));
						Wait = false;
					}
					else if (SellPrice == 0 && Balance.Purchase > 0)
						SellPrice = Base.GetStartingPrice((int)((1 + sc.ReservationSellRate) * Balance.Purchase), Market);

					else if (BuyPrice == 0 && Balance.Purchase > 0)
						BuyPrice = Base.GetStartingPrice((int)(Balance.Purchase * (1 - sc.ReservationBuyRate)), Market);
				}
				Gap = gap;
				Peek = peek;
			}
		}
		const string order = "Order|";
	}
}