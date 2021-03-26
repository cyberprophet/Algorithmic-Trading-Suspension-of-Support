using System;
using System.Collections.Generic;
using System.Linq;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.SecondaryIndicators.OpenAPI
{
	public class Stocks : Analysis
	{
		public override event EventHandler<SendSecuritiesAPI> Send;
		public override event EventHandler<SendConsecutive> Consecutive;
		public override Balance OnReceiveBalance(string kiwoom, Dictionary<int, string> balance)
		{
			string str_purchase = balance[0x3A3], str_quantity = balance[0x3A2], str_current = balance[0xA], str_offer = balance[0x1B], str_bid = balance[0x1C];

			if (int.TryParse(str_purchase, out int purchase) && int.TryParse(str_quantity, out int quantity) && int.TryParse(str_current[0] is '-' ? str_current[1..] : str_current, out int current) && int.TryParse(str_offer[0] is '-' ? str_offer[1..] : str_offer, out int offer) && int.TryParse(str_bid[0] is '-' ? str_bid[1..] : str_bid, out int bid))
			{
				Current = current;
				Quantity = quantity;
				Purchase = purchase;
				Revenue = (current - purchase) * quantity;
				Rate = current / (double)purchase - 1;
				Bid = bid;
				Offer = offer;
				Wait = true;

				if (string.IsNullOrEmpty(Name))
					Name = balance[0x12E].Trim();
			}
			return new Balance
			{
				Kiwoom = kiwoom,
				Code = Code,
				Name = Name,
				Quantity = Quantity.ToString("N0"),
				Purchase = Purchase.ToString("N0"),
				Current = Current.ToString("N0"),
				Revenue = Revenue.ToString("C0"),
				Rate = Rate.ToString("P2"),
				Account = balance[0x23F1].Substring(0, 8).Insert(4, "－")
			};
		}
		public override int OnReceiveConclusion(Dictionary<int, string> on)
		{
			string price = on[0xA], state = on[0x391], number = on[0x23F3], classification = on[0x389], original = on[0x388];
			var cash = 0;

			if (int.TryParse(price[0] is '-' ? price[1..] : price, out int current))
			{
				var remove = true;
				Current = current;

				switch (state)
				{
					case conclusion:
						if (OrderNumber.Remove(number))
							remove = false;

						break;

					case acceptance when on[0x386][0] is not '0':
						if (int.TryParse(on[0x385], out int op) && op > 0)
						{
							OrderNumber[number] = op;
							cash -= op;
						}
						if (string.IsNullOrEmpty(Name))
							Name = on[0x12E].Trim();

						break;

					case confirmation when classification.EndsWith(cancellantion) || classification.EndsWith(correction):
						if (classification.EndsWith(cancellantion) && OrderNumber.TryGetValue(original, out dynamic order_price))
							cash = (order_price < Current ? order_price : 0) * (int.TryParse(on[0x384], out int volume) ? volume : 0);

						remove = OrderNumber.Remove(original);
						break;
				}
				Wait = remove;
			}
			return cash;
		}
		public override void OnReceiveEvent(string time, string price, string volume)
		{
			if (int.TryParse(volume, out int vol))
			{
				var consecutive = new SendConsecutive(time, price, vol);

				if (Current != consecutive.Price)
				{
					Revenue = (long)((consecutive.Price - Purchase) * Quantity);
					Rate = consecutive.Price / (double)Purchase - 1;
					Current = consecutive.Price;
				}
				if (Classification is Interface.IStrategics)
					Consecutive?.Invoke(this, consecutive);
			}
		}
		public override void OnReceiveDrawChart(object sender, SendConsecutive e)
		{
			switch (Classification)
			{
				case Catalog.Scenario scenario when Wait:
					if (Offer > 0 && Quantity > 0 && scenario.Hope * (1 + scenario.Target + Base.Tax) < Offer && e.Date.CompareTo(NextOrderStringTime.Item1) > 0 && (OrderNumber is null || OrderNumber.Any(o => o.Value >= e.Price) is false || Quantity > OrderNumber.Count(o => o.Value >= e.Price) * scenario.Short))
					{
						Wait = false;
						Send?.Invoke(this, new SendSecuritiesAPI(new Catalog.OpenAPI.Order
						{
							AccNo = scenario.Account,
							Code = scenario.Code,
							OrderType = (int)OrderType.신규매도,
							HogaGb = ((int)HogaGb.지정가).ToString("D2"),
							OrgOrderNo = string.Empty,
							Price = Offer,
							Qty = OrderNumber is null ? 1 : (scenario.Short < Quantity - (OrderNumber.Any(o => o.Value >= e.Price) ? OrderNumber.Count(o => o.Value >= e.Price) : 0) ? scenario.Short : 1)
						}));
						if (Reservation.Item1.TryDequeue(out string next))
							NextOrderStringTime = (e.Date.CompareTo(next) < 0 ? next : e.Date, NextOrderStringTime.Item2);

						else
							NextOrderStringTime = (scenario.Account, NextOrderStringTime.Item2);

						return;
					}
					if (Bid > 0 && Bid < scenario.Hope * (1 - Base.Tax) && e.Date.CompareTo(NextOrderStringTime.Item2) > 0 && scenario.Maximum * (1 - Base.Tax) > (long)e.Price * (OrderNumber is null || OrderNumber.Any(o => o.Value <= e.Price) is false ? Quantity : Quantity + scenario.Long * OrderNumber.Count(o => o.Value <= e.Price)))
					{
						Wait = false;
						Send?.Invoke(this, new SendSecuritiesAPI(new Catalog.OpenAPI.Order
						{
							AccNo = scenario.Account,
							Code = scenario.Code,
							OrderType = (int)OrderType.신규매수,
							HogaGb = ((int)HogaGb.지정가).ToString("D2"),
							OrgOrderNo = string.Empty,
							Price = Bid,
							Qty = scenario.Long
						}));
						if (Reservation.Item2.TryDequeue(out string next))
							NextOrderStringTime = (NextOrderStringTime.Item1, e.Date.CompareTo(next) < 0 ? next : e.Date);

						else
							NextOrderStringTime = (NextOrderStringTime.Item1, scenario.Account);

						return;
					}
					break;

				case Catalog.LongPosition lop when Wait && (Quantity > 0 && double.IsNaN(Purchase) is false && Purchase > 0 ? e.Price / Purchase - 1 : 0D) is double rate:
					if (Offer > 0 && lop.Underweight + Base.Tax < rate && (OrderNumber is null || OrderNumber.ContainsValue(Offer) is false) && e.Date.CompareTo(NextOrderStringTime.Item1) > 0)
					{
						Wait = false;
						Send?.Invoke(this, new SendSecuritiesAPI(new Catalog.OpenAPI.Order
						{
							AccNo = lop.Account,
							Code = lop.Code,
							OrderType = (int)OrderType.신규매도,
							HogaGb = ((int)HogaGb.지정가).ToString("D2"),
							OrgOrderNo = string.Empty,
							Price = Offer,
							Qty = 1
						}));
						NextOrderStringTime = (e.Date, NextOrderStringTime.Item2);
						return;
					}
					if (Bid > 0 && (long)Quantity * e.Price < lop.Overweight * (1 - Base.Tax) && lop.Underweight - Base.Tax > rate && (OrderNumber is null || OrderNumber.ContainsValue(Bid) is false) && e.Date.CompareTo(NextOrderStringTime.Item2) > 0)
					{
						Wait = false;
						Send?.Invoke(this, new SendSecuritiesAPI(new Catalog.OpenAPI.Order
						{
							AccNo = lop.Account,
							Code = lop.Code,
							OrderType = (int)OrderType.신규매수,
							HogaGb = ((int)HogaGb.지정가).ToString("D2"),
							OrgOrderNo = string.Empty,
							Price = Bid,
							Qty = 1
						}));
						NextOrderStringTime = (NextOrderStringTime.Item1, e.Date);
						return;
					}
					break;

				case Catalog.SatisfyConditionsAccordingToTrends sc:
					if (GetCheckOnDate(e.Date))
					{
						Trend.Pop();
						Short.Pop();
						Long.Pop();
					}
					Trend.Push(Trend.TryPeek(out double tp) ? EMA.Make(Classification.Trend, Trend.Count, e.Price, tp) : EMA.Make(e.Price));
					Short.Push(Short.TryPeek(out double sp) ? EMA.Make(Classification.Short, Short.Count, e.Price, sp) : EMA.Make(e.Price));
					Long.Push(Long.TryPeek(out double lp) ? EMA.Make(Classification.Long, Long.Count, e.Price, lp) : EMA.Make(e.Price));

					if (Short.TryPop(out double popShort) && Long.TryPop(out double popLong) && Short.TryPeek(out double short_peek) && Long.TryPeek(out double long_peek) && Trend.TryPeek(out double peek))
					{
						var gap = popShort - popLong - (short_peek - long_peek);
						var interval = new DateTime(NextOrderTime.Year, NextOrderTime.Month, NextOrderTime.Day, int.TryParse(e.Date.Substring(0, 2), out int cHour) ? cHour : DateTime.Now.Hour, int.TryParse(e.Date.Substring(2, 2), out int cMinute) ? cMinute : DateTime.Now.Minute, int.TryParse(e.Date[4..], out int cSecond) ? cSecond : DateTime.Now.Second);

						if (Bid > 0 && sc.TradingBuyQuantity > 0 && Bid < peek * (1 - sc.TradingBuyRate) && gap > 0 && OrderNumber.ContainsValue(Bid) is false && Wait && (sc.TradingBuyInterval == 0 || sc.TradingBuyInterval > 0 && interval.CompareTo(NextOrderTime) > 0))
						{
							Wait = false;
							Send?.Invoke(this, new SendSecuritiesAPI(new Catalog.OpenAPI.Order
							{
								Code = Code,
								OrderType = (int)OrderType.신규매수,
								HogaGb = ((int)HogaGb.지정가).ToString("D2"),
								OrgOrderNo = string.Empty,
								Price = Bid,
								Qty = sc.TradingBuyQuantity,
								AccNo = Account
							}));
							if (sc.TradingBuyInterval > 0)
								NextOrderTime = Base.MeasureTheDelayTime(sc.TradingBuyInterval * (Purchase > 0 && Bid > 0 ? Purchase / (double)Bid : 1), interval);
						}
						else if (Quantity > 0)
						{
							if (sc.TradingSellQuantity > 0 && Offer > peek * (1 + sc.TradingSellRate) && Offer > Purchase + Base.Tax * Offer && gap < 0 && OrderNumber.ContainsValue(Offer) is false && Wait && (sc.TradingSellInterval == 0 || sc.TradingSellInterval > 0 && interval.CompareTo(NextOrderTime) > 0))
							{
								Wait = false;
								Send?.Invoke(this, new SendSecuritiesAPI(new Catalog.OpenAPI.Order
								{
									Code = Code,
									OrderType = (int)OrderType.신규매도,
									HogaGb = ((int)HogaGb.지정가).ToString("D2"),
									OrgOrderNo = string.Empty,
									Price = Offer,
									Qty = sc.TradingSellQuantity,
									AccNo = Account
								}));
								if (sc.TradingSellInterval > 0)
									NextOrderTime = Base.MeasureTheDelayTime(sc.TradingSellInterval * (Purchase > 0 && Offer > 0 ? Offer / (double)Purchase : 1), interval);
							}
							else if (SellPrice > 0 && sc.ReservationSellQuantity > 0 && Offer > SellPrice && OrderNumber.ContainsValue(Offer) is false && Wait)
							{
								for (int i = 0; i < sc.ReservationSellUnit; i++)
									SellPrice += Base.GetQuoteUnit(SellPrice, MarketMarginRate == 1);

								Send?.Invoke(this, new SendSecuritiesAPI(new Catalog.OpenAPI.Order
								{
									Code = Code,
									OrderType = (int)OrderType.신규매도,
									HogaGb = ((int)HogaGb.지정가).ToString("D2"),
									OrgOrderNo = string.Empty,
									Price = Offer,
									Qty = sc.ReservationSellQuantity,
									AccNo = Account
								}));
								Wait = false;
							}
							else if (Bid > 0 && BuyPrice > 0 && sc.ReservationBuyQuantity > 0 && Bid < BuyPrice && OrderNumber.ContainsValue(Bid) is false && Wait)
							{
								for (int i = 0; i < sc.ReservationBuyUnit; i++)
									BuyPrice -= Base.GetQuoteUnit(BuyPrice, MarketMarginRate == 1);

								Send?.Invoke(this, new SendSecuritiesAPI(new Catalog.OpenAPI.Order
								{
									Code = Code,
									OrderType = (int)OrderType.신규매수,
									HogaGb = ((int)HogaGb.지정가).ToString("D2"),
									OrgOrderNo = string.Empty,
									Price = Bid,
									Qty = sc.ReservationBuyQuantity,
									AccNo = Account
								}));
								Wait = false;
							}
							else if (SellPrice == 0 && Purchase > 0)
								SellPrice = Base.GetStartingPrice((int)((1 + sc.ReservationSellRate) * Purchase), MarketMarginRate == 1);

							else if (BuyPrice == 0 && Purchase > 0)
								BuyPrice = Base.GetStartingPrice((int)(Purchase * (1 - sc.ReservationBuyRate)), MarketMarginRate == 1);
						}
						Gap = gap;
						Peek = peek;
						Short.Push(popShort);
						Long.Push(popLong);
					}
					break;
			}
		}
		public override bool GetCheckOnDate(string date)
		{
			var line = date.Equals(DateLine);

			switch (Strategics)
			{
				case Interface.Strategics.SC:
					line = string.IsNullOrEmpty(DateLine) is false;
					break;
			}
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
		public override Tuple<Queue<string>, Queue<string>> Reservation
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
		protected internal override double Gap
		{
			get; set;
		}
		protected internal override double Peek
		{
			get; set;
		}
		protected internal override string DateLine
		{
			get; set;
		}
		protected internal override (string, string) NextOrderStringTime
		{
			get; set;
		}
		protected internal override DateTime NextOrderTime
		{
			get; set;
		}
	}
}