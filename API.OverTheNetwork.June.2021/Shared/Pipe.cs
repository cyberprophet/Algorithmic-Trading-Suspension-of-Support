using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using ShareInvest.Catalog.Models;

namespace ShareInvest
{
	[SupportedOSPlatform("windows")]
	static class Pipe
	{
		internal static StreamWriter Server
		{
			get; set;
		}
		internal static void OnReceivePipeClientMessage(NamedPipeClientStream client, NamedPipeServerStream server)
		{
			Task stocks_task = null, futures_task = null;
			DateTime today = DateTime.Now, now = today.Hour > 0xF ? today.AddDays(Base.IsDebug ? 0 : 1) : today;
			bool repeat = true, collection = false, stocks = false, futures = false, sat = Base.CheckIfMarketDelay(now);
			using (var sr = new StreamReader(client))
				try
				{
					while (client.IsConnected)
					{
						var param = sr.ReadLine();

						if (string.IsNullOrEmpty(param) == false)
						{
							string[] temp = param.Split('|'), price;

							if ((temp[0].Length < 5 || temp[0].Length > 5 && temp[0].Length < 0xA) && collection
								&& Progress.Collection.TryGetValue(temp[1], out Analysis analysis))
								switch (temp[0].Length)
								{
									case 4 when temp[0].Equals("주식시세") == false && (stocks && temp[1].Length == 6 || temp[1].Length == 8 && futures):
										price = temp[^1].Split(';');
										analysis.AnalyzeTheConclusion(price);
										analysis.Collection.Enqueue(new Collect
										{
											Time = price[0],
											Datum = temp[^1][7..]
										});
										if (analysis.Collector == false && analysis.Collection.Count < 2)
										{
											analysis.Collector = true;
											analysis.Send += analysis.OnReceiveDrawChart;
											analysis.Wait = true;
										}
										break;

									case 6 when temp[0].Equals("주식우선호가") == false && analysis.Collector:
										price = temp[^1].Split(';');
										new Task(() => analysis.AnalyzeTheQuotes(price)).Start();
										analysis.Collection.Enqueue(new Collect
										{
											Time = price[0],
											Datum = temp[^1][7..]
										});
										break;

									case 8 when temp[1][1] > '0':
									case 4 when temp[0].Equals("주식시세"):
										price = temp[^1].Split(';');
										analysis.Current = int.TryParse(price[0][0] == '-' ? price[0][1..] : price[0], out int current) ? current : 0;
										analysis.Offer = int.TryParse(price[1][0] == '-' ? price[1][1..] : price[1], out int stocks_offer) ? stocks_offer : 0;
										analysis.Bid = int.TryParse(price[^1][0] == '-' ? price[^1][1..] : price[^1], out int stocks_bid) ? stocks_bid : 0;
										break;

									case 8 when temp[1][1] == '0':
										price = temp[^1].Split(';');
										analysis.Current
											= double.TryParse(price[0][0] == '-' ? price[0][1..] : price[0], out double options_current) ? options_current : 0D;
										analysis.Offer
											= double.TryParse(price[1][0] == '-' ? price[1][1..] : price[1], out double options_offer) ? options_offer : 0D;
										analysis.Bid
											= double.TryParse(price[^1][0] == '-' ? price[^1][1..] : price[^1], out double options_bid) ? options_bid : 0D;
										break;

									case 6 when temp[0].Equals("주식우선호가"):
										price = temp[^1].Split(';');
										analysis.Offer = int.TryParse(price[0][0] == '-' ? price[0][1..] : price[0], out int offer) ? offer : 0;
										analysis.Bid = int.TryParse(price[^1][0] == '-' ? price[^1][1..] : price[^1], out int bid) ? bid : 0;
										break;
								}
							else if (temp[0].Length == 5 && temp[0].Equals("장시작시간"))
							{
								var operation = temp[^1].Split(';');

								if (int.TryParse(operation[0], out int number))
								{
									switch (Enum.ToObject(typeof(Catalog.OpenAPI.Operation), number))
									{
										case Catalog.OpenAPI.Operation.장시작:
											collection = operation[1].Equals(sat ? "100000" : "090000") && operation[^1].Equals("000000");
											stocks = true;
											futures = true;
											break;

										case Catalog.OpenAPI.Operation.장마감 when stocks:
											stocks = false;
											stocks_task = new Task(() =>
											{
												foreach (var collect in Progress.Collection)
													if (collect.Key is not null && collect.Key.Length == 6 && collect.Value.Collection.Count > 0)
														try
														{
															var convert = collect.Value.SortTheRecordedInformation;
															Repository.KeepOrganizedInStorage(JsonConvert.SerializeObject(convert.Item1),
																collect.Key, convert.Item2, convert.Item3, convert.Item4);
														}
														catch (Exception ex)
														{
															Base.SendMessage(collect.Value.GetType(), ex.StackTrace, collect.Key);
															Base.SendMessage(ex.StackTrace, collect.Key, collect.Value.GetType());
														}
											});
											stocks_task.Start();
											Server.WriteLine(string.Concat(typeof(Catalog.OpenAPI.Operation).Name, '|', operation[0]));
											break;

										case Catalog.OpenAPI.Operation.장시작전:
											if (operation[1].Equals(sat ? "095500" : "085500"))
												new Task(() =>
												{
													SetReservation();
													Base.SendMessage(operation[1], Progress.Collection.Count, typeof(Balance));

												}).Start();
											else if (operation[1].Equals(sat && Base.IsDebug is false ? "095000" : "085000"))
											{
												Server.WriteLine(string.Concat(typeof(Crypto.Security).Name, '|', Progress.Account));
												Server.WriteLine(string.Concat(typeof(Catalog.OpenAPI.Operation).Name, '|', operation[1]));
												GC.Collect();
											}
											else if (operation[1].Equals(sat ? "094500" : "084500"))
												new Task(async () =>
												{
													foreach (var length in new int[] { 6, 8 })
														foreach (var ch in await Progress.Client.GetContextAsync(new Codes { }, length) as List<Codes>)
															if (Progress.Collection.TryGetValue(ch.Code, out Analysis select) && double.TryParse(ch.Price, out double price))
															{
																if (length == 6)
																{
																	select.Market = 1 == (int)ch.MarginRate;
																	select.Current = (int)price;
																}
																else if (length == 8)
																{
																	select.MarginRate = ch.MarginRate;
																	select.Current = ch.Code[1] is '0' ? price : (int)price;
																}
															}
													Base.SendMessage(operation[1], Progress.Collection.Count, typeof(Analysis));

												}).Start();
											break;

										case Catalog.OpenAPI.Operation.장마감전_동시호가 when operation[1].Equals(sat && Base.CheckIfMarketDelay(now, 1) ? "162000" : "152000"):
											new Task(() =>
											{
												foreach (var stop in Progress.Collection)
													if (stop.Key.Length == 6 && stop.Value.Collector)
														stop.Value.Collector = false;

												SetReservation();
												Base.SendMessage(operation[1], Progress.Collection.Count, typeof(Balance));

											}).Start();
											break;
									}
									Base.SendMessage(string.Concat(DateTime.Now.ToString("HH:mm:ss.ffff"), '_',
										Enum.GetName(typeof(Catalog.OpenAPI.Operation), number), '_', operation[1]), typeof(Catalog.OpenAPI.Operation));
								}
								else if (char.TryParse(operation[0], out char charactor))
								{
									switch (Enum.ToObject(typeof(Catalog.OpenAPI.Operation), charactor))
									{
										case Catalog.OpenAPI.Operation.선옵_장마감전_동시호가_종료 when futures && collection:
											if (repeat && futures && collection)
											{
												repeat = false;
												futures_task = new Task(() =>
												{
													foreach (var collect in Progress.Collection)
														if (collect.Key.Length == 8 && collect.Value.Collection.Count > 0)
															try
															{
																var convert = collect.Value.SortTheRecordedInformation;
																Repository.KeepOrganizedInStorage(JsonConvert.SerializeObject(convert.Item1),
																	collect.Key, convert.Item2, convert.Item3, convert.Item4);
															}
															catch (Exception ex)
															{
																Base.SendMessage(collect.Value.GetType(), ex.StackTrace, collect.Key);
																Base.SendMessage(ex.StackTrace, collect.Key, collect.Value.GetType());
															}
												});
												futures_task.Start();
											}
											else
											{
												futures = false;
												collection = false;
											}
											break;

										case Catalog.OpenAPI.Operation.선옵_장마감전_동시호가_시작:
											new Task(() =>
											{
												foreach (var stop in Progress.Collection)
													if (stop.Key.Length == 8 && stop.Value.Collector)
														stop.Value.Collector = false;

											}).Start();
											break;

										case Catalog.OpenAPI.Operation.시간외_단일가_매매종료:
											if (stocks_task != null && stocks_task.IsCompleted == false)
											{
												Base.SendMessage("Waiting to receive Stocks. . .", typeof(Repository));
												stocks_task.Wait();
											}
											if (futures_task != null && futures_task.IsCompleted == false)
											{
												Base.SendMessage("Waiting to receive Futures. . .", typeof(Repository));
												futures_task.Wait();
											}
											if (server.IsConnected)
											{
												Server.WriteLine(string.Concat(typeof(Catalog.OpenAPI.Operation).Name, '|', operation[0]));
												server.Disconnect();
											}
											Process.Start("shutdown.exe", "-r");
											Process.GetCurrentProcess().Kill();
											break;
									}
									Base.SendMessage(string.Concat(DateTime.Now.ToString("HH:mm:ss.ffff"), '_',
										Enum.GetName(typeof(Catalog.OpenAPI.Operation), charactor), '_', operation[1]), typeof(Catalog.OpenAPI.Operation));
								}
							}
							else if (temp[0].Length == 0xD)
							{
								var balance = temp[^1].Split(';');
								var market = balance[0].Length == 8 && balance[0][1] == '0';

								if (balance.Length > 2 && Progress.Collection.TryGetValue(balance[0], out Analysis bal) && double.TryParse(balance[4], out double current))
								{
									if (int.TryParse(balance[2], out int quantity) && long.TryParse(balance[5], out long revenue)
										&& double.TryParse(balance[3], out double purchase) && double.TryParse(balance[6], out double rate))
									{
										Client.Local.Instance.PostContext(new Catalog.Models.Balance
										{
											Code = balance[0],
											Name = balance[1],
											Quantity = quantity.ToString("N0"),
											Purchase = purchase.ToString(market ? "N2" : "N0"),
											Current = current.ToString(market ? "N2" : "N0"),
											Revenue = revenue.ToString("C0"),
											Rate = rate.ToString("P2"),
											Trend = string.Empty,
											Separation = string.Empty
										});
										bal.Balance = new Balance
										{
											Market = market,
											Name = balance[1],
											Quantity = quantity,
											Purchase = market ? purchase : (int)purchase,
											Revenue = revenue,
											Rate = rate,
										};
									}
									bal.SellPrice = market ? 0D : 0;
									bal.BuyPrice = market ? 0D : 0;
									bal.Current = market ? current : (int)current;
								}
								else
								{
									var look = DateTime.Now;

									if (look.Hour > 8 || look.Hour < 5)
									{
										collection = true;
										stocks = true;
										futures = true;
									}
									if (long.TryParse(balance[1], out long cash))
										Strategics.Cash = cash;
								}
								Base.SendMessage(string.Concat(DateTime.Now.ToString("HH:mm:ss.ffff"), '_', temp[^1]), typeof(Balance));
							}
							else if (temp.Length == 1 && param.Length == 0xA)
								Base.SendMessage(param, typeof(Balance));
						}
					}
				}
				catch (Exception ex)
				{
					Base.SendMessage(typeof(Pipe), ex.StackTrace);
					Base.SendMessage(ex.StackTrace, typeof(Pipe));
				}
				finally
				{
					client.Close();
					client.Dispose();

					if (server.IsConnected)
					{
						Server.Close();
						Server.Dispose();
						server.Close();
						server.Dispose();
					}
					TellTheClientConnectionStatus(server.GetType().Name, server.IsConnected);
					TellTheClientConnectionStatus(client.GetType().Name, client.IsConnected);
				}
			if (repeat)
			{
				Thread.Sleep(0xC67);
				Progress.TryToConnectThePipeStream(null);
				Console.WriteLine("Wait for the {0}API to Restart. . .", Progress.Company is 'O' ? "Open" : "Xing");
			}
			else
			{
				Process.Start("shutdown.exe", "-r");
				Process.GetCurrentProcess().Kill();
			}
		}
		internal static void TellTheClientConnectionStatus(string name, bool is_connected) => Console.WriteLine("{0} is connected on {1}", name, is_connected);
		static void SetReservation()
		{
			foreach (var kv in new Reservation(Progress.Collection
				.Where(o => o.Key is not null && o.Value.Balance is Balance bal && bal.Quantity > 0 && o.Value.Strategics is Catalog.SatisfyConditionsAccordingToTrends).ToArray()).Stocks)
			{
				var order = string.Concat("Order|", kv.Value);
				Server.WriteLine(order);
				Base.SendMessage(order, (int)(long.MaxValue - kv.Key > int.MaxValue ? int.MaxValue : long.MaxValue - kv.Key), kv.Key.GetType());
			}
		}
	}
}