using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

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
		internal SecuritiesAPI(dynamic key, ISecuritiesAPI<SendSecuritiesAPI> connect)
		{
			icons = new[] { Properties.Resources.sleep_adium_bird_20649, Properties.Resources.idle_adium_bird_20651, Properties.Resources.awake_adium_bird_20653, Properties.Resources.alert_adium_bird_20655, Properties.Resources.away_adium_bird_20654, Properties.Resources.invisible_adium_bird_20650 };
			InitializeComponent();
			this.connect = connect;
			this.key = key;
			api = API.GetInstance(key);
			random = new Random(Guid.NewGuid().GetHashCode());
			Codes = new Queue<string>();
			GetTheCorrectAnswer = new int[(key as string).Length];
			server = GoblinBat.GetInstance(key);
			timer.Start();
		}
		void OnReceiveInformationTheDay() => BeginInvoke(new Action(async () =>
		{
			if (Codes.TryDequeue(out string str))
			{
				var now = DateTime.Now;

				if (string.IsNullOrEmpty(str) is false && await api.GetContextAsync(str) is Retention retention && string.IsNullOrEmpty(retention.Code) is false)
				{
					if (string.IsNullOrEmpty(retention.LastDate) is false && now.ToString(Base.DateFormat).Equals(retention.LastDate.Substring(0, 6)) || string.IsNullOrEmpty(retention.Code) is false && retention.LastDate is null)
						OnReceiveInformationTheDay();

					else
						try
						{
							if (retention.Code.Length == 6)
							{
								var consensus = new Client.Consensus(key);

								if (consensus.GrantAccess)
								{
									Queue<ConvertConsensus> queue;
									Queue<FinancialStatement> context = null;

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
								else if (await api.GetContextAsync(new Catalog.IncorporatedStocks { Market = 'P' }) is int next && await api.PostContextAsync(new Client.IncorporatedStocks(key).OnReceiveSequentially(next)) != 0xC8)
									Base.SendMessage(retention.Code, next, GetType());
							}
						}
						catch (Exception ex)
						{
							Base.SendMessage(ex.StackTrace, GetType());
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
				if (string.IsNullOrEmpty(code))
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
				case Dictionary<string, string> chejan:
					if (await server.PutContextAsync(sender.GetType(), chejan) is int status)
						Base.SendMessage(sender.GetType(), chejan["종목명"], status);

					return;

				case Tuple<Catalog.OpenAPI.Operation, string, string> operation:
					switch (operation.Item1)
					{
						case Catalog.OpenAPI.Operation.장시작전:
							switch (operation.Item2[2..])
							{
								case reservation:
									foreach (var order in Reservation.Stocks)
									{
										connect.SendOrder(order.Value);
										Base.SendMessage(sender.GetType(), order.Key.ToString("N0"), order.Value.Code);
									}
									return;

								case construction:
									RequestBalanceInquiry();
									return;
							}
							break;

						case Catalog.OpenAPI.Operation.장시작:

							break;

						case Catalog.OpenAPI.Operation.장마감전_동시호가:
							switch (operation.Item2[2..])
							{
								case before_market_closing:
									RequestBalanceInquiry();
									return;

								case market_closing_reservation:
									foreach (var order in Reservation.Stocks)
									{
										connect.SendOrder(order.Value);
										Base.SendMessage(sender.GetType(), order.Key.ToString("N0"), order.Value.Code);
									}
									return;
							}
							break;

						case Catalog.OpenAPI.Operation.장마감:
							OnReceiveInformationTheDay();
							break;

						case Catalog.OpenAPI.Operation.시간외_단일가_매매종료:

							break;

						case Catalog.OpenAPI.Operation.장종료_시간외종료:
							Dispose(connect as Control);
							return;
					}
					notifyIcon.Text = Enum.GetName(typeof(Catalog.OpenAPI.Operation), operation.Item1);
					return;

				case Tuple<string, string, string, string, int> tr when string.IsNullOrEmpty(tr.Item1) is false:
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
						(connect as OpenAPI.ConnectAPI).Append(tr.Item1, new SecondaryIndicators.OpenAPI.Stocks
						{
							Code = tr.Item1,
							Name = tr.Item2,
							Current = classfication ? (int)price : price,
							MarketMarginRate = tr.Item5,
							Offer = classfication ? 0 : 0D,
							Bid = classfication ? 0 : 0D
						});
						if (param.Code.Length == 6 || param.Code.Length == 8 && param.Code[1] is '0')
							Codes.Enqueue(param.Code);
					}
					if (param.Code.Length == 8)
					{
						if (Codes.TryPeek(out string code) && code.Length > 8 && Codes.TryDequeue(out string dequeue))
						{
							var temp = dequeue.Split('_');
							(connect as OpenAPI.ConnectAPI).RemoveValueRqData(temp[0], temp[^1]).Send -= OnReceiveSecuritiesAPI;
						}
						(connect as OpenAPI.ConnectAPI).RemoveValueRqData(sender.GetType().Name, param.Code).Send -= OnReceiveSecuritiesAPI;
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
					(connect as OpenAPI.ConnectAPI).RemoveValueRqData(name, string.Concat(connect.Account[name.EndsWith("Opw00005") ? 0 : ^1], password)).Send -= OnReceiveSecuritiesAPI;

					while (hold.TryDequeue(out string[] ing))
						if (ing[0].Length == 8 && int.TryParse(ing[4], out int quantity) && double.TryParse(ing[9], out double fRate) && long.TryParse(ing[8], out long fValuation) && double.TryParse(ing[6], out double fCurrent) && double.TryParse(ing[5], out double fPurchase) && await server.PostContextAsync(new Balance
						{
							Code = ing[0],
							Name = ing[1].Equals(ing[0]) && (connect as OpenAPI.ConnectAPI).TryGetValue(ing[1], out Analysis analysis) ? analysis.Name : ing[1],
							Quantity = (ing[2].Equals("1") ? -quantity : quantity).ToString("N0"),
							Purchase = fPurchase.ToString(ing[0][1] is '0' ? "N2" : "N0"),
							Current = fCurrent.ToString(ing[0][1] is '0' ? "N2" : "N0"),
							Revenue = fValuation.ToString("C0"),
							Rate = (fRate * 1e-2).ToString("P2"),
							Separation = string.Empty,
							Trend = string.Empty
						}) is 0xC8)
						{
							Base.SendMessage(sender.GetType(), ing[0], quantity);
						}
						else if (ing[3].Length > 0 && ing[3][0] is 'A' && double.TryParse(ing[0xC]?.Insert(6, "."), out double ratio) && long.TryParse(ing[0xB], out long valuation) && int.TryParse(ing[6], out int amount) && uint.TryParse(ing[8], out uint purchase) && uint.TryParse(ing[7], out uint current) && await server.PostContextAsync(new Balance
						{
							Code = ing[3][1..].Trim(),
							Name = ing[4].Trim(),
							Quantity = amount.ToString("N0"),
							Purchase = purchase.ToString("N0"),
							Current = current.ToString("N0"),
							Revenue = valuation.ToString("C0"),
							Rate = ratio.ToString("P2"),
							Trend = string.Empty,
							Separation = string.Empty
						}) is 0xC8)
						{
							if (Reservation is not null && (connect as OpenAPI.ConnectAPI).TryGetValue(ing[3][1..].Trim(), out Analysis held))
							{
								held.Quantity = amount;
								held.Purchase = purchase;
								held.Current = current;
								Reservation.Push((connect as OpenAPI.ConnectAPI).Append(held.Code, held));
							}
							Base.SendMessage(sender.GetType(), ing[4].Trim(), amount);
						}
					return;

				case Tuple<long, long> balance when now.Hour == 8 || now.Hour == 0xF:
					Reservation = new Reservation(balance.Item2, connect.Account);
					return;

				case Tuple<string, Stack<string>> charts:
					(connect as OpenAPI.ConnectAPI).RemoveValueRqData(sender.GetType().Name, charts.Item1).Send -= OnReceiveSecuritiesAPI;

					if ((charts.Item1.Length == 8 ? (charts.Item1[0] > '1' ? await api.PostContextAsync(Catalog.Models.Convert.ToStoreInOptions(charts.Item1, charts.Item2)) : await api.PostContextAsync(Catalog.Models.Convert.ToStoreInFutures(charts.Item1, charts.Item2))) : await api.PostContextAsync(Catalog.Models.Convert.ToStoreInStocks(charts.Item1, charts.Item2))) > 0xC7)
					{
						var message = string.Format("Collecting Datum on {0}.\nStill {1} Stocks to be Collect.", charts.Item1.Length == 6 && (connect as OpenAPI.ConnectAPI).TryGetValue(charts.Item1, out Analysis analysis) ? analysis.Name : charts.Item1, Codes.Count.ToString("N0"));
						notifyIcon.Text = message.Length < 0x40 ? message : string.Format("Still {0} Stocks to be Collect.", Codes.Count.ToString("N0"));
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
					}, accounts.Length > 0)) is 0xC8)
					{
						this.connect.Account = new string[2];
						var connect = this.connect as OpenAPI.ConnectAPI;
						connect.Real.Send += OnReceiveSecuritiesAPI;
						((ISendSecuritiesAPI<SendSecuritiesAPI>)connect.API).Send += OnReceiveSecuritiesAPI;

						foreach (var str in accounts)
							if (str.Length == 0xA && str[^2..].CompareTo("32") < 0)
							{
								if (str[^2..].CompareTo("31") == 0)
									this.connect.Account[^1] = str;

								else
									this.connect.Account[0] = str;
							}
						foreach (var ctor in connect?.Chejan)
							ctor.Send += OnReceiveSecuritiesAPI;

						Security.SetKey(this.connect.Securities("USER_ID"));
						RequestBalanceInquiry();
					}
					return;

				case short error:
					var send = connect as OpenAPI.ConnectAPI;
					var hermes = send.SendErrorMessage(error);

					if (string.IsNullOrEmpty(hermes) is false && await server.PostContextAsync(new Catalog.Models.Message { Convey = string.Format("[{0}] {1}({2})", Math.Abs(error).ToString("D6"), hermes, send.Count.ToString("D4")), Key = Security.Key }) is int)
						notifyIcon.Text = hermes;

					switch (error)
					{
						case -0x6A:
						case -0xC8:
						case -0x64:
							Dispose(connect as Control);
							return;
					}
					return;

				case Tuple<string, Stack<Catalog.Models.RevisedStockPrice>, Queue<Stocks>> models:
					(connect as OpenAPI.ConnectAPI).RemoveValueRqData(sender.GetType().Name, models.Item1).Send -= OnReceiveSecuritiesAPI;

					while (models.Item2.TryPop(out Catalog.Models.RevisedStockPrice revise))
						if (await api.PostContextAsync(revise) is int rsp && rsp != 0xC8)
							Base.SendMessage(sender.GetType(), models.Item1, models.Item2.Count);

					while (models.Item3.TryDequeue(out Stocks stock) && stock.Volume == 0)
						if (await api.PostContextAsync(stock) is string confirm && string.IsNullOrEmpty(confirm) is false)
						{
							Repository.KeepOrganizedInStorage(stock, models.Item3.Count);
							Base.SendMessage(sender.GetType(), models.Item1, stock.Date);
							var str = string.Format("{0} of the {1} data have been deleted.", (connect as OpenAPI.ConnectAPI).TryGetValue(models.Item1, out Analysis analysis) ? analysis.Name : models.Item1, confirm);
							notifyIcon.Text = str.Length < 0x40 ? str : string.Concat(models.Item1, '_', confirm);
						}
					CheckTheInformationReceivedOnTheDay();
					return;

				case Tuple<string, Queue<Stocks>> confirm:
					(connect as OpenAPI.ConnectAPI).RemoveValueRqData(sender.GetType().Name, confirm.Item1).Send -= OnReceiveSecuritiesAPI;

					while (confirm.Item2.TryDequeue(out Stocks stock))
						if (await api.PostContextAsync(stock) is string model && string.IsNullOrEmpty(model) is false)
						{
							Repository.KeepOrganizedInStorage(stock, confirm.Item2.Count);
							Base.SendMessage(sender.GetType(), confirm.Item1, stock.Date);
							var str = string.Format("{0} of the {1} data have been deleted.", (connect as OpenAPI.ConnectAPI).TryGetValue(confirm.Item1, out Analysis analysis) ? analysis.Name : confirm.Item1, model);
							notifyIcon.Text = str.Length < 0x40 ? str : string.Concat(confirm.Item1, '_', model);
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
					if (connect.Account[^1] is not null && connect.Account[^1][^2..].Equals("31"))
					{

					}
					o.InputValueRqData(string.Concat(instance, "Opw00005"), string.Concat(connect.Account[0], password)).Send += OnReceiveSecuritiesAPI;
				}
				else
				{

				}
		}
		[Conditional("DEBUG")]
		void SendReservation(DialogResult result)
		{
			if (DialogResult.OK.Equals(result))
				worker.RunWorkerAsync();
		}
		void WorkerDoWork(object sender, DoWorkEventArgs e)
		{

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
				{
					if (Base.IsDebug is false && api.IsAdministrator && now.Hour > 0x11)
						CheckTheInformationReceivedOnTheDay(now, 0xEA61);

					notifyIcon.Text = connect.Securities("USER_NAME");
				}
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
					notifyIcon.Icon = icons[^2];
					StartProgress(connect as Control);
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
					e.ClickedItem.Text = Base.IsDebug ? "조회" : "설정";

					if (connect.Start)
						Process.Start(new ProcessStartInfo(server.Url) { UseShellExecute = connect.Start });

					else
						StartProgress(connect as Control);
				}
				else if (e.ClickedItem.Text.Equals("조회"))
					switch (MessageBox.Show(look_up, connect.Securities("USER_NAME"), MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
					{
						case DialogResult.Abort:

							break;

						case DialogResult.Retry:
							RequestBalanceInquiry();
							break;

						case DialogResult.Ignore:
							RequestTheMissingInformation();
							break;
					}
				else
					Process.Start(new ProcessStartInfo(server.Url) { UseShellExecute = connect.Start });

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
				this.connect.StartProgress();
			}
			else
				Close();
		}
		void Dispose(Control connect)
		{
			if (connect is Control)
			{
				if (this.connect.ConnectToReceiveRealTime.IsConnected)
					this.connect.Writer.WriteLine(string.Format("장시작시간|{0}|{1};{2};{3}", GetType(), (int)Catalog.OpenAPI.Operation.장종료_시간외종료, api.IsAdministrator, Catalog.OpenAPI.Operation.장종료_시간외종료));

				connect.Dispose();
			}
			Dispose();
		}
		void CheckTheInformationReceivedOnTheDay(DateTime now, int delay) => BeginInvoke(new Action(async () =>
		{
			if (Base.IsDebug)
			{
				foreach (var key in new[] { "10100000", "20100000" })
				{
					await Task.Delay(delay);

					if (await api.PostContextAsync(new Stocks
					{
						Code = key,
						Price = key,
						Date = now.ToString(Base.DateFormat),
						Volume = int.MaxValue,
						Retention = null

					}) is string remove && string.IsNullOrEmpty(remove) is false)
						Base.SendMessage(GetType(), key[0], remove);
				}
				await Task.Delay(delay * (Base.IsDebug ? 1 : 5));
				CheckTheInformationReceivedOnTheDay();
			}
			else
			{
				await Task.Delay(delay * (Base.IsDebug ? 1 : 5));
				OnReceiveInformationTheDay();
			}
		}));
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

				case DialogResult.Abort:
					CheckTheInformationReceivedOnTheDay(now, 0x3000);
					break;
			}
			if (Base.IsDebug is false && api.IsAdministrator)
				(connect as OpenAPI.ConnectAPI).CorrectTheDelayMilliseconds(0xE11);
		}
		Queue<string> Codes
		{
			get;
		}
		Reservation Reservation
		{
			get; set;
		}
		int[] GetTheCorrectAnswer
		{
			get;
		}
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