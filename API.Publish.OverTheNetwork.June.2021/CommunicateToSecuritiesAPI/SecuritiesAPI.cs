using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Catalog.Models;
using ShareInvest.Client;
using ShareInvest.EventHandler;
using ShareInvest.Interface;

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
			collection = new Dictionary<string, Codes>();
			server = GoblinBat.GetInstance(key);
			timer.Start();
		}
		void OnReceiveInformationTheDay() => BeginInvoke(new Action(async () =>
		{
			if (Codes.Count > 0)
			{
				var now = DateTime.Now;

				if (await api.GetContextAsync(Codes.Dequeue()) is Retention retention && string.IsNullOrEmpty(retention.Code) is false)
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
				Dispose(0x1CE3);
		}));
		void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e) => BeginInvoke(new Action(async () =>
		{
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

							break;

						case Catalog.OpenAPI.Operation.장시작:

							break;

						case Catalog.OpenAPI.Operation.장마감전_동시호가:

							break;

						case Catalog.OpenAPI.Operation.장마감:
							OnReceiveInformationTheDay();
							break;

						case Catalog.OpenAPI.Operation.시간외_단일가_매매종료:

							break;
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
					if (param.MaturityMarketCap.Contains(Base.TransactionSuspension) is false)
					{
						connect.Writer.WriteLine(string.Concat(param.GetType().Name, '|', param.Code));
						collection[tr.Item1] = param;

						if (param.Code.Length == 6 || param.Code.Length == 8 && param.Code[1] is '0')
							Codes.Enqueue(param.Code);
					}
					if (param.Code.Length == 8)
					{
						if (Codes.TryPeek(out string code) && code.Length > 8)
						{
							var temp = Codes.Dequeue().Split('_');
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
					while (hold.Count > 0)
					{
						var ing = hold.Dequeue();
						var convey = string.Empty;

						if (ing[0].Length == 8 && int.TryParse(ing[4], out int quantity) && double.TryParse(ing[9], out double fRate) && long.TryParse(ing[8], out long fValuation) && double.TryParse(ing[6], out double fCurrent) && double.TryParse(ing[5], out double fPurchase))
							convey = string.Concat(sender.GetType().Name, inquiry, ing[0], ';', ing[1].Equals(ing[0]) && collection.TryGetValue(ing[1], out Codes c) ? c.Name : ing[1], ';', ing[2].Equals("1") ? -quantity : quantity, ';', fPurchase, ';', fCurrent, ';', fValuation, ';', fRate * 1e-2);

						else if (ing[3].Length > 0 && ing[3][0] == 'A' && double.TryParse(ing[0xC]?.Insert(6, "."), out double ratio) && long.TryParse(ing[0xB], out long valuation) && int.TryParse(ing[6], out int reserve) && uint.TryParse(ing[8], out uint purchase) && uint.TryParse(ing[7], out uint current))
							convey = string.Concat(sender.GetType().Name, inquiry, ing[3][1..].Trim(), ';', ing[4].Trim(), ';', reserve, ';', purchase, ';', current, ';', valuation, ';', ratio);
					}
					var name = sender.GetType().Name;
					(connect as OpenAPI.ConnectAPI).RemoveValueRqData(name, string.Concat(connect.Account[name.EndsWith("Opw00005") ? 0 : ^1], password)).Send -= OnReceiveSecuritiesAPI;
					return;

				case Tuple<long, long> balance:
					var bal = string.Concat(sender.GetType().Name, inquiry, balance.Item1, ';', balance.Item2);
					return;

				case Tuple<string, Stack<string>> charts:
					(connect as OpenAPI.ConnectAPI).RemoveValueRqData(sender.GetType().Name, charts.Item1).Send -= OnReceiveSecuritiesAPI;

					if ((charts.Item1.Length == 8 ? (charts.Item1[0] > '1' ? await api.PostContextAsync(Catalog.Models.Convert.ToStoreInOptions(charts.Item1, charts.Item2)) : await api.PostContextAsync(Catalog.Models.Convert.ToStoreInFutures(charts.Item1, charts.Item2))) : await api.PostContextAsync(Catalog.Models.Convert.ToStoreInStocks(charts.Item1, charts.Item2))) > 0xC7)
					{
						var message = string.Format("Collecting Datum on {0}.\nStill {1} Stocks to be Collect.", charts.Item1.Length == 6 && collection.TryGetValue(charts.Item1, out Codes mc) ? mc.Name : charts.Item1, Codes.Count.ToString("N0"));
						notifyIcon.Text = message.Length < 0x40 ? message : string.Format("Still {0} Stocks to be Collect.", Codes.Count.ToString("N0"));
					}
					break;

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
						Security.SetKey(this.connect.Securities("USER_ID"));

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
					}
					return;

				case short error:
					Dispose(connect as Control);
					return;
			}
			OnReceiveInformationTheDay();
		}));
		void RequestBalanceInquiry()
		{
			if (connect.Account is not null)
			{
				if (connect is OpenAPI.ConnectAPI o)
				{
					if (connect.Account[^1][^2..].Equals("31"))
					{

					}
					o.InputValueRqData(string.Concat(instance, "Opw00005"), string.Concat(connect.Account[0], password)).Send += OnReceiveSecuritiesAPI;
				}
				SendReservation(MessageBox.Show("Time to go back 5 minutes from the early start bell.", "Temporary Code for Debugging", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2));
			}
		}
		[Conditional("DEBUG")]
		void SendReservation(DialogResult result)
		{
			if (DialogResult.OK.Equals(result))
			{
				var now = DateTime.Now;
				connect.Writer.WriteLine(string.Concat("장시작시간|", GetType(), now.Hour < 0x11 && Base.CheckIfMarketDelay(now) ? "|0;095500;" : "|0;085500;", now.ToString("HH:mm:ss.ffff"), ';', typeof(Catalog.OpenAPI.Operation)));
			}
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
				if (notifyIcon.Text.Length == 0 || notifyIcon.Text.Length > 0xF && notifyIcon.Text[^5..].Equals(". . ."))
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
					_ => now.Hour > (sat ? 9 : 8) || Array.Exists(Base.Holidays, o => o.Equals(now.ToString(Base.DateFormat))) ? now.AddDays(1) : now,
				};
				sat = Base.CheckIfMarketDelay(now);
				var remain = new DateTime(now.Year, now.Month, now.Day, sat ? 0xA : 9, 0, 0) - DateTime.Now;
				notifyIcon.Text = Base.GetRemainingTime(remain);

				if (connect.Start is false && remain.TotalMinutes < 0x1F && now.Hour == (sat ? 9 : 8) && now.Minute > 0x1E && (remain.TotalMinutes < 0x15 || Array.Exists(GetTheCorrectAnswer, o => o == random.Next(0, 0x4B2))))
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

						if (e.Cancel is false && connect.Writer is not null)
						{
							Dispose(0xF75);

							return;
						}
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
				connect.Dispose();

			Dispose();
		}
		void Dispose(int milliseconds)
		{
			connect.Writer.WriteLine(string.Concat("장시작시간|", GetType(), "|d;", DateTime.Now.ToString("HH:mm:ss.ffff"), ';', typeof(Catalog.OpenAPI.Operation)));
			Thread.Sleep(milliseconds);
			Dispose(connect as Control);
		}
		void RequestTheMissingInformation()
		{
			var now = DateTime.Now;

			switch (MessageBox.Show(administrator, Text, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Ignore when now.Hour > 0xF:
					if (Base.IsDebug is false && api.IsAdministrator)
						(connect as OpenAPI.ConnectAPI).CorrectTheDelayMilliseconds(0xE11);

					OnReceiveInformationTheDay();
					return;

				case DialogResult.Retry:
					connect.Writer.WriteLine(string.Concat("장시작시간|", GetType(), Base.CheckIfMarketDelay(now) ? "|0;095000;" : "|0;085000;", now.ToString("HH:mm:ss.ffff"), ';', typeof(Catalog.OpenAPI.Operation)));
					break;

				case DialogResult.Abort:
					return;
			}
		}
		Queue<string> Codes
		{
			get;
		}
		int[] GetTheCorrectAnswer
		{
			get;
		}
		readonly string key;
		readonly Random random;
		readonly API api;
		readonly GoblinBat server;
		readonly Dictionary<string, Codes> collection;
		readonly Icon[] icons;
		readonly ISecuritiesAPI<SendSecuritiesAPI> connect;
		[Conditional("DEBUG")]
		static void PreventsFromRunningAgain(FormClosingEventArgs e) => e.Cancel = true;
	}
}