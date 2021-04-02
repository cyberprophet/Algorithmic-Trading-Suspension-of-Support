using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;

using ShareInvest.Catalog;
using ShareInvest.Catalog.Models;
using ShareInvest.Client;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest
{
	sealed partial class SecuritiesAPI : Form
	{
		internal SecuritiesAPI(bool version, dynamic key, ISecuritiesAPI<SendSecuritiesAPI> connect)
		{
			icons = new[] { Properties.Resources.sleep_adium_bird_20649, Properties.Resources.idle_adium_bird_20651, Properties.Resources.awake_adium_bird_20653, Properties.Resources.alert_adium_bird_20655, Properties.Resources.away_adium_bird_20654, Properties.Resources.invisible_adium_bird_20650 };
			InitializeComponent();
			this.connect = connect;
			this.version = version;
			this.key = key;
			api = API.GetInstance(key);
			random = new Random(Guid.NewGuid().GetHashCode());
			Codes = new Queue<string>();
			GetTheCorrectAnswer = new int[this.key.Length];
			server = GoblinBat.GetInstance(key);
			connect.IsServer = api.IsServer && api.IsAdministrator;
			timer.Start();
		}
		async Task OnReceiveFinancialStatement(DateTime now, Retention retention)
		{
			Queue<ConvertConsensus> queue;
			Queue<FinancialStatement> context = null;
			var consensus = new Client.Consensus(key);

			for (int i = 0; i < retention.Code.Length / 3; i++)
			{
				queue = await consensus.GetContextAsync(i, retention.Code);
				int status = int.MinValue;

				if (queue != null && queue.Count > 0)
				{
					status = await api.PostContextAsync(queue);

					if (i == 0)
						context = new Summary(key).GetContext(retention.Code, now.Day);

					if (i == 1 && context != null)
						status = await api.PostContextAsync(context);
				}
				Base.SendMessage(retention.Code, status, GetType());
			}
		}
		void OnReceiveInformationTheDay() => BeginInvoke(new Action(async () =>
		{
			if (Codes.TryDequeue(out string str))
			{
				var now = DateTime.Now;

				if (string.IsNullOrEmpty(str) is false && str.Length is 6 or 8 && await api.GetContextAsync(str) is Retention retention && string.IsNullOrEmpty(retention.Code) is false)
				{
					if (string.IsNullOrEmpty(retention.LastDate) is false && now.ToString(Base.DateFormat).Equals(retention.LastDate.Substring(0, 6)) || string.IsNullOrEmpty(retention.Code) is false && retention.LastDate is null)
						OnReceiveInformationTheDay();

					else
						try
						{
							if (retention.Code.Length == 6)
							{
								if (api.IsInsider)
									await OnReceiveFinancialStatement(now, retention);

								else
									switch (Operation)
									{
										case Catalog.OpenAPI.Operation.장마감 or Catalog.OpenAPI.Operation.시간외_종가매매_시작 or Catalog.OpenAPI.Operation.선옵_장마감전_동시호가_시작 or Catalog.OpenAPI.Operation.장종료_예상지수종료 or Catalog.OpenAPI.Operation.장종료_시간외종료:
											if (await new KRX.Incorporate(Interface.KRX.Catalog.지수구성종목, key).GetConstituentStocks(now, Codes.Count) is Stack<Catalog.IncorporatedStocks> stack && await api.PostContextAsync(stack) == 0xC8)
												await Task.Delay(0x100);

											break;

										case Catalog.OpenAPI.Operation.선옵_장마감전_동시호가_종료 or Catalog.OpenAPI.Operation.시간외_종가매매_종료 when await api.GetPageAsync(retention.Code) is int page:
											await new Advertise(key).StartAdvertisingInTheDataCollectionSection(page);
											break;

										default:
											await OnReceiveFinancialStatement(now, retention);
											break;
									}
							}
						}
						catch (Exception ex)
						{
							Base.SendMessage(GetType(), ex.StackTrace);
						}
						finally
						{
							(connect as OpenAPI.ConnectAPI).InputValueRqData(string.Concat(instance, retention.Code.Length == 8 ? (retention.Code[0] > '1' ? "Opt50066" : "Opt50028") : "Opt10079"), string.Concat(retention.Code, ';', retention.LastDate)).Send += OnReceiveSecuritiesAPI;
						}
				}
				else
					OnReceiveInformationTheDay();
			}
			else
				Dispose(connect as Control);
		}));
		void CheckTheInformationReceivedOnTheDay()
		{
			if (Codes.TryDequeue(out string code))
			{
				if (string.IsNullOrEmpty(code) || Base.IsDebug && code.Length == 6)
					CheckTheInformationReceivedOnTheDay();

				else
					(connect as OpenAPI.ConnectAPI).InputValueRqData(string.Concat(instance, code.Length == 8 ? (code[0] > '1' ? "Opt50068" : "OPT50030") : "Opt10081"), string.Concat(code, ';', DateTime.Now.ToString(code.Length == 6 ? Base.LongDateFormat : Base.DateFormat))).Send += OnReceiveSecuritiesAPI;
			}
			else
				Dispose(connect as Control);
		}
		void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e) => BeginInvoke(new Action(async () =>
		{
			var now = DateTime.Now;

			switch (e.Convey)
			{
				case Catalog.OpenAPI.Order order:
					if ((connect as OpenAPI.ConnectAPI).TryGetValue(order.Code, out Analysis oc) && oc.OrderNumber is not null && oc.Classification is not null)
					{
						if (oc.Quantity > 0 && oc.Quantity - oc.OrderNumber.Count(o => o.Value > order.Price) < (oc.Strategics is Strategics.Scenario ? oc.Classification.Short : 1))
							connect.SendOrder(new Catalog.OpenAPI.Order
							{
								AccNo = order.AccNo,
								OrderType = (int)OrderType.매도취소,
								Code = order.Code,
								Qty = 1,
								Price = order.Price,
								HogaGb = ((int)HogaGb.지정가).ToString("D2"),
								OrgOrderNo = oc.OrderNumber.OrderByDescending(o => o.Value).First().Key
							});
						else if (Cash < order.Price * (oc.Strategics is Strategics.Scenario ? oc.Classification.Long : 1) && oc.OrderNumber.Any(o => o.Value < order.Price))
						{
							var kv = oc.OrderNumber.OrderBy(o => o.Value).First();
							connect.SendOrder(new Catalog.OpenAPI.Order
							{
								AccNo = order.AccNo,
								Code = order.Code,
								HogaGb = order.HogaGb,
								OrgOrderNo = kv.Key,
								OrderType = (int)OrderType.매수취소,
								Price = order.Price,
								Qty = 1
							});
							Cash += kv.Value;
						}
					}
					connect.SendOrder(order);
					return;

				case int cash:
					Cash += cash;
					return;

				case Balance balance:
					if (await server.PostContextAsync(balance) is int status)
						Base.SendMessage(sender.GetType(), balance.Name, status);

					return;

				case Tuple<Catalog.OpenAPI.Operation, string, string> operation:
					switch (operation.Item1)
					{
						case Catalog.OpenAPI.Operation.장시작전:
							switch (operation.Item2[2..])
							{
								case reservation:
									if (Reservation is not null)
										foreach (var order in Reservation.Stocks)
										{
											connect.SendOrder(order.Value);
											Base.SendMessage(sender.GetType(), order.Key.ToString("N0"), order.Value.Code);
										}
									return;

								case construction:
									if (Reservation is not null)
									{
										Cash = Reservation.Cash;
										Reservation.Clear();
									}
									RequestBalanceInquiry();
									return;
							}
							break;

						case Catalog.OpenAPI.Operation.장시작 when (Base.IsDebug || api.IsAdministrator is false) && connect is OpenAPI.ConnectAPI wait:
							foreach (var ctor in wait.Enumerator)
								ctor.Wait = true;

							break;

						case Catalog.OpenAPI.Operation.장마감전_동시호가:
							switch (operation.Item2[2..])
							{
								case before_market_closing:
									if (Reservation is not null)
										Reservation.Clear();

									RequestBalanceInquiry();
									return;

								case market_closing_reservation:
									if (Reservation is not null)
										foreach (var order in Reservation.Stocks)
										{
											connect.SendOrder(order.Value);
											Base.SendMessage(sender.GetType(), order.Key.ToString("N0"), order.Value.Code);
										}
									return;
							}
							break;

						case Catalog.OpenAPI.Operation.장마감:
							if (worker.WorkerSupportsCancellation)
								worker.CancelAsync();

							OnReceiveInformationTheDay();
							break;

						case Catalog.OpenAPI.Operation.시간외_단일가_매매종료:

							break;

						case Catalog.OpenAPI.Operation.장종료_시간외종료:
							Dispose(connect as Control);
							return;
					}
					Operation = operation.Item1;
					notifyIcon.Text = Enum.GetName(typeof(Catalog.OpenAPI.Operation), operation.Item1);
					return;

				case Tuple<string, string, string, string, int> tr when string.IsNullOrEmpty(tr.Item1) is false:
					var app = connect as OpenAPI.ConnectAPI;
					var param = new Codes
					{
						Code = tr.Item1,
						Name = tr.Item2,
						MaturityMarketCap = tr.Item3,
						Price = tr.Item4,
						MarginRate = tr.Item5
					};
					if (param.MaturityMarketCap.Contains(Base.TransactionSuspension) is false && double.TryParse(tr.Item4, out double price))
					{
						connect.Writer.WriteLine(string.Concat(param.GetType().Name, '|', param.Code, '|', param.Price));
						var classfication = param.Code.Length == 6 || param.Code.Length == 8 && param.Code[1].CompareTo('0') > 0;

						if (Base.IsDebug || api.IsAdministrator is false)
						{
							if (classfication)
							{
								if (param.Code.Length == 6)
									app.Append(tr.Item1, new SecondaryIndicators.OpenAPI.Stocks
									{
										Code = tr.Item1,
										Name = tr.Item2,
										Current = (int)price,
										MarketMarginRate = tr.Item5,
										Offer = 0,
										Bid = 0

									}).Send += OnReceiveSecuritiesAPI;
								else if (param.Code.Length == 8 && param.Code[1].CompareTo('0') > 0)
									app.Append(tr.Item1, new SecondaryIndicators.OpenAPI.StockFutures
									{
										Code = tr.Item1,
										Name = tr.Item2,
										Current = (int)price,
										MarketMarginRate = tr.Item5,
										Offer = 0,
										Bid = 0

									}).Send += OnReceiveSecuritiesAPI;
							}
							else
							{
								if (param.Code.Length == 8 && param.Code[0] is '1')
									app.Append(tr.Item1, new SecondaryIndicators.OpenAPI.Futures
									{
										Code = tr.Item1,
										Name = tr.Item2,
										Current = price,
										MarketMarginRate = tr.Item5,
										Offer = 0D,
										Bid = 0D

									}).Send += OnReceiveSecuritiesAPI;
								else if (param.Code.Length == 8 && param.Code[0] is '2' or '3')
									app.Append(tr.Item1, new SecondaryIndicators.OpenAPI.Options
									{
										Code = tr.Item1,
										Name = tr.Item2,
										Current = price,
										MarketMarginRate = tr.Item5,
										Offer = 0D,
										Bid = 0D

									}).Send += OnReceiveSecuritiesAPI;
							}
						}
						if (param.Code.Length == 6 || param.Code.Length == 8 && param.Code[1] is '0')
							Codes.Enqueue(param.Code);
					}
					if (param.Code.Length == 8)
					{
						if (Codes.TryPeek(out string code) && code.Length > 8 && Codes.TryDequeue(out string dequeue))
						{
							var temp = dequeue.Split('_');
							app.RemoveValueRqData(temp[0], temp[^1]).Send -= OnReceiveSecuritiesAPI;
						}
						app.RemoveValueRqData(sender.GetType().Name, param.Code).Send -= OnReceiveSecuritiesAPI;
					}
					if ((param.Code.Length == 6 || param.Code.Length == 8 && param.Code[0] > '1') && api.IsAdministrator && await api.PutContextAsync(param) is string response && param.Code.Equals(response))
						notifyIcon.Text = param.Name;

					return;

				case Tuple<string, string> request:
					if (request.Item2.Length != 8)
						Codes.Enqueue(string.Concat(request.Item1, '_', request.Item2));

					(sender as OpenAPI.ConnectAPI).InputValueRqData(string.Concat(instance, request.Item1), request.Item2).Send += OnReceiveSecuritiesAPI;
					return;

				case Codes codes:
					if (api.IsAdministrator && string.IsNullOrEmpty(await api.PutContextAsync(codes) as string))
						Base.SendMessage(sender.GetType(), codes.Name, codes.MaturityMarketCap);

					return;

				case string message:
					if (await server.PostContextAsync(new Catalog.Models.Message { Convey = message, Key = Security.Key }) is int)
						notifyIcon.Text = string.Concat(DateTime.Now.ToLongTimeString(), " ", message);

					return;

				case Queue<string[]> hold:
					var name = sender.GetType().Name;
					var bal = connect as OpenAPI.ConnectAPI;
					bal.RemoveValueRqData(name, string.Concat(connect.Account[name.EndsWith("Opw00005") ? 0 : ^1], password)).Send -= OnReceiveSecuritiesAPI;

					while (hold.TryDequeue(out string[] ing))
						if (ing[0].Length == 8 && int.TryParse(ing[4], out int quantity) && double.TryParse(ing[9], out double fRate) && long.TryParse(ing[8], out long fValuation) && double.TryParse(ing[6], out double fCurrent) && double.TryParse(ing[5], out double fPurchase) && await server.PostContextAsync(new Balance
						{
							Kiwoom = Security.Key,
							Account = connect.Account[^1].Substring(0, 8).Insert(4, "－"),
							Code = ing[0],
							Name = ing[1].Equals(ing[0]) && bal.TryGetValue(ing[1], out Analysis analysis) ? analysis.Name : ing[1],
							Quantity = (ing[2].Equals("1") ? -quantity : quantity).ToString("N0"),
							Purchase = fPurchase.ToString(ing[0][1] is '0' ? "N2" : "N0"),
							Current = fCurrent.ToString(ing[0][1] is '0' ? "N2" : "N0"),
							Revenue = fValuation.ToString("C0"),
							Rate = (fRate * 1e-2).ToString("P2")

						}) is 0xC8)
							Base.SendMessage(sender.GetType(), ing[0], quantity);

						else if (ing[3].Length > 0 && ing[3][0] is 'A' && double.TryParse(ing[0xC]?.Insert(6, "."), out double ratio) && long.TryParse(ing[0xB], out long valuation) && int.TryParse(ing[6], out int amount) && uint.TryParse(ing[8], out uint purchase) && uint.TryParse(ing[7], out uint current) && await server.PostContextAsync(new Balance
						{
							Kiwoom = Security.Key,
							Account = connect.Account[0].Substring(0, 8).Insert(4, "－"),
							Code = ing[3][1..].Trim(),
							Name = ing[4].Trim(),
							Quantity = amount.ToString("N0"),
							Purchase = purchase.ToString("N0"),
							Current = current.ToString("N0"),
							Revenue = valuation.ToString("C0"),
							Rate = ratio.ToString("P2")

						}) is 0xC8)
						{
							if ((Base.IsDebug || api.IsAdministrator is false) && bal.TryGetValue(ing[3][1..].Trim(), out Analysis held))
							{
								held.Quantity = amount;
								held.Purchase = purchase;
								held.Current = current;
								held.Rate = ratio;
								held.Revenue = valuation;
								held.Name = ing[4].Trim();

								if (Reservation is not null)
									Reservation.Push(held);
							}
							Base.SendMessage(sender.GetType(), ing[4].Trim(), amount);
						}
					return;

				case Tuple<long, long> balance:
					if (now.Hour is 8 or 0xF)
						Reservation = new Reservation(balance.Item2, connect.Account);

					if (worker.WorkerSupportsCancellation is false && worker.IsBusy is false)
					{
						if (api.IsAdministrator is false || Base.IsDebug)
							worker.RunWorkerAsync(connect.Account);

						if (api.IsAdministrator && api.IsServer || Base.IsDebug)
						{
							var hour = Base.CheckIfMarketDelay(now) ? 9 : 8;
							var con = connect as OpenAPI.ConnectAPI;

							for (int i = hour == now.Hour ? 1 : 0; i < (hour == now.Hour ? con.Conditions.Count : con.Conditions.Count - 1); i++)
							{
								name = con.Conditions[i];
								con.SendCondition(i, name, name.Length == 2 ? 1 : 0);
							}
						}
					}
					Cash = balance.Item2;
					return;

				case Tuple<string, Stack<string>> charts:
					var receive = connect as OpenAPI.ConnectAPI;
					receive.RemoveValueRqData(sender.GetType().Name, charts.Item1).Send -= OnReceiveSecuritiesAPI;

					if ((charts.Item1.Length == 8 ? (charts.Item1[0] > '1' ? await api.PostContextAsync(Catalog.Models.Convert.ToStoreInOptions(charts.Item1, charts.Item2)) : await api.PostContextAsync(Catalog.Models.Convert.ToStoreInFutures(charts.Item1, charts.Item2))) : await api.PostContextAsync(Catalog.Models.Convert.ToStoreInStocks(charts.Item1, charts.Item2))) > 0xC7)
					{
						var message = $"Collecting Datum on {(charts.Item1.Length == 6 && receive.TryGetValue(charts.Item1, out Analysis analysis) ? analysis.Name : charts.Item1)}.\nStill {Codes.Count:N0} Stocks to be Collect.";
						notifyIcon.Text = message.Length < 0x40 ? message : $"Still {Codes.Count:N0} Stocks to be Collect.";
					}
					OnReceiveInformationTheDay();
					return;

				case string[] accounts:
					if (await server.PostContextAsync(Crypto.Security.Encrypt(new Account
					{
						Length = accounts.Length,
						Number = accounts,
						Security = key,
						Identity = connect.Securities("USER_ID"),
						Name = connect.Securities("USER_NAME")

					}, accounts.Length > 0)) is 0xC8 && ulong.TryParse(now.ToString(Base.FullDateFormat), out ulong date))
					{
						this.connect.Account = new string[2];
						var connect = this.connect as OpenAPI.ConnectAPI;
						connect.Real.Send += OnReceiveSecuritiesAPI;
						((ISendSecuritiesAPI<SendSecuritiesAPI>)connect.API).Send += OnReceiveSecuritiesAPI;
						string encrypt = Crypto.Security.Encrypt(this.connect.Securities("USER_ID")), server = this.connect.Securities("GetServerGubun"), length = (accounts.Length % 0xA).ToString("D1");

						if (await api.GetSecurityAsync(key) is Privacies pri && char.TryParse(pri.SecuritiesAPI, out char initial) && string.IsNullOrEmpty(pri.CodeStrategics) is false)
						{
							if (char.IsLetter(initial) && await api.PutContextAsync(new Privacies
							{
								Security = key,
								SecuritiesAPI = length,
								SecurityAPI = pri.SecurityAPI,
								CodeStrategics = encrypt,
								Coin = pri.Coin,
								Commission = date,
								Account = string.IsNullOrWhiteSpace(server) ? (server.Length % 0xA).ToString("D1") : server[^1..]

							}) is 0xC8)
								Base.SendMessage(sender.GetType(), key, accounts.Length);

							if (string.IsNullOrEmpty(pri.SecurityAPI) is false)
							{
								var acc = Crypto.Security.Decipher(pri.Security, pri.SecuritiesAPI, pri.SecurityAPI).Split(';');
								this.connect.Account[^1] = acc.Length > 1 && string.IsNullOrEmpty(acc[^1]) is false ? acc[^1] : string.Empty;
								this.connect.Account[0] = acc[0];
							}
							Privacy = pri;
						}
						else
						{
							foreach (var str in accounts)
								if (str.Length == 0xA && str[^2..].CompareTo("32") < 0)
								{
									if (str[^2..].CompareTo("31") == 0)
										this.connect.Account[^1] = str;

									else
										this.connect.Account[0] = str;
								}
							if (await api.PostContextAsync(new Privacies
							{
								Security = key,
								SecuritiesAPI = length,
								SecurityAPI = Crypto.Security.Encrypt(new Privacies { Security = key, SecuritiesAPI = key[^1].ToString() }, string.Concat(this.connect.Account[0], ';', Array.Exists(this.connect.Account, o => string.IsNullOrEmpty(o)) ? string.Empty : this.connect.Account[^1]), this.connect.Account.Length > 0),
								CodeStrategics = encrypt,
								Commission = date,
								Account = string.IsNullOrWhiteSpace(server) ? (server.Length % 0xA).ToString("D1") : server[^1..]

							}) is 0xC8)
								Base.SendMessage(sender.GetType(), key, accounts.Length);
						}
						foreach (var ctor in connect?.Chejan)
						{
							ctor.Identity = encrypt;
							ctor.Send += OnReceiveSecuritiesAPI;
						}
						Security.SetKey(encrypt);
						RequestBalanceInquiry();
					}
					return;

				case short error:
					var send = connect as OpenAPI.ConnectAPI;
					var hermes = send.SendErrorMessage(error);

					if (string.IsNullOrEmpty(hermes) is false && await server.PostContextAsync(new Catalog.Models.Message { Convey = $"[{Math.Abs(error):D6}] {hermes}({send.Count:D4})", Key = Security.Key }) is int)
						notifyIcon.Text = hermes;

					switch (error)
					{
						case -0x6A or -0xC8 or -0x64:
						case -0xCA when MessageBox.Show(ResetTheAccount(connect.Securities("ACCLIST").Split(';')), connect.Securities("USER_NAME"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) is DialogResult.Yes && await api.DeleteContextAsync(Privacy) is 0xC8:
							Dispose(connect as Control);
							return;
					}
					return;

				case Tuple<string, Stack<Catalog.Models.RevisedStockPrice>, Queue<Stocks>> models when connect is OpenAPI.ConnectAPI ca:
					ca.RemoveValueRqData(sender.GetType().Name, models.Item1).Send -= OnReceiveSecuritiesAPI;
					var restore = DateTime.TryParseExact(Base.End, Base.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime store) ? store : DateTime.UnixEpoch;
					IEnumerable<Tick> storage = await api.GetContextAsync(new Tick(), models.Item1) as IEnumerable<Tick>, repository = Repository.RetrieveSavedMaterial(models.Item1) as IEnumerable<Tick>;

					while (models.Item2.TryPop(out Catalog.Models.RevisedStockPrice revise))
						if (await api.PostContextAsync(revise) is int rsp && rsp != 0xC8)
							Base.SendMessage(sender.GetType(), models.Item1, models.Item2.Count);

					while (models.Item3.TryDequeue(out Stocks stock) && stock.Volume == 0)
						if (await api.PostContextAsync(stock) is string confirm)
						{
							var message = $"Deleted {confirm} {models.Item1} Data {stock.Date} where the Error was found.";
							notifyIcon.Text = message.Length < 0x40 ? message : $"Still {Codes.Count:N0} Stocks to be Collect.";
						}
						else
							notifyIcon.Text = $"Still {Codes.Count:N0} Stocks to be Collect.";

					while (restore.CompareTo(now) < 0)
					{
						if (restore.DayOfWeek is not DayOfWeek.Saturday or DayOfWeek.Sunday && Array.Exists(Base.Holidays, o => o.Equals(restore.ToString(Base.DateFormat))) is false)
						{
							var str = restore.ToString(Base.LongDateFormat);

							if (repository.Any(o => o.Date.Equals(str)) && repository.First(o => o.Date.Equals(str)) is Tick my)
							{
								if (storage is IEnumerable<Tick> && storage.Any(o => o.Date.Equals(str)) && storage.First(o => o.Date.Equals(str)) is Tick server && (my.Open.Equals(server.Open) && my.Close.Equals(server.Close) && my.Price.Equals(server.Price)) is false && int.TryParse(my.Open, out int mo) && int.TryParse(server.Open, out int so) && int.TryParse(my.Close, out int mc) && int.TryParse(server.Close, out int sc))
								{
									if ((mo < so || mc > sc) && Repository.RetrieveSavedMaterial(my) is string contents && await api.PostContextAsync(new Tick { Code = my.Code, Date = my.Date, Open = my.Open, Close = my.Close, Price = my.Price, Path = Path.Combine($"F:\\{key}", my.Code, my.Date.Substring(0, 4), my.Date.Substring(4, 2), $"{my.Date[6..]}.res"), Contents = contents }) is 0xC8)
										notifyIcon.Text = $"Modify the {my.Code} stored on the Server.";

									else if ((mo > so || mc < sc || my.Price.Equals(Base.PriceEmpty)) && await api.GetContextAsync(new Tick { Code = models.Item1, Date = server.Date, Open = server.Open, Close = server.Close, Price = server.Price, Path = string.Empty, Contents = server.Contents }) is Tick tick && Repository.Delete(my))
										Repository.KeepOrganizedInStorage(tick);
								}
								else if ((storage is null || storage.Any(o => o.Date.Equals(str)) is false) && Repository.RetrieveSavedMaterial(my) is string file && await api.PostContextAsync(new Tick { Code = my.Code, Date = my.Date, Open = my.Open, Close = my.Close, Price = my.Price, Contents = file, Path = Path.Combine($"F:\\{key}", my.Code, my.Date.Substring(0, 4), my.Date.Substring(4, 2), $"{my.Date[6..]}.res") }) is 0xC8)
									notifyIcon.Text = $"Modify the {my.Code} stored on the Server.";
							}
							else if (storage is IEnumerable<Tick> && storage.Any(o => o.Date.Equals(str)) && storage.First(o => o.Date.Equals(str)) is Tick server && await api.GetContextAsync(new Tick { Code = models.Item1, Date = server.Date, Open = server.Open, Close = server.Close, Price = server.Price, Path = string.Empty, Contents = server.Contents }) is Tick tick)
								Repository.KeepOrganizedInStorage(tick);
						}
						restore = restore.AddDays(1);
					}
					if (ca.CorrectTheDelayMilliseconds() > 0)
						CheckTheInformationReceivedOnTheDay();

					return;

				case Tuple<string, Queue<Stocks>> confirm:
					(connect as OpenAPI.ConnectAPI).RemoveValueRqData(sender.GetType().Name, confirm.Item1).Send -= OnReceiveSecuritiesAPI;
					var fo = DateTime.TryParseExact(Base.IsServer(api.IsServer), Base.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dt) ? dt : DateTime.UnixEpoch;
					IEnumerable<Tick> ss = await api.GetContextAsync(new Tick(), confirm.Item1) as IEnumerable<Tick>, rs = Repository.RetrieveSavedMaterial(confirm.Item1) as IEnumerable<Tick>;

					while (confirm.Item2.TryDequeue(out Stocks stock))
						if (await api.PostContextAsync(stock) is string model)
						{
							var message = $"Deleted {model} {confirm.Item1} Data {stock.Date} where the Error was found.";
							notifyIcon.Text = message.Length < 0x40 ? message : $"Still {Codes.Count:N0} Futures to be Collect.";
						}
						else
							notifyIcon.Text = $"Still {Codes.Count:N0} Futures to be Collect.";

					while (fo.CompareTo(now) < 0)
					{
						if (fo.DayOfWeek is not DayOfWeek.Saturday or DayOfWeek.Sunday && Array.Exists(Base.Holidays, o => o.Equals(fo.ToString(Base.DateFormat))) is false)
						{
							var str = fo.ToString(Base.LongDateFormat);

							if (rs.Any(o => o.Date.Equals(str)) && rs.First(o => o.Date.Equals(str)) is Tick my)
							{
								if (ss is IEnumerable<Tick> && ss.Any(o => o.Date.Equals(str)) && ss.First(o => o.Date.Equals(str)) is Tick server && (my.Open.Equals(server.Open) && my.Close.Equals(server.Close) && my.Price.Equals(server.Price)) is false && int.TryParse(my.Open, out int mo) && int.TryParse(server.Open, out int so) && int.TryParse(my.Close, out int mc) && int.TryParse(server.Close, out int sc))
								{
									if ((mo < so || mc > sc) && Repository.RetrieveSavedMaterial(my) is string contents && await api.PostContextAsync(new Tick { Code = my.Code, Date = my.Date, Open = my.Open, Close = my.Close, Price = my.Price, Path = Path.Combine($"F:\\{key}", my.Code, my.Date.Substring(0, 4), my.Date.Substring(4, 2), $"{my.Date[6..]}.res"), Contents = contents }) is 0xC8)
										notifyIcon.Text = $"Modify the {my.Code} stored on the Server.";

									else if ((mo > so || mc < sc || my.Price.Equals(Base.PriceEmpty)) && await api.GetContextAsync(new Tick { Code = confirm.Item1, Date = server.Date, Open = server.Open, Close = server.Close, Price = server.Price, Path = string.Empty, Contents = server.Contents }) is Tick tick && Repository.Delete(my))
										Repository.KeepOrganizedInStorage(tick);
								}
								else if ((ss is null || ss.Any(o => o.Date.Equals(str)) is false) && Repository.RetrieveSavedMaterial(my) is string file && await api.PostContextAsync(new Tick { Code = my.Code, Date = my.Date, Open = my.Open, Close = my.Close, Price = my.Price, Contents = file, Path = Path.Combine($"F:\\{key}", my.Code, my.Date.Substring(0, 4), my.Date.Substring(4, 2), $"{my.Date[6..]}.res") }) is 0xC8)
									notifyIcon.Text = $"Modify the {my.Code} stored on the Server.";
							}
							else if (ss is IEnumerable<Tick> && ss.Any(o => o.Date.Equals(str)) && ss.First(o => o.Date.Equals(str)) is Tick server && await api.GetContextAsync(new Tick { Code = confirm.Item1, Date = server.Date, Open = server.Open, Close = server.Close, Price = server.Price, Path = string.Empty, Contents = server.Contents }) is Tick tick)
								Repository.KeepOrganizedInStorage(tick);
						}
						fo = fo.AddDays(1);
					}
					CheckTheInformationReceivedOnTheDay();
					return;
			}
		}));
		void RequestBalanceInquiry()
		{
			if (connect.Account is not null)
				if (connect is OpenAPI.ConnectAPI o)
				{
					if (Array.Exists(connect.Account, o => string.IsNullOrEmpty(o)) is false && string.IsNullOrEmpty(connect.Account[^1]) is false && connect.Account[^1].Length == 0xA && connect.Account[^1][^2..].Equals("31"))
					{

					}
					o.InputValueRqData(string.Concat(instance, "Opw00005"), string.Concat(connect.Account[0], password)).Send += OnReceiveSecuritiesAPI;
				}
				else
				{

				}
		}
		void BringInStrategics(Strategics strategics, BringIn bring)
		{
			if ((connect as OpenAPI.ConnectAPI).TryGetValue(bring.Code, out Analysis analysis))
			{
				analysis.Wait = false;
				analysis.Consecutive -= analysis.OnReceiveDrawChart;
				analysis.Send -= OnReceiveSecuritiesAPI;

				switch (strategics)
				{
					case Strategics.Long_Position when JsonConvert.DeserializeObject<LongPosition>(bring.Contents) is LongPosition json && analysis.Code.Equals(json.Code):
						analysis.Account = json.Account;
						analysis.Classification = json;
						analysis.Strategics = strategics;
						break;

					case Strategics.Scenario when JsonConvert.DeserializeObject<Scenario>(bring.Contents) is Scenario json && analysis.Code.Equals(json.Code):
						int amount = (int)(json.Maximum / json.Long / analysis.Current), remain = (int)(json.Maximum / json.Short / json.Hope), order = 0;
						DateTime now = DateTime.Now, time = new(now.Year, now.Month, now.Day, Base.CheckIfMarketDelay(now) ? 0xA : 9, 0, 0);
						analysis.Reservation = new Tuple<Queue<string>, Queue<string>>(new Queue<string>(0x20), new Queue<string>(0x20));
						analysis.Classification = new Scenario
						{
							Account = json.Account,
							Code = json.Code,
							Date = remain,
							Hope = json.Hope,
							Maximum = json.Maximum,
							Target = json.Target,
							Trend = json.Trend,
							Short = remain > Base.Tradable ? remain / Base.Tradable + 1 : 1,
							Long = amount > Base.Tradable ? amount / Base.Tradable + 1 : 1
						};
						for (order = 0; order < Base.Tradable * 0x19; order += Base.Tradable * 0x19 / (amount / analysis.Classification.Long))
							analysis.Reservation.Item2.Enqueue(time.AddSeconds(order).ToString(Base.TimeFormat));

						for (order = 0; order < Base.Tradable * 0x19; order += Base.Tradable * 0x19 / (remain / analysis.Classification.Short))
							analysis.Reservation.Item1.Enqueue(time.AddSeconds(order).ToString(Base.TimeFormat));

						analysis.Account = json.Account;
						analysis.Strategics = strategics;
						break;

					default:
						return;
				}
				analysis.Wait = DateTime.Now.Hour > 8;
				analysis.Send += OnReceiveSecuritiesAPI;
				analysis.Consecutive += analysis.OnReceiveDrawChart;

				if (Base.IsDebug && analysis.Reservation is Tuple<Queue<string>, Queue<string>>)
					Base.SendMessage(bring.GetType(), analysis.Name, analysis.Quantity, analysis.Reservation.Item1.Count, analysis.Reservation.Item2.Count, analysis.Purchase, analysis.Current as object, analysis.Classification);
			}
		}
		void ExceptInStrategics(IEnumerable<string> enumerable)
		{
			if (connect is OpenAPI.ConnectAPI api && api.Enumerator?.Where(o => o.Classification is IStrategics) is IEnumerable<Analysis> strategics && strategics.Select(o => o.Code).Except(enumerable) is IEnumerable<string> list)
				foreach (var except in list)
					if (string.IsNullOrEmpty(except) is false && api.TryGetValue(except, out Analysis analysis))
					{
						analysis.Classification = null;

						if (analysis.Classification is null)
						{
							if (analysis.OrderNumber is Dictionary<string, dynamic> numbers && numbers.Count > 0)
								foreach (var kv in numbers.OrderByDescending(o => o.Value))
									if (analysis.Code.Length == 6)
									{
										connect.SendOrder(new Catalog.OpenAPI.Order
										{
											AccNo = connect.Account[0],
											Code = analysis.Code,
											HogaGb = ((int)HogaGb.지정가).ToString("D2"),
											OrgOrderNo = kv.Key,
											OrderType = (int)(analysis.Current < kv.Value ? OrderType.매도취소 : OrderType.매수취소),
											Price = analysis.Current,
											Qty = 1
										});
										if (analysis.Current > kv.Value)
											Cash += kv.Value;
									}
							Base.SendMessage(analysis.GetType(), analysis.Name, analysis.Strategics);
						}
					}
		}
		async void WorkerDoWork(object sender, DoWorkEventArgs e)
		{
			var now = DateTime.Now;

			switch (e.Argument)
			{
				case string[]:
					while (e.Cancel is false)
					{
						try
						{
							switch (await api.GetStrategics(key))
							{
								case IEnumerable<BringIn> enumerable:
									foreach (var bring in enumerable)
										if (Enum.TryParse(bring.Strategics, out Strategics strategics))
										{
											if (worker.WorkerSupportsCancellation && now.CompareTo(new DateTime(bring.Date)) > 0)
												continue;

											BringInStrategics(strategics, bring);
										}
									ExceptInStrategics(enumerable.Select(o => o.Code));
									break;

								case int:
									if (connect is OpenAPI.ConnectAPI api && api.Enumerator?.Where(o => o.Classification is IStrategics) is IEnumerable<Analysis> enumerator)
										foreach (var sis in enumerator)
											if (string.IsNullOrEmpty(sis.Code) is false && api.TryGetValue(sis.Code, out Analysis analysis))
											{
												analysis.Classification = null;

												if (analysis.Classification is null)
												{
													if (analysis.OrderNumber is Dictionary<string, dynamic> numbers && numbers.Count > 0)
														foreach (var kv in numbers.OrderByDescending(o => o.Value))
															if (analysis.Code.Length == 6)
															{
																connect.SendOrder(new Catalog.OpenAPI.Order
																{
																	AccNo = connect.Account[0],
																	Code = analysis.Code,
																	HogaGb = ((int)HogaGb.지정가).ToString("D2"),
																	OrgOrderNo = kv.Key,
																	OrderType = (int)(analysis.Current < kv.Value ? OrderType.매도취소 : OrderType.매수취소),
																	Price = analysis.Current,
																	Qty = 1
																});
																if (analysis.Current > kv.Value)
																	Cash += kv.Value;
															}
													Base.SendMessage(analysis.GetType(), analysis.Name, analysis.Strategics);
												}
											}
									break;
							}
						}
						catch (Exception ex)
						{
							Base.SendMessage(sender.GetType(), ex.StackTrace);
						}
						finally
						{
							if (worker.CancellationPending && worker.IsBusy is false)
							{
								e.Cancel = true;

								if (e.Argument is string[] enumerable)
									foreach (var code in enumerable)
										Base.SendMessage(sender.GetType(), code, now.ToLongTimeString());
							}
							else
							{
								if (worker.WorkerSupportsCancellation is false)
									worker.WorkerSupportsCancellation = true;

								now = DateTime.Now;
								await Task.Delay(0xEA61);
							}
						}
					}
					break;

				case uint page:
					while (page++ < 0x80)
						try
						{
							if (page is 3 or 0x32 && await api.PostContextAsync(await new KRX.Incorporate(Interface.KRX.Catalog.지수구성종목, key).GetConstituentStocks(now, (int)page) as Stack<Catalog.IncorporatedStocks>) == 0xC8)
								await Task.Delay(0x400);

							if (page is 1)
								await Task.Delay(0x1400);

							if (Base.IsDebug is false)
								await new Advertise(key).StartAdvertisingInTheDataCollectionSection(random.Next(7 + now.Hour, 0x2CB));
						}
						catch (Exception ex)
						{
							Base.SendMessage(sender.GetType(), ex.StackTrace);
						}
						finally
						{
							GC.Collect();
						}
					(connect as OpenAPI.ConnectAPI).CorrectTheDelayMilliseconds(Base.IsDebug ? 0x400 : 0x1000);
					CheckTheInformationReceivedOnTheDay();
					break;
			}
		}
		void TimerTick(object sender, EventArgs e)
		{
			var now = DateTime.Now;

			if (connect is null)
			{
				timer.Stop();
				strip.ItemClicked -= StripItemClicked;
				Dispose(connect as Control);
			}
			else if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) is false)
			{
				BeginInvoke(new Action(async () =>
				{
					if (await Security.GetUpdateVisionAsync(server, api))
					{
						timer.Stop();
						strip.ItemClicked -= StripItemClicked;
						Dispose(connect as Control);
					}
					else
						for (int i = 0; i < GetTheCorrectAnswer.Length; i++)
						{
							var temp = 1 + random.Next(0, 0x4B0) * (i + 1);
							GetTheCorrectAnswer[i] = temp < 0x4B1 ? temp : 0x4B0 - i;
						}
					notifyIcon.Icon = icons[^1];
				}));
				WindowState = FormWindowState.Minimized;
			}
			else if (connect.Start)
			{
				if (now.Hour == 5 && now.Minute < 0xA)
				{
					timer.Stop();
					strip.ItemClicked -= StripItemClicked;
					Dispose(connect as Control);

					return;
				}
				else if (notifyIcon.Text.Length == 0 || notifyIcon.Text.Length > 0xF && notifyIcon.Text[^5..].Equals(". . ."))
					notifyIcon.Text = connect.Securities("USER_NAME");

				notifyIcon.Icon = icons[now.Second % 4];
			}
			else if (Visible is false && ShowIcon is false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
			{
				var sat = Base.CheckIfMarketDelay(now);
				now = now.DayOfWeek switch
				{
					DayOfWeek.Sunday => now.AddDays(1),
					DayOfWeek.Saturday => now.AddDays(2),
					DayOfWeek weeks when weeks.Equals(DayOfWeek.Friday) && now.Hour > (sat ? 9 : 8) => now.AddDays(3),
					_ => now.Hour > (sat ? 9 : 8) || Array.Exists(Base.Holidays, o => o.Equals(now.ToString(Base.DateFormat))) ? now.AddDays(1) : now
				};
				sat = Base.CheckIfMarketDelay(now);
				var remain = new DateTime(now.Year, now.Month, now.Day, sat ? 0xA : 9, 0, 0) - DateTime.Now;
				notifyIcon.Text = Base.GetRemainingTime(remain);

				if (connect.Start is false && (remain.TotalMinutes < 0x1F && now.Hour == (sat ? 9 : 8) && now.Minute > 0x1E || api.IsAdministrator && now.Hour == 0x12 && Base.IsDebug) && (remain.TotalMinutes < 0x15 || Array.Exists(GetTheCorrectAnswer, o => o == random.Next(0, 0x4B2))))
				{
					foreach (var kill in new[] { "Rscript", "chromedriver", "cmd" })
						Base.SendMessage(kill);

					GC.Collect();
					notifyIcon.Icon = icons[^2];
					StartProgress(connect as Control);
				}
				else if (api.IsAdministrator && api.IsServer && connect.Start is false && now.Hour == 0 && now.Minute > 0x10 && DateTime.Now.DayOfWeek is not DayOfWeek.Sunday and not DayOfWeek.Monday)
				{
					var worker = new BackgroundWorker();
					notifyIcon.Icon = icons[^2];
					worker.DoWork += WorkerDoWork;
					StartProgress(connect as Control);
					worker.RunWorkerAsync(0U);
				}
			}
		}
		void SecuritiesResize(object sender, EventArgs e) => BeginInvoke(new Action(() =>
		{
			if (WindowState.Equals(FormWindowState.Minimized))
			{
				SuspendLayout();
				Visible = false;
				ShowIcon = false;
				notifyIcon.Visible = true;
				ResumeLayout();
			}
		}));
		void StripItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Name.Equals(reference.Name))
				if (e.ClickedItem.Text.Equals("연결"))
				{
					e.ClickedItem.Text = api.IsAdministrator ? "조회" : "설정";

					if (connect.Start)
						Process.Start(new ProcessStartInfo(@"https://coreapi.shareinvest.net")
						{
							UseShellExecute = connect.Start
						});
					else
						StartProgress(connect as Control);
				}
				else if (e.ClickedItem.Text.Equals("조회"))
					switch (MessageBox.Show(look_up, connect.Securities("USER_NAME"), MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
					{
						case DialogResult.Abort when worker.WorkerSupportsCancellation:
							worker.CancelAsync();
							break;

						case DialogResult.Retry:
							RequestBalanceInquiry();
							break;

						case DialogResult.Ignore:
							RequestTheMissingInformation();
							e.ClickedItem.Text = "설정";
							break;
					}
				else
					Process.Start(new ProcessStartInfo(@"https://coreapi.shareinvest.net")
					{
						UseShellExecute = connect.Start
					});
			else
				Close();
		}
		void JustBeforeFormClosing(object sender, FormClosingEventArgs e)
		{
			if (CloseReason.UserClosing.Equals(e.CloseReason))
				switch (MessageBox.Show(form_exit, connect.Securities("USER_NAME"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
				{
					case DialogResult.OK:
						PreventsFromRunningAgain(e);
						break;

					case DialogResult.Cancel:
						e.Cancel = e.CloseReason.Equals(CloseReason.UserClosing);
						return;
				}
			Dispose(connect as Control);
		}
		void StartProgress(Control connect)
		{
			if (connect is Control)
			{
				Controls.Add(connect);
				connect.Dock = DockStyle.Fill;
				connect.Show();
				FormBorderStyle = FormBorderStyle.None;
				CenterToScreen();
				this.connect.Send += OnReceiveSecuritiesAPI;
				this.connect.StartProgress(version);
			}
			else
				Close();
		}
		void Dispose(Control connect)
		{
			if (connect is Control)
			{
				if (this.connect.ConnectToReceiveRealTime.IsConnected)
					this.connect.Writer.WriteLine($"장시작시간|{GetType()}|{(int)Catalog.OpenAPI.Operation.장종료_시간외종료};{api.IsAdministrator};{Catalog.OpenAPI.Operation.장종료_시간외종료}");

				connect.Dispose();
			}
			Dispose();
		}
		void RequestTheMissingInformation()
		{
			var now = DateTime.Now;

			switch (MessageBox.Show(administrator, Text, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Ignore when now.Hour > 0xF:
					OnReceiveInformationTheDay();
					break;

				case DialogResult.Retry:
					connect.Writer.WriteLine(string.Concat("장시작시간|", GetType(), Base.CheckIfMarketDelay(now) ? "|3;100000;000000" : "|3;090000;000000"));
					return;

				case DialogResult.Abort when (now.Hour < 5 || now.DayOfWeek is DayOfWeek.Sunday or DayOfWeek.Saturday) && api.IsAdministrator || Base.IsDebug:
					var worker = new BackgroundWorker();
					worker.DoWork += WorkerDoWork;
					worker.RunWorkerAsync(0U);
					break;
			}
			(connect as OpenAPI.ConnectAPI).CorrectTheDelayMilliseconds(Base.IsDebug ? 0x259 : 0xE11);
		}
		string ResetTheAccount(dynamic acc)
		{
			if (acc is string[] accounts && accounts.Length > 1)
			{
				var sb = new StringBuilder();

				foreach (var param in accounts)
					if (string.IsNullOrEmpty(param) is false)
						sb.AppendLine(param.Substring(0, 8).Insert(4, "－"));

				return $"{(connect.Securities("GetServerGubun").Equals("1") ? "Mock Investment" : connect.Securities("USER_ID"))}\n{sb}\nThere seems to be a problem with the Account registration process.\n\nAfter Initialization,\nproceed Again.";
			}
			return string.Empty;
		}
		Queue<string> Codes
		{
			get;
		}
		Reservation Reservation
		{
			get; set;
		}
		Catalog.Models.Privacies Privacy
		{
			get; set;
		}
		Catalog.OpenAPI.Operation Operation
		{
			get; set;
		}
		int[] GetTheCorrectAnswer
		{
			get;
		}
		long Cash
		{
			get; set;
		}
		readonly bool version;
		readonly string key;
		readonly Random random;
		readonly API api;
		readonly GoblinBat server;
		readonly Icon[] icons;
		readonly ISecuritiesAPI<SendSecuritiesAPI> connect;
		[Conditional("DEBUG")]
		static void PreventsFromRunningAgain(FormClosingEventArgs e) => e.Cancel = true;
	}
}