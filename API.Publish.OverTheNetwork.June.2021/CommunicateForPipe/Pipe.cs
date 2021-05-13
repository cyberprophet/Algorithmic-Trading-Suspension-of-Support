using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using ShareInvest.Catalog.Models;
using ShareInvest.Crypto;
using ShareInvest.EventHandler;

namespace ShareInvest
{
	[SupportedOSPlatform("windows")]
	public class Pipe
	{
		public event EventHandler<SendSecuritiesAPI> Send;
		public Pipe(string name, string type, bool lite)
		{
			if (type.Equals(typeof(Security).Name))
				price = new Dictionary<string, string>();

			this.name = name;
			this.lite = lite;
			Collection = new Dictionary<string, Queue<Collect>>(0x800);
			Initialize();
		}
		public string Message
		{
			get; private set;
		}
		public void StartProgress() => new Task(async () =>
		{
			Message = Base.TellTheClientConnectionStatus(Client.GetType().Name, Client.IsConnected);
			await Client.ConnectAsync();

			if (Client.IsConnected)
				OnReceivePipeClientMessage();

		}).Start();
		void Initialize() => Client = new NamedPipeClientStream(".", name, PipeDirection.In, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
		void OnReceivePipeClientMessage()
		{
			DateTime today = DateTime.Now, now = today.Hour > 0xF ? today.AddDays(Base.IsDebug ? 0 : 1) : today;
			bool collection = false, stocks = false, futures = false, sq = true, fq = true, sat = Base.CheckIfMarketDelay(now);
			using (var sr = new StreamReader(Client))
				try
				{
					while (Client.IsConnected)
					{
						var param = sr.ReadLine();

						if (string.IsNullOrEmpty(param) is false)
						{
							string[] temp = param.Split('|');

							switch (temp[0].Length)
							{
								case 4 when temp[0].Equals("주식시세") is false && (stocks && temp[1].Length == 6 || temp[1].Length == 8 && futures):
								case 6 when temp[0].Equals("주식우선호가") is false && (sq && temp[1].Length == 6 || temp[1].Length == 8 && fq):
									if (collection && Collection.TryGetValue(temp[1], out Queue<Collect> collector))
									{
										if (temp[0].Length == 6 && collector.Count == 0)
											continue;

										var price = temp[^1].Split(';');

										if (this.price is not null && temp[0].Length == 4 && temp[1].Length == 6 && price[1].Equals(this.price[temp[1]]) is false)
										{
											this.price[temp[1]] = price[1];
											Send?.Invoke(this, new SendSecuritiesAPI(new Message { Key = temp[1], Convey = price[1] }));
										}
										collector.Enqueue(new Collect
										{
											Time = price[0],
											Datum = temp[^1][7..]
										});
									}
									break;

								case 5:
									if (temp[0].Equals("Codes") && (temp[1].Length == 6 || temp[1].Length == 8) && lite)
									{
										Collection[temp[1]] = new Queue<Collect>(0x800);

										if (price is not null && temp[1].Length == 6)
										{
											price[temp[1]] = temp[^1];
											Send?.Invoke(this, new SendSecuritiesAPI(new Message { Key = temp[1], Convey = temp[^1] }));
										}
										if (Collection.Count % 0x400 == 0 || Collection.Count > 0x400 * 3)
											Send?.Invoke(this, new SendSecuritiesAPI($"Total of {Collection.Count:N0} Stocks to be Collect."));
									}
									else if (temp[0].Equals("장시작시간"))
									{
										var operation = temp[^1].Split(';');
										var op = Enum.ToObject(typeof(Catalog.OpenAPI.Operation), int.TryParse(operation[0], out int number) ? number : char.TryParse(operation[0], out char initial) ? initial : null);

										switch (op)
										{
											case Catalog.OpenAPI.Operation.장시작 when lite:
												collection = operation[1].Equals(sat ? "100000" : "090000") && operation[^1].Equals("000000");
												stocks = true;
												futures = true;
												break;

											case Catalog.OpenAPI.Operation.장마감 when stocks:
												stocks = false;
												new Task(async () => await RecordedInformation(6, DateTime.Now)).Start();
												break;

											case Catalog.OpenAPI.Operation.장시작전:
												if (operation[1].Equals(sat ? "095000" : "085000"))
													GC.Collect();

												break;

											case Catalog.OpenAPI.Operation.장마감전_동시호가 when operation[1].Equals(sat ? "162000" : "152000"):
												sq = false;
												break;

											case Catalog.OpenAPI.Operation.선옵_장마감전_동시호가_종료 when futures && collection:
												futures = false;
												collection = false;
												break;

											case Catalog.OpenAPI.Operation.선옵_장마감전_동시호가_시작:
												fq = false;
												break;

											case Catalog.OpenAPI.Operation.시간외_단일가_매매종료:
												GC.Collect();
												break;

											case Catalog.OpenAPI.Operation.장종료_시간외종료:
												if (price is not null)
													price.Clear();

												if (lite)
													Collection.Clear();

												GC.Collect();
												break;
										}
										Send?.Invoke(this, new SendSecuritiesAPI(Enum.GetName(typeof(Catalog.OpenAPI.Operation), op)));
									}
									break;

								case 1:
									if (char.TryParse(temp[0], out char type))
										Send?.Invoke(this, new SendSecuritiesAPI(type, temp[1]));

									break;

								case 0xB when temp[^1].Split(';') is string[] response:
									Send?.Invoke(this, new SendSecuritiesAPI(new Catalog.Strategics.Options
									{
										Code = response[0],
										StrikePrice = response[1],
										Current = response[2],
										ImpliedVolatility = response[3],
										IntrinsicValue = response[4],
										TimeValue = response[5],
										Delta = response[6],
										Gamma = response[7],
										Theta = response[8],
										Vega = response[9],
										Rho = response[^1]
									}));
									break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Base.SendMessage(typeof(Pipe), ex.StackTrace);
					Send?.Invoke(this, new SendSecuritiesAPI(ex.TargetSite.Name));
				}
				finally
				{
					Client.Close();
					Client.Dispose();
					Send?.Invoke(this, new SendSecuritiesAPI(Base.TellTheClientConnectionStatus(Client.GetType().Name, Client.IsConnected)));
				}
			if (price is not null && string.IsNullOrEmpty(name) is false)
			{
				Thread.Sleep(0xC67);
				Initialize();
				Thread.Sleep(0xC97);
				StartProgress();
			}
			now = DateTime.Now;

			if (now.Hour is 0x10 or 0x11 && now.DayOfWeek is not DayOfWeek.Saturday or DayOfWeek.Sunday && Base.CheckIfMarketDelay(now) is false)
				foreach (var collect in Collection)
					if (string.IsNullOrEmpty(collect.Key) is false && collect.Value.Count > 0)
						try
						{
							var convert = new Sort(collect.Key).TheRecordedInformation(collect.Value);
							Repository.KeepOrganizedInStorage(JsonConvert.SerializeObject(convert.Item1), collect.Key, convert.Item2, convert.Item3, convert.Item4);
						}
						catch (Exception ex)
						{
							Send?.Invoke(this, new SendSecuritiesAPI($"{collect.Key}_{ex.Message}"));
						}
						finally
						{
							collect.Value.Clear();
						}
			Send?.Invoke(this, new SendSecuritiesAPI((short)-0x6A));
		}
		async Task RecordedInformation(int classfication, DateTime now)
		{
			do
			{
				foreach (var collect in Collection)
					if (string.IsNullOrEmpty(collect.Key) is false && collect.Key.Length == classfication && collect.Value.Count > 0)
						try
						{
							var convert = new Sort(collect.Key).TheRecordedInformation(collect.Value);
							Repository.KeepOrganizedInStorage(JsonConvert.SerializeObject(convert.Item1), collect.Key, convert.Item2, convert.Item3, convert.Item4);
						}
						catch (Exception ex)
						{
							Send?.Invoke(this, new SendSecuritiesAPI($"{collect.Key}_{ex.Message}"));
						}
						finally
						{
							collect.Value.Clear();
						}
				while (now.Hour == (Base.CheckIfMarketDelay(now) ? 0x10 : 0xF) && now.Minute < 0x2E)
				{
					await Task.Delay(0xEA71);
					now = DateTime.Now;
				}
				classfication += 2;
			}
			while (classfication == 8);
		}
		NamedPipeClientStream Client
		{
			get; set;
		}
		Dictionary<string, Queue<Collect>> Collection
		{
			get;
		}
		readonly bool lite;
		readonly string name;
		readonly Dictionary<string, string> price;
	}
}