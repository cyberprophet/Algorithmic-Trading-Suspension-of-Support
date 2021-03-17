using System.Collections.Generic;
using System.Linq;

using ShareInvest.Interface;

namespace ShareInvest
{
	public class Reservation
	{
		public IEnumerable<KeyValuePair<ulong, ISendOrder>> Stocks
		{
			get
			{
				var reservation = new Dictionary<ulong, ISendOrder>();
				var index = ulong.MaxValue;

				while (Orders.TryPop(out Analysis r))
				{
					var stock = 1 == (int)r.MarketMarginRate;
					int type, price = (int)r.Current, upper = (int)(price * 1.3), lower = (int)(price * 0.7), buy, sell, quantity = r.Quantity;

					switch (r.Strategics)
					{
						case Strategics.Long_Position:
						case Strategics.Scenario:
							continue;

						case Strategics.SC when r.Classification is Catalog.SatisfyConditionsAccordingToTrends sc:
							if (sc.ReservationSellQuantity > 0)
							{
								sell = Base.GetStartingPrice((int)(r.Purchase * (1 + sc.ReservationSellRate)), stock);
								sell = sell < lower ? Base.GetStartingPrice(lower, stock) + Base.GetQuoteUnit(sell, stock) : sell;
								r.SellPrice = sell;
								type = (int)Interface.OpenAPI.OrderType.신규매도;

								while (sell < upper && quantity-- > 0)
								{
									if (r.OrderNumber is null || r.OrderNumber.ContainsValue(sell) is false)
									{
										reservation[Base.MakeKey(index, type, r.Code)] = new Catalog.OpenAPI.Order
										{
											AccNo = account[0],
											Code = r.Code,
											Qty = sc.ReservationSellQuantity,
											Price = sell,
											OrgOrderNo = string.Empty,
											HogaGb = ((int)Interface.OpenAPI.HogaGb.지정가).ToString("D2"),
											OrderType = type
										};
										index -= 0x989680;
										r.Offer = sell;
									}
									for (int i = 0; i < sc.ReservationSellUnit; i++)
										sell += Base.GetQuoteUnit(sell, stock);
								}
								index = ulong.MaxValue;
							}
							if (sc.ReservationBuyQuantity > 0)
							{
								buy = Base.GetStartingPrice((int)(r.Purchase * (1 - sc.ReservationBuyRate)), stock);
								buy = buy > upper ? Base.GetStartingPrice(upper, stock) - Base.GetQuoteUnit(buy, stock) : buy;
								r.BuyPrice = buy;
								type = (int)Interface.OpenAPI.OrderType.신규매수;

								while (buy > lower && Cash > buy * (1.5e-4 + 1))
								{
									if (r.OrderNumber is null || r.OrderNumber.ContainsValue(buy) is false)
									{
										Cash -= (long)(buy * (1.5e-4 + 1));
										reservation[Base.MakeKey(index, type, r.Code)] = new Catalog.OpenAPI.Order
										{
											AccNo = account[0],
											Code = r.Code,
											Qty = sc.ReservationBuyQuantity,
											Price = buy,
											OrgOrderNo = string.Empty,
											HogaGb = ((int)Interface.OpenAPI.HogaGb.지정가).ToString("D2"),
											OrderType = type
										};
										index -= 0x989680;
										r.Bid = buy;
									}
									for (int i = 0; i < sc.ReservationBuyUnit; i++)
										buy -= Base.GetQuoteUnit(buy, stock);
								}
								index = ulong.MaxValue;
							}
							Base.SendMessage(GetType(), r.Name, reservation.Count);
							break;
					}
				}
				return reservation.OrderByDescending(o => o.Key);
			}
		}
		public Reservation(long cash, string[] account)
		{
			this.account = account;
			Cash = cash;
			Orders = new Stack<Analysis>();
		}
		public void Clear() => Orders.Clear();
		public void Push(Analysis stocks_held) => Orders.Push(stocks_held);
		public long Cash
		{
			get; private set;
		}
		Stack<Analysis> Orders
		{
			get; set;
		}
		readonly string[] account;
	}
}