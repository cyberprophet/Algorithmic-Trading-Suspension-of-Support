using System;
using System.Collections.Generic;

using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.SecondaryIndicators.OpenAPI
{
	public class Stocks : Analysis
	{
		public override event EventHandler<SendSecuritiesAPI> Send;
		public override event EventHandler<SendConsecutive> Consecutive;
		public override int OnReceiveConclusion(Dictionary<string, string> on)
		{
			string price = on["현재가"], state = on["주문상태"], number = on["주문번호"], classification = on["주문구분"], original = on["원주문번호"];
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

					case acceptance when on["미체결수량"][0] is not '0':
						if (int.TryParse(on["주문가격"], out int op) && op > 0)
						{
							OrderNumber[number] = op;
							cash -= op;
						}
						if (string.IsNullOrEmpty(Name))
							Name = on["종목명"].Trim();

						break;

					case confirmation when classification.EndsWith(cancellantion) || classification.EndsWith(correction):
						if (classification.EndsWith(cancellantion) && OrderNumber.TryGetValue(original, out dynamic order_price))
							cash = (order_price < Current ? order_price : 0) * (int.TryParse(on["주문수량"], out int volume) ? volume : 0);

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
			if (GetCheckOnDate(e.Date))
			{
				Trend.Pop();
				Short.Pop();
				Long.Pop();
			}
			Trend.Push(Trend.TryPeek(out double tp) ? EMA.Make(Classification.Trend, Trend.Count, e.Price, tp) : EMA.Make(e.Price));
			Short.Push(Short.TryPeek(out double sp) ? EMA.Make(Classification.Short, Short.Count, e.Price, sp) : EMA.Make(e.Price));
			Long.Push(Long.TryPeek(out double lp) ? EMA.Make(Classification.Long, Long.Count, e.Price, lp) : EMA.Make(e.Price));
			SendSecuritiesAPI securities = null;

			switch (Classification)
			{
				case Catalog.SatisfyConditionsAccordingToTrends sc when Short.TryPop(out double popShort) && Long.TryPop(out double popLong):
					if (Short.TryPeek(out double short_peek) && Long.TryPeek(out double long_peek) && Trend.TryPeek(out double peek))
					{
						var gap = popShort - popLong - (short_peek - long_peek);
						var interval = new DateTime(NextOrderTime.Year, NextOrderTime.Month, NextOrderTime.Day, int.TryParse(e.Date.Substring(0, 2), out int cHour) ? cHour : DateTime.Now.Hour, int.TryParse(e.Date.Substring(2, 2), out int cMinute) ? cMinute : DateTime.Now.Minute, int.TryParse(e.Date[4..], out int cSecond) ? cSecond : DateTime.Now.Second);

						if (Bid > 0 && sc.TradingBuyQuantity > 0 && Bid < peek * (1 - sc.TradingBuyRate) && gap > 0 && OrderNumber.ContainsValue(Bid) is false && Wait && (sc.TradingBuyInterval == 0 || sc.TradingBuyInterval > 0 && interval.CompareTo(NextOrderTime) > 0))
						{
							Wait = false;
							securities = new SendSecuritiesAPI(new Catalog.OpenAPI.Order
							{
								Code = Code,
								OrderType = (int)OrderType.신규매수,
								HogaGb = ((int)HogaGb.지정가).ToString("D2"),
								OrgOrderNo = string.Empty,
								Price = Bid,
								Qty = sc.TradingBuyQuantity,
								AccNo = Account
							});
							if (sc.TradingBuyInterval > 0)
								NextOrderTime = Base.MeasureTheDelayTime(sc.TradingBuyInterval * (Purchase > 0 && Bid > 0 ? Purchase / (double)Bid : 1), interval);
						}
						else if (Quantity > 0)
						{
							if (sc.TradingSellQuantity > 0 && Offer > peek * (1 + sc.TradingSellRate) && Offer > Purchase + Base.Tax * Offer && gap < 0 && OrderNumber.ContainsValue(Offer) is false && Wait && (sc.TradingSellInterval == 0 || sc.TradingSellInterval > 0 && interval.CompareTo(NextOrderTime) > 0))
							{
								Wait = false;
								securities = new SendSecuritiesAPI(new Catalog.OpenAPI.Order
								{
									Code = Code,
									OrderType = (int)OrderType.신규매도,
									HogaGb = ((int)HogaGb.지정가).ToString("D2"),
									OrgOrderNo = string.Empty,
									Price = Offer,
									Qty = sc.TradingSellQuantity,
									AccNo = Account
								});
								if (sc.TradingSellInterval > 0)
									NextOrderTime = Base.MeasureTheDelayTime(sc.TradingSellInterval * (Purchase > 0 && Offer > 0 ? Offer / (double)Purchase : 1), interval);
							}
							else if (SellPrice > 0 && sc.ReservationSellQuantity > 0 && Offer > SellPrice && OrderNumber.ContainsValue(Offer) is false && Wait)
							{
								for (int i = 0; i < sc.ReservationSellUnit; i++)
									SellPrice += Base.GetQuoteUnit(SellPrice, MarketMarginRate == 1);

								securities = new SendSecuritiesAPI(new Catalog.OpenAPI.Order
								{
									Code = Code,
									OrderType = (int)OrderType.신규매도,
									HogaGb = ((int)HogaGb.지정가).ToString("D2"),
									OrgOrderNo = string.Empty,
									Price = Offer,
									Qty = sc.ReservationSellQuantity,
									AccNo = Account
								});
								Wait = false;
							}
							else if (Bid > 0 && BuyPrice > 0 && sc.ReservationBuyQuantity > 0 && Bid < BuyPrice && OrderNumber.ContainsValue(Bid) is false && Wait)
							{
								for (int i = 0; i < sc.ReservationBuyUnit; i++)
									BuyPrice -= Base.GetQuoteUnit(BuyPrice, MarketMarginRate == 1);

								securities = new SendSecuritiesAPI(new Catalog.OpenAPI.Order
								{
									Code = Code,
									OrderType = (int)OrderType.신규매수,
									HogaGb = ((int)HogaGb.지정가).ToString("D2"),
									OrgOrderNo = string.Empty,
									Price = Bid,
									Qty = sc.ReservationBuyQuantity,
									AccNo = Account
								});
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
			if (securities?.Convey is Catalog.OpenAPI.Order)
				Send?.Invoke(this, securities);
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
		protected internal override DateTime NextOrderTime
		{
			get; set;
		}
	}
}