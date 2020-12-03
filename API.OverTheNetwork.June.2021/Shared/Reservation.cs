using System.Collections.Generic;
using System.Linq;

namespace ShareInvest
{
	class Reservation
	{
		internal Reservation(dynamic list)
		{
			foreach (var kv in list)
				if (kv.Value is Analysis reservation)
				{
					if (this.list is null)
						this.list = new List<Analysis>();

					this.list.Add(reservation);
					Base.SendMessage(kv.Value.Balance.Name, kv.Value.Balance.Quantity, GetType());
				}
		}
		internal IEnumerable<KeyValuePair<ulong, string>> Stocks
		{
			get
			{
				var reservation = new Dictionary<ulong, string>();
				var index = ulong.MaxValue;

				foreach (var r in list)
				{
					var stock = r.Market;
					int type, price = (int)r.Current, upper = (int)(price * 1.3), lower = (int)(price * 0.7), buy, sell, quantity = r.Balance.Quantity;

					switch (r.Strategics)
					{
						case Catalog.SatisfyConditionsAccordingToTrends sc:
							if (sc.ReservationSellQuantity > 0)
							{
								sell = Base.GetStartingPrice((int)(r.Balance.Purchase * (1 + sc.ReservationSellRate)), stock);
								sell = sell < lower ? lower + r.GetQuoteUnit(sell, stock) : sell;
								r.SellPrice = sell;
								type = (int)Interface.OpenAPI.OrderType.신규매도;

								while (sell < upper && quantity-- > 0)
								{
									reservation[Base.MakeKey(index, type, r.Code)]
										= Strategics.SetOrder(r.Code, type, sell, sc.ReservationSellQuantity, ((int)Interface.OpenAPI.HogaGb.지정가).ToString("D2"), string.Empty);
									index -= 0x989680;

									for (int i = 0; i < sc.ReservationSellUnit; i++)
										sell += r.GetQuoteUnit(sell, stock);
								}
								index = ulong.MaxValue;
							}
							if (sc.ReservationBuyQuantity > 0)
							{
								buy = Base.GetStartingPrice((int)(r.Balance.Purchase * (1 - sc.ReservationBuyRate)), stock);
								buy = buy > upper ? upper - r.GetQuoteUnit(buy, stock) : buy;
								r.BuyPrice = buy;
								type = (int)Interface.OpenAPI.OrderType.신규매수;

								while (buy > lower && Strategics.Cash > buy * (1.5e-4 + 1))
								{
									Strategics.Cash -= (long)(buy * (1.5e-4 + 1));
									reservation[Base.MakeKey(index, type, r.Code)]
										= Strategics.SetOrder(r.Code, type, buy, sc.ReservationBuyQuantity, ((int)Interface.OpenAPI.HogaGb.지정가).ToString("D2"), string.Empty);
									index -= 0x989680;

									for (int i = 0; i < sc.ReservationBuyUnit; i++)
										buy -= r.GetQuoteUnit(buy, stock);
								}
								index = ulong.MaxValue;
							}
							Base.SendMessage(r.Balance.Name, reservation.Count, sc.GetType());
							break;
					}
				}
				return reservation.OrderByDescending(o => o.Key);
			}
		}
		readonly List<Analysis> list;
	}
}