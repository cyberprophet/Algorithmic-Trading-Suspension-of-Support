using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using ShareInvest.Catalog.Models;

namespace ShareInvest
{
	static class Pipe
	{
		internal static void OnReceivePipeClientMessage(dynamic security, NamedPipeClientStream client)
		{
			var api = security as Security;
			DateTime today = DateTime.Now, now = today.Hour > 0xF ? today.AddDays(Base.IsDebug ? 0 : 1) : today;
			bool repeat = true, collection = false, stocks = false, futures = false, sq = true, fq = true, sat = Base.CheckIfMarketDelay(now);
			using (var sr = new StreamReader(client))
				try
				{
					while (client.IsConnected)
					{
						var param = sr.ReadLine();

						if (string.IsNullOrEmpty(param) is false)
						{
							string[] temp = param.Split('|');

							switch (temp[0].Length)
							{
								case 4 when temp[0].Equals("주식시세") is false && (stocks && temp[1].Length == 6 || temp[1].Length == 8 && futures):
								case 6 when temp[0].Equals("주식우선호가") is false && (sq && temp[1].Length == 6 || temp[1].Length == 8 && fq):
									if (collection && Progress.Collection.TryGetValue(temp[1], out Queue<Collect> collector))
									{
										if (temp[0].Length == 6 && collector.Count == 0)
											continue;

										collector.Enqueue(new Collect
										{
											Time = temp[^1].Split(';')[0],
											Datum = temp[^1][7..]
										});
									}
									break;

								case 5:
									if (temp[0].Equals("Codes") && (temp[^1].Length == 6 || temp[^1].Length == 8))
									{
										Progress.Collection[temp[^1]] = new Queue<Collect>(0x800);

										if (Progress.Collection.Count % 0x400 == 0 || Progress.Collection.Count > 0x400 * 3)
											api.SendMessage(string.Format("Total of {0} Stocks to be Collect.", Progress.Collection.Count.ToString("N0")));
									}
									else if (temp[0].Equals("장시작시간"))
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
													new Task(() =>
													{
														foreach (var collect in Progress.Collection)
															if (string.IsNullOrEmpty(collect.Key) is false && collect.Key.Length == 6 && collect.Value.Count > 0)
																try
																{
																	var convert = new Sort(collect.Key).TheRecordedInformation(collect.Value);
																	Repository.KeepOrganizedInStorage(JsonConvert.SerializeObject(convert.Item1), collect.Key, convert.Item2, convert.Item3, convert.Item4);
																}
																catch (Exception ex)
																{
																	Base.SendMessage(collect.Value.GetType(), ex.StackTrace, collect.Key);
																	api.SendMessage(ex.TargetSite.Name);
																}
													}).Start();
													break;

												case Catalog.OpenAPI.Operation.장시작전:
													if (operation[1].Equals(sat ? "095000" : "085000"))
														GC.Collect();

													break;

												case Catalog.OpenAPI.Operation.장마감전_동시호가 when operation[1].Equals(sat ? "162000" : "152000"):
													sq = false;
													break;
											}
											api.SendMessage(Enum.GetName(typeof(Catalog.OpenAPI.Operation), number));
										}
										else if (char.TryParse(operation[0], out char charactor))
										{
											switch (Enum.ToObject(typeof(Catalog.OpenAPI.Operation), charactor))
											{
												case Catalog.OpenAPI.Operation.선옵_장마감전_동시호가_종료 when futures && collection:
													if (repeat && futures && collection)
													{
														repeat = false;
														new Task(() =>
														{
															foreach (var collect in Progress.Collection)
																if (string.IsNullOrEmpty(collect.Key) is false && collect.Key.Length == 8 && collect.Value.Count > 0)
																	try
																	{
																		var convert = new Sort(collect.Key).TheRecordedInformation(collect.Value);
																		Repository.KeepOrganizedInStorage(JsonConvert.SerializeObject(convert.Item1), collect.Key, convert.Item2, convert.Item3, convert.Item4);
																	}
																	catch (Exception ex)
																	{
																		Base.SendMessage(collect.Value.GetType(), ex.StackTrace, collect.Key);
																		api.SendMessage(ex.TargetSite.Name);
																	}
														}).Start();
													}
													else
													{
														futures = false;
														collection = false;
													}
													break;

												case Catalog.OpenAPI.Operation.선옵_장마감전_동시호가_시작:
													fq = false;
													break;

												case Catalog.OpenAPI.Operation.시간외_단일가_매매종료:
													api.ShutDown();
													break;
											}
											api.SendMessage(Enum.GetName(typeof(Catalog.OpenAPI.Operation), charactor));
										}
									}
									break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Base.SendMessage(typeof(Pipe), ex.StackTrace);
					api.SendMessage(ex.TargetSite.Name);
				}
				finally
				{
					client.Close();
					client.Dispose();
					api.SendMessage(TellTheClientConnectionStatus(client.GetType().Name, client.IsConnected));
				}
			if (repeat)
			{
				Thread.Sleep(0xC67);

				if (Base.IsDebug)
					api.SendMessage(Enum.GetName(typeof(Catalog.OpenAPI.Operation), (char)Catalog.OpenAPI.Operation.시간외_단일가_매매종료));

				else
				{
					Progress.TryToConnectThePipeStream(api);
					api.SendMessage("Wait for the CoreAPI to Restart. . .");
				}
			}
			else
			{
				api.ShutDown();
				api.SendMessage(Enum.GetName(typeof(Catalog.OpenAPI.Operation), (char)Catalog.OpenAPI.Operation.시간외_단일가_매매종료));
			}
		}
		internal static string TellTheClientConnectionStatus(string name, bool is_connected) => string.Format("{0} is connected on {1}", name, is_connected);
	}
}