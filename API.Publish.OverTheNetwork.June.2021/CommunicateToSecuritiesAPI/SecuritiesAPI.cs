using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;

using ShareInvest.Catalog.Models;
using ShareInvest.Client;
using ShareInvest.EventHandler;
using ShareInvest.Interface;

namespace ShareInvest
{
	sealed partial class SecuritiesAPI : Form
	{
		internal SecuritiesAPI(dynamic param, ISecuritiesAPI<SendSecuritiesAPI> connect)
		{
			icons = new[] { Properties.Resources.sleep_adium_bird_20649, Properties.Resources.idle_adium_bird_20651, Properties.Resources.awake_adium_bird_20653, Properties.Resources.alert_adium_bird_20655, Properties.Resources.away_adium_bird_20654, Properties.Resources.invisible_adium_bird_20650 };
			InitializeComponent();
			var key = Security.GetAdministrator(param);
			this.connect = connect;
			this.key = key;
			api = API.GetInstance(key);
			random = new Random(Guid.NewGuid().GetHashCode());
			Codes = new Queue<string>();
			GetTheCorrectAnswer = new int[Security.Initialize(param)];
			collection = new Dictionary<string, Codes>();
			server = GoblinBat.GetInstance(key);
			timer.Start();
		}
		void OnReceiveInformationTheDay() => BeginInvoke(new Action(async () =>
		{
			if (Codes.Count > 0)
			{
				if (await api.PostContextAsync(new Retention { Code = Codes.Dequeue() }) is Retention retention && string.IsNullOrEmpty(retention.Code) is false)
					switch (connect)
					{
						case OpenAPI.ConnectAPI o:
							o.InputValueRqData(string.Concat(instance, retention.Code.Length == 8 ? (retention.Code[0] > '1' ? "Opt50066" : "Opt50028") : "Opt10079"), string.Concat(retention.Code, ';', retention.LastDate)).Send += OnReceiveSecuritiesAPI;
							return;
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

				case Tuple<string, string, string, string, int> tr when string.IsNullOrEmpty(tr.Item1) is false:
					var param = new Codes
					{
						Code = tr.Item1,
						Name = tr.Item2,
						MaturityMarketCap = tr.Item3,
						Price = tr.Item4,
						MarginRate = tr.Item5
					};
					if (param.Code.Length == 6 || param.Code.Length == 8 && param.Code[1] == '0')
						Codes.Enqueue(param.Code);

					if (param.Code.Length == 8)
					{
						if (Codes.TryPeek(out string code) && code.Length > 8)
						{
							var temp = Codes.Dequeue().Split('_');
							(connect as OpenAPI.ConnectAPI).RemoveValueRqData(temp[0], temp[^1]).Send -= OnReceiveSecuritiesAPI;
						}
						(connect as OpenAPI.ConnectAPI).RemoveValueRqData(sender.GetType().Name, param.Code).Send -= OnReceiveSecuritiesAPI;
					}
					if ((param.Code.Length == 6 || param.Code.Length == 8 && param.Code[0] > '1') && await api.PutContextAsync(param) is string response && param.Code.Equals(response))
						notifyIcon.Text = param.Name;

					if (param.MaturityMarketCap.Contains(Base.TransactionSuspension) is false)
						connect.Writer.WriteLine(string.Concat(param.GetType().Name, '|', param.Code));

					collection[tr.Item1] = param;
					return;

				case Tuple<string, string> request:
					if (request.Item2.Length != 8)
						Codes.Enqueue(string.Concat(request.Item1, '_', request.Item2));

					(sender as OpenAPI.ConnectAPI).InputValueRqData(string.Concat(instance, request.Item1), request.Item2).Send += OnReceiveSecuritiesAPI;
					return;

				case Codes codes:
					if (string.IsNullOrEmpty(await api.PutContextAsync(codes) as string))
						Base.SendMessage(sender.GetType(), codes.Name, codes.MaturityMarketCap);

					return;

				case string message:
					if (await server.PostContextAsync(new Catalog.Models.Message { Convey = message }) is int)
						notifyIcon.Text = string.Concat(DateTime.Now.ToLongTimeString(), " ", message);

					return;

				case Queue<string[]> hold:
					while (hold.Count > 0)
					{
						var ing = hold.Dequeue();
						var convey = string.Empty;

						if (ing[0].Length == 8 && int.TryParse(ing[4], out int quantity) && double.TryParse(ing[9], out double fRate)
							&& long.TryParse(ing[8], out long fValuation) && double.TryParse(ing[6], out double fCurrent) && double.TryParse(ing[5], out double fPurchase))
							convey = string.Concat(sender.GetType().Name, inquiry, ing[0], ';', ing[1].Equals(ing[0]) && collection.TryGetValue(ing[1], out Codes c)
								? c.Name : ing[1], ';', ing[2].Equals("1") ? -quantity : quantity, ';', fPurchase, ';', fCurrent, ';', fValuation, ';', fRate * 1e-2);

						else if (ing[3].Length > 0 && ing[3][0] == 'A' && double.TryParse(ing[0xC]?.Insert(6, "."), out double ratio)
							&& long.TryParse(ing[0xB], out long valuation) && int.TryParse(ing[6], out int reserve)
							&& uint.TryParse(ing[8], out uint purchase) && uint.TryParse(ing[7], out uint current))
							convey = string.Concat(sender.GetType().Name, inquiry, ing[3][1..].Trim(), ';', ing[4].Trim(), ';', reserve, ';', purchase, ';', current, ';', valuation, ';', ratio);
					}
					var name = sender.GetType().Name;
					(connect as OpenAPI.ConnectAPI).RemoveValueRqData(name, string.Concat(connect.Account[name.EndsWith("Opw00005") ? 0 : ^1], password)).Send -= OnReceiveSecuritiesAPI;
					return;

				case Tuple<long, long> balance:
					var bal = string.Concat(sender.GetType().Name, inquiry, balance.Item1, ';', balance.Item2);
					return;

				case Tuple<string, Stack<string>> charts:
					switch (sender)
					{
						case OpenAPI.ConnectAPI o:
							o.RemoveValueRqData(sender.GetType().Name, charts.Item1).Send -= OnReceiveSecuritiesAPI;
							break;
					}
					if ((charts.Item1.Length == 8 ? (charts.Item1[0] > '1'
						? await api.PostContextAsync(Catalog.Models.Convert.ToStoreInOptions(charts.Item1, charts.Item2))
						: await api.PostContextAsync(Catalog.Models.Convert.ToStoreInFutures(charts.Item1, charts.Item2)))
						: await api.PostContextAsync(Catalog.Models.Convert.ToStoreInStocks(charts.Item1, charts.Item2))) > 0xC7)
					{
						OnReceiveInformationTheDay();
						var message = string.Format("Collecting Datum on {0}.\nStill {1} Stocks to be Collect.",
							charts.Item1.Length == 6 && collection.TryGetValue(charts.Item1, out Codes mc) ? mc.Name : charts.Item1, Codes.Count.ToString("N0"));
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
						connect.Account = new string[2];

						foreach (var str in accounts)
							if (str.Length == 0xA && str[^2..].CompareTo("32") < 0)
							{
								if (str[^2..].CompareTo("31") == 0)
									connect.Account[^1] = str;

								else
									connect.Account[0] = str;
							}
						foreach (var ctor in (connect as OpenAPI.ConnectAPI)?.Chejan)
							ctor.Send += OnReceiveSecuritiesAPI;
					}
					return;

				case short error:
					Dispose(connect as Control);
					return;
			}
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
				connect.Writer
					.WriteLine(string.Concat("장시작시간|", GetType(), now.Hour < 0x11 && Base.CheckIfMarketDelay(now) ? "|0;095500;" : "|0;085500;", now.ToString("HH:mm:ss.ffff"), ';', typeof(Catalog.OpenAPI.Operation)));
			}
		}
		void WorkerDoWork(object sender, DoWorkEventArgs e)
		{
			if (e.Argument is NamedPipeClientStream client)
				using (var sr = new StreamReader(client))
					try
					{
						while (client.IsConnected)
						{
							var param = sr.ReadLine();

							if (string.IsNullOrEmpty(param) == false)
							{
								var temp = param.Split('|');

								if (temp[0].Length == 5)
								{
									var order = temp[^1].Split(';');

									switch (connect)
									{
										case OpenAPI.ConnectAPI o:
											if (order[0].Length == 6 && int.TryParse(order[1], out int type)
												&& int.TryParse(order[2], out int price)
												&& int.TryParse(order[3], out int quantity))
												o.SendOrder(new Catalog.OpenAPI.Order
												{
													AccNo = connect.Account[0],
													Code = order[0],
													OrderType = type,
													Price = price,
													Qty = quantity,
													HogaGb = order[4],
													OrgOrderNo = order[5]
												});
											break;
									}
								}
								else if (temp[0].Length == 9)
									switch (temp[^1])
									{
										case "085000":
										case "095000":
											RequestBalanceInquiry();
											break;

										case "8":
											OnReceiveInformationTheDay();
											break;

										case "d":
											Dispose(connect as Control);
											break;
									}
								else if (temp[0].Length == 8)
								{
									if (string.IsNullOrEmpty(temp[^1]))
									{

									}
									else
										connect.Account = temp[^1].Split(';');
								}
								Base.SendMessage(sender.GetType(), temp[0], temp[^1]);
							}
						}
					}
					catch (Exception ex)
					{
						Base.SendMessage(sender.GetType(), ex.StackTrace);
					}
					finally
					{
						client.Close();
						client.Dispose();
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
				for (int i = 0; i < GetTheCorrectAnswer.Length; i++)
				{
					var temp = 1 + random.Next(0, 0x4B0) * (i + 1);
					GetTheCorrectAnswer[i] = temp < 0x4B1 ? temp : 0x4B0 - i;
				}
				notifyIcon.Icon = icons[^1];
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
				now = now.DayOfWeek switch
				{
					DayOfWeek.Sunday => now.AddDays(1),
					DayOfWeek.Saturday => now.AddDays(2),
					DayOfWeek weeks when weeks.Equals(DayOfWeek.Friday) && now.Hour > 8 => now.AddDays(3),
					_ => now.Hour > 8 || Array.Exists(Base.Holidays, o => o.Equals(now.ToString(Base.DateFormat))) ? now.AddDays(1) : now,
				};
				var sat = Base.CheckIfMarketDelay(now);
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
						Process.Start(new ProcessStartInfo(Local.Url) { UseShellExecute = connect.Start });

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
			var message = string.Empty;
			var now = DateTime.Now;

			switch (MessageBox.Show(administrator, Text, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
			{
				case DialogResult.Ignore when now.Hour > 0xF:
					message = string.Concat("장시작시간|", GetType(), "|8;", now.ToString("HH:mm:ss.ffff"), ';', typeof(Catalog.OpenAPI.Operation));
					break;

				case DialogResult.Retry:
					message
						= string.Concat("장시작시간|", GetType(), now.Hour < 0x11 && Base.CheckIfMarketDelay(now) ? "|0;095000;" : "|0;085000;", DateTime.Now.ToString("HH:mm:ss.ffff"), ';', typeof(Catalog.OpenAPI.Operation));
					break;

				case DialogResult.Abort:
					return;
			}
			if (string.IsNullOrEmpty(message) == false)
				connect.Writer.WriteLine(message);
		}
		Queue<string> Codes
		{
			get;
		}
		int[] GetTheCorrectAnswer
		{
			get;
		}
		const string administrator = "☞중단\n'실행'을 취소합니다.\n\n☞재시도\n임의로 '계좌번호'를 불러옵니다.\n\n☞무시\n프로그램을 '장마감'이후로 변경합니다.";
		const string api_exit = "해당 프로그램과 연결된 모든 프로세스를 종료합니다.";
		const string look_up = "'조회'는 장시작 5분전 자동으로 실행됩니다.\n\n강제로 프로그램을 실행하지 않았다면\n사용을 '중단'할 것을 권장합니다.\n\n☞중단\n'조회'를 취소합니다.\n\n☞재시도\n'조회'가 실행되면 장시작 설정으로 초기화됩니다.\n\n☞무시\n'관리자'기능입니다.\n임의로 사용할 경우 프로그램 '오작동'을 유발합니다.";
		const string form_exit = "사용자 종료시 데이터 소실 및 오류를 초래합니다.\n\n그래도 종료하시겠습니까?\n\n프로그램 종료후 자동으로 재부팅됩니다.";
		const string instance = "ShareInvest.OpenAPI.Catalog.";
		const string password = ";;00";
		const string inquiry = @"_잔고조회|";
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