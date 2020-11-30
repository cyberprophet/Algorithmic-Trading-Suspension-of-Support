using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareInvest
{
	class Reservation
	{
		internal Reservation() => Append = new Queue<Analysis>();
		internal (Queue<string>, Stack<string>) Stocks
		{
			get
			{
				var sell_queue = new Queue<string>();
				var buy_stack = new Stack<string>();
				Parallel.ForEach(Append, new Action<Analysis>((r) =>
				{
					switch (r.Strategics)
					{
						case Catalog.SatisfyConditionsAccordingToTrends sc:
							int sell, buy, price = r.Current, upper = (int)(price * 1.3), lower = (int)(price * 0.7), bPrice, sPrice, quantity = r.Balance.Quantity;
							var stock = r.Market;

							if (sc.ReservationSellQuantity > 0)
							{
								sell = (int)(r.Balance.Purchase * (1 + sc.ReservationSellRate));
								sPrice = Base.GetStartingPrice(sell, stock);
								sPrice = sPrice < lower ? lower + r.GetQuoteUnit(sPrice, stock) : sPrice;
								r.SellPrice = sPrice;

								while (sPrice < upper && quantity-- > 0)
								{
									Base.SendMessage(r.Code, sPrice.ToString("C0"), typeof(Strategics));
									sell_queue.Enqueue(Strategics.SetOrder(r.Code, (int)Interface.OpenAPI.OrderType.신규매도, sPrice, sc.ReservationSellQuantity, ((int)Interface.OpenAPI.HogaGb.지정가).ToString("D2"), string.Empty));

									for (int i = 0; i < sc.ReservationSellUnit; i++)
										sPrice += r.GetQuoteUnit(sPrice, stock);
								}
							}
							if (sc.ReservationBuyQuantity > 0)
							{
								buy = (int)(r.Balance.Purchase * (1 - sc.ReservationBuyRate));
								r.BuyPrice = Base.GetStartingPrice(buy, stock);
								bPrice = Base.GetStartingPrice(lower, stock);

								while (bPrice < upper && bPrice < buy && Strategics.Cash > bPrice * (1.5e-4 + 1))
								{
									buy_stack.Push(Strategics.SetOrder(r.Code, (int)Interface.OpenAPI.OrderType.신규매수, bPrice, sc.ReservationBuyQuantity, ((int)Interface.OpenAPI.HogaGb.지정가).ToString("D2"), string.Empty));
									Strategics.Cash -= (long)(bPrice * (1.5e-4 + 1));
									Base.SendMessage(r.Code, string.Concat(bPrice.ToString("C0"), Strategics.Cash.ToString("C0")), typeof(Strategics));

									for (int i = 0; i < sc.ReservationBuyUnit; i++)
										bPrice += r.GetQuoteUnit(bPrice, stock);
								}
							}
							break;
					}
				}));
				return (sell_queue, buy_stack);
			}
		}
		internal Queue<Analysis> Append
		{
			get;
		}
	}
}