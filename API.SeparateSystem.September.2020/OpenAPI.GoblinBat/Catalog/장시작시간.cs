using System;

using AxKHOpenAPILib;

using ShareInvest.Analysis;
using ShareInvest.Catalog;
using ShareInvest.Catalog.OpenAPI;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
	class 장시작시간 : Real, ISendSecuritiesAPI<SendSecuritiesAPI>
	{
		public event EventHandler<SendSecuritiesAPI> Send;
		internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
		{
			var param = base.OnReceiveRealData(e, fid);
			int arg = int.MinValue;

			if (int.TryParse(param[1].Substring(0, 2), out int conclusion))
			{
				switch (param[0])
				{
					case "0":
						if (Reservation.Equals(param[1]))
						{
							arg = (int)Operation.장시작전;
							Delay.Milliseconds = 0xE7;
						}
						break;

					case "3":
						foreach (var holding in Connect.HoldingStock)
							holding.Value.WaitOrder = true;

						arg = (int)Operation.장시작;
						DeadLine = true;
						Delay.Milliseconds = 0xC9;
						break;

					case "e" when DeadLine:
						arg = (int)Operation.선옵_장마감전_동시호가_종료;
						DeadLine = false;
						Delay.Milliseconds = 0xE17;
						break;

					case "8":
						arg = (int)Operation.장마감;
						DeadLine = false;
						Delay.Milliseconds = 0xE11;
						break;

					case "d":
						arg = (int)Operation.시간외_단일가_매매종료;
						break;

					case "2":
						if (Quote.Equals(param[1]))
						{
							int bPrice, sPrice, quantity, price, upper, lower;
							bool stock;
							var connect = Connect.GetInstance(API);
							Delay.Milliseconds = 0xCF;

							foreach (var kv in Connect.HoldingStock)
								switch (kv.Value.FindStrategics)
								{
									case SatisfyConditionsAccordingToTrends sc:
										quantity = kv.Value.Quantity;
										price = int.TryParse(API.GetMasterLastPrice(sc.Code), out int eve) ? eve : 0;
										upper = (int)(price * 1.3);
										lower = (int)(price * 0.7);
										stock = API.KOA_Functions(info, sc.Code).Split(';')[0].Contains(market);

										if (sc.ReservationSellQuantity > 0)
										{
											sPrice = kv.Value.GetStartingPrice((int)((kv.Value.Purchase ?? 0D) * (1 + sc.ReservationSellRate)), stock);
											sPrice = sPrice < lower ? lower + kv.Value.GetQuoteUnit(sPrice, stock) : sPrice;

											while (sPrice < upper && quantity-- > 0)
											{
												if (kv.Value.OrderNumber.ContainsValue(sPrice))
													SendMessage(sPrice.ToString("C0"));

												else
													connect.SendOrder(new SendOrder
													{
														RQName = API.GetMasterCodeName(sc.Code),
														ScreenNo = connect.LookupScreenNo,
														AccNo = Account,
														OrderType = (int)OpenOrderType.신규매도,
														Code = sc.Code,
														Qty = sc.ReservationSellQuantity,
														Price = sPrice,
														HogaGb = ((int)HogaGb.지정가).ToString("D2"),
														OrgOrderNo = string.Empty
													});
												for (int i = 0; i < sc.ReservationSellUnit; i++)
													sPrice += kv.Value.GetQuoteUnit(sPrice, stock);
											}
										}
										if (sc.ReservationBuyQuantity > 0)
										{
											bPrice = kv.Value.GetStartingPrice((int)((kv.Value.Purchase ?? 0D) * (1 - sc.ReservationBuyRate)), stock);

											while (bPrice > lower && Connect.Cash > bPrice * (15e-5 + 1))
											{
												if (kv.Value.OrderNumber.ContainsValue(bPrice))
													SendMessage(Connect.Cash.ToString("C0"));

												else
												{
													connect.SendOrder(new SendOrder
													{
														RQName = API.GetMasterCodeName(sc.Code),
														ScreenNo = connect.LookupScreenNo,
														AccNo = Account,
														OrderType = (int)OpenOrderType.신규매수,
														Code = sc.Code,
														Qty = sc.ReservationBuyQuantity,
														Price = bPrice,
														HogaGb = ((int)HogaGb.지정가).ToString("D2"),
														OrgOrderNo = string.Empty
													});
													Connect.Cash -= (long)(bPrice * (15e-5 + 1));
												}
												for (int i = 0; i < sc.ReservationBuyUnit; i++)
													bPrice -= kv.Value.GetQuoteUnit(bPrice, stock);
											}
										}
										break;

									case TrendsInValuation tv:
										foreach (var order in kv.Value.OrderNumber)
										{
											var send = new SendOrder
											{
												RQName = API.GetMasterCodeName(tv.Code),
												ScreenNo = connect.LookupScreenNo,
												AccNo = Account,
												OrderType = (int)(kv.Value.Current > order.Value ? OpenOrderType.매수취소 : OpenOrderType.매도취소),
												Code = tv.Code,
												Qty = kv.Value.Current > order.Value ? tv.ReservationAddtionalQuantity : tv.ReservationSubtractionalQuantity,
												Price = order.Value,
												HogaGb = ((int)HogaGb.지정가).ToString("D2"),
												OrgOrderNo = order.Key
											};
											connect.SendOrder(send);
											Connect.Cash += (send.OrderType == 3 ? send.Price : 0);
										}
										quantity = kv.Value.Quantity;
										price = int.TryParse(API.GetMasterLastPrice(tv.Code), out int before) ? before : 0;
										upper = (int)(price * 1.3);
										lower = (int)(price * 0.7);
										stock = API.KOA_Functions(info, tv.Code).Split(';')[0].Contains(market);

										if (tv.ReservationSubtractionalQuantity > 0)
										{
											sPrice = kv.Value.GetStartingPrice((int)((kv.Value.Purchase ?? 0D) * (1 + tv.Subtraction)), stock);
											sPrice = sPrice < lower ? lower + kv.Value.GetQuoteUnit(sPrice, stock) : sPrice;

											while (sPrice < upper && quantity-- > 0)
											{
												SendMessage(sPrice.ToString("C0"));
												connect.SendOrder(new SendOrder
												{
													RQName = API.GetMasterCodeName(tv.Code),
													ScreenNo = connect.LookupScreenNo,
													AccNo = Account,
													OrderType = (int)OpenOrderType.신규매도,
													Code = tv.Code,
													Qty = tv.ReservationSubtractionalQuantity,
													Price = sPrice,
													HogaGb = ((int)HogaGb.지정가).ToString("D2"),
													OrgOrderNo = string.Empty
												});
												for (int i = 0; i < tv.SubtractionalUnit; i++)
													sPrice += kv.Value.GetQuoteUnit(sPrice, stock);
											}
										}
										if (tv.ReservationAddtionalQuantity > 0)
										{
											bPrice = kv.Value.GetStartingPrice((int)((kv.Value.Purchase ?? 0D) * (1 - tv.Addition)), stock);

											while (bPrice > lower && Connect.Cash > bPrice * (1.5e-4 + 1))
											{
												connect.SendOrder(new SendOrder
												{
													RQName = API.GetMasterCodeName(tv.Code),
													ScreenNo = connect.LookupScreenNo,
													AccNo = Account,
													OrderType = (int)OpenOrderType.신규매수,
													Code = tv.Code,
													Qty = tv.ReservationAddtionalQuantity,
													Price = bPrice,
													HogaGb = ((int)HogaGb.지정가).ToString("D2"),
													OrgOrderNo = string.Empty
												});
												for (int i = 0; i < tv.AdditionalUnit; i++)
													bPrice -= kv.Value.GetQuoteUnit(bPrice, stock);

												Connect.Cash -= (long)(bPrice * (1.5e-4 + 1));
												SendMessage(Connect.Cash.ToString("C0"));
											}
										}
										break;
								}
						}
						return;
				}
				if (arg > int.MinValue)
					Send?.Invoke(this, new SendSecuritiesAPI(arg, conclusion));
			}
			SendMessage(string.Concat(DeadLine, '_', param[0], '_', param[1], '_', param[2]));
		}
		internal override AxKHOpenAPI API
		{
			get; set;
		}
		bool DeadLine
		{
			get; set;
		}
		bool SAT => Array.Exists(Connect.SAT, o => o.Equals(DateTime.Now.ToString("yyMMdd")));
		string Reservation => SAT ? "095500" : reservation;
		string Quote => SAT && Array.FindIndex(Connect.SAT, o => o.Equals(DateTime.Now.ToString("yyMMdd"))) % 2 == 1 ? "162000" : quote;
		const string market = "거래소";
		const string info = "GetMasterStockInfo";
		const string reservation = "085500";
		const string quote = "152000";
		readonly int[] fid = new int[] { 0xD7, 0x14, 0xD6 };
	}
}