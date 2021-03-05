using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

using ShareInvest.Catalog.Models;
using ShareInvest.Client;
using ShareInvest.EventHandler;

namespace ShareInvest
{
	sealed partial class CoreAPI : Form
	{
		internal CoreAPI(dynamic param)
		{
			InitializeComponent();
			icon = new[] { Properties.Resources.upload_server_icon_icons_com_76732, Properties.Resources.download_server_icon_icons_com_76720, Properties.Resources.data_server_icon_icons_com_76718 };
			var initial = Initialize(param);
			key = initial.Item1;
			api = API.GetInstance(key);
			pipe = new Pipe(api.GetType().Name, typeof(CoreAPI).Name, initial.Item2);

			if (Environment.ProcessorCount > 4)
			{
				if (Environment.ProcessorCount > 6)
				{
					if (api.IsInsider || Base.IsDebug)
						theme = new BackgroundWorker();
				}
				if (Base.IsDebug is false)
					search = new BackgroundWorker();

				big = new BackgroundWorker();
			}
			timer.Start();
		}
		string Message
		{
			get; set;
		}
		void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e) => BeginInvoke(new Action(() =>
		{
			switch (e.Convey)
			{
				case string:
					Message = e.Convey as string;
					return;

				case short exit:
					Dispose(exit);
					return;
			}
		}));
		void TimerTick(object sender, EventArgs e)
		{
			if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) is false)
			{
				pipe.Send += OnReceiveSecuritiesAPI;
				WindowState = FormWindowState.Minimized;
				Visible = false;
				ShowIcon = false;
				notifyIcon.Visible = true;

				if (theme is BackgroundWorker)
					theme.DoWork += new DoWorkEventHandler(WorkerDoWork);

				if (search is BackgroundWorker)
					search.DoWork += new DoWorkEventHandler(WorkerDoWork);

				if (big is BackgroundWorker)
					big.DoWork += new DoWorkEventHandler(WorkerDoWork);
			}
			else if (string.IsNullOrEmpty(pipe.Message))
			{
				BeginInvoke(new Action(async () =>
				{
					if (await api.GetContextAsync(new Catalog.TrendsToCashflow()) is IEnumerable<Interface.IStrategics> enumerable)
						worker.RunWorkerAsync(enumerable);

					if (theme is BackgroundWorker)
						theme.RunWorkerAsync(uint.MinValue);

					if (big is BackgroundWorker && await api.GetConfirmAsync(new Catalog.Dart.Theme()) is List<Catalog.Models.Theme> shuffle)
						big.RunWorkerAsync(shuffle.OrderBy(o => Guid.NewGuid()));

					if (search is BackgroundWorker && await api.GetConfirmAsync(new Catalog.Dart.Theme()) is List<Catalog.Models.Theme> list)
						search.RunWorkerAsync(list);
				}));
				pipe.StartProgress();
			}
			else if (Visible is false && ShowIcon is false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
			{
				if (string.IsNullOrEmpty(Message))
					Message = pipe.Message;

				else
				{
					notifyIcon.Icon = Message.EndsWith(false.ToString()) ? icon[^1] : icon[DateTime.Now.Second % 2];
					notifyIcon.Text = Message.Length < 0x40 ? Message : Message.Substring(0, 0x3F);
				}
			}
		}
		async void WorkerDoWork(object sender, DoWorkEventArgs e)
		{
			if (await api.GetContextAsync(new Codes(), 6) is List<Codes> list)
			{
				var now = DateTime.Now;
				var page = uint.MaxValue;
				await Task.Delay(Base.IsDebug ? 0x400 : new Random(Guid.NewGuid().GetHashCode()).Next(0x32000, 0x64000));

				switch (e.Argument)
				{
					case List<Catalog.Models.Theme> theme:
						foreach (var cn in list.OrderBy(o => Guid.NewGuid()))
						{
							if (cn.MaturityMarketCap.Contains(Base.TransactionSuspension) is false && cn.MarginRate > 0)
								try
								{
									var contents = new Stack<string>();

									if (int.TryParse(cn.Code, out int length) && await new Naver.Search(key).SearchForKeyword(HttpUtility.UrlEncode(cn.Name), length) is Queue<string> queue)
										while (queue.TryDequeue(out string url))
											if (await new Naver.Search(key).BrowseTheSite(url) is (string, Stack<string>) co)
											{
												while (co.Item2.TryPop(out string text))
													contents.Push(text);

												contents.Push(co.Item1);
											}
									if (contents.Count > 0)
									{
										contents.Push(cn.Code);
										contents.Push(cn.Name);
									}
								}
								catch (Exception ex)
								{
									Base.SendMessage(sender.GetType(), cn.Name, ex.StackTrace);
								}
								finally
								{
									await Task.Delay(0x200);
									now = DateTime.Now;
								}
							if (now.Hour == 8)
							{
								if (now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || Array.Exists(Base.Holidays, o => o.Equals(now.ToString(Base.DateFormat))))
									continue;

								break;
							}
						}
						break;

					case IEnumerable<Catalog.Models.Theme> enumerable:
						foreach (var cn in list.OrderBy(o => Guid.NewGuid()))
						{
							if (cn.MaturityMarketCap.Contains(Base.TransactionSuspension) is false && cn.MarginRate > 0)
								try
								{
									if (int.TryParse(cn.Code, out int _) && await new Naver.Search(key).VisualizeTheResultsOfAnAnalysis(cn.Name) is (List<Catalog.KRX.Cloud>, Dictionary<string, string>) cloud)
										await new Advertise(key).TransmitCollectedInformation(cn, cloud);
								}
								catch (Exception ex)
								{
									Base.SendMessage(sender.GetType(), cn.Name, ex.StackTrace);
								}
								finally
								{
									await Task.Delay(0x200);
									now = DateTime.Now;
								}
							if (now.Hour == 8)
							{
								if (now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || Array.Exists(Base.Holidays, o => o.Equals(now.ToString(Base.DateFormat))))
									continue;

								break;
							}
						}
						break;

					case uint arg:
						while (++arg > 0 && arg < page)
						{
							if (new Client.Theme(key).OnReceiveMarketPriceByTheme((int)arg) is (uint, IEnumerable<Catalog.Models.Theme>) enumerable)
								foreach (var theme in enumerable.Item2)
									try
									{
										if (await api.PostContextAsync(theme) is Catalog.Dart.Theme st && new Client.Theme(key).GetDetailsFromGroup(st.Index, 4) is Queue<GroupDetail> queue)
											while (queue.TryDequeue(out GroupDetail detail))
												if (list.Any(o => o.Code.Equals(detail.Code) && o.MaturityMarketCap.Contains(Base.TransactionSuspension) is false && o.MarginRate > 0) && await api.GetConfirmAsync(detail) is string index && detail.Index.Equals(index) is false)
												{
													var find = list.First(o => o.Code.Equals(detail.Code));
													var bring = new Indicators.BringInTheme(key, api, detail, find);
													var slope = new Indicators.Slope(find.Name);
													bring.Send += slope.OnReceiveCurrentLocation;

													if (await bring.StartProgress() is double percent)
													{
														bring.Send -= slope.OnReceiveCurrentLocation;

														if (await api.PostContextAsync(new GroupDetail
														{
															Code = detail.Code,
															Compare = detail.Compare,
															Current = detail.Current,
															Date = slope.Date,
															Index = detail.Index,
															Rate = detail.Rate,
															Title = detail.Title,
															Percent = percent,
															Tick = (int[])Enum.GetValues(typeof(Interface.KRX.Line)),
															Inclination = slope.CalculateTheSlope

														}) is int change && change > 0)
															Base.SendMessage(bring.GetType(), find.Name, change);
													}
												}
												else
													Base.SendMessage(detail.GetType(), $"There is Data on the same day of {list.Find(o => o.Code.Equals(detail.Code)).Name}.");

										page = enumerable.Item1;
									}
									catch (Exception ex)
									{
										Base.SendMessage(sender.GetType(), theme.Name, ex.StackTrace);
									}
									finally
									{
										await Task.Delay(0xC00);
										now = DateTime.Now;
									}
							if (now.Hour == 8)
							{
								if (now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || Array.Exists(Base.Holidays, o => o.Equals(now.ToString(Base.DateFormat))))
									continue;

								break;
							}
						}
						break;

					case IEnumerable<Interface.IStrategics> enumerable:
						foreach (Catalog.TrendsToCashflow analysis in enumerable)
						{
							foreach (var ch in list.OrderBy(o => Guid.NewGuid()))
								try
								{
									if (string.IsNullOrEmpty(ch.Price) is false && (ch.MarginRate == 1 || ch.MarginRate == 2) && ch.MaturityMarketCap.StartsWith(Base.Margin) && int.TryParse(ch.Price, out int price) && price > 0 && ch.MaturityMarketCap.Contains(Base.TransactionSuspension) is false && await api.PostConfirmAsync(new Catalog.ConfirmStrategics
									{
										Code = ch.Code,
										Date = now.Hour > 0xF ? now.ToString(Base.DateFormat) : now.AddDays(-1).ToString(Base.DateFormat),
										Strategics = string.Concat("TC.", analysis.AnalysisType)

									}) is false)
									{
										var estimate = new Indicators.AnalyzeFinancialStatements(await api.GetContextAsync(new Catalog.FinancialStatement { Code = ch.Code }) as List<Catalog.FinancialStatement>, analysis.AnalysisType.ToCharArray()).Estimate;
										var cf = new Indicators.TrendsToCashflow
										{
											Code = ch.Code,
											Strategics = analysis,
											Market = ch.MarginRate == 1,
											Name = ch.Name,
											Purchase = 0,
											Quantity = 0,
											Rate = 0,
											Revenue = 0
										};
										var bring = new Indicators.BringInInformation(ch, await api.GetContextAsync(new Catalog.Strategics.RevisedStockPrice { Code = ch.Code }) as Queue<Catalog.Strategics.ConfirmRevisedStockPrice>, api);
										bring.Send += cf.OnReceiveDrawChart;
										cf.StartProgress(Base.Tax);
										await bring.StartProgress();
										var statistics = cf.SendMessage;

										if (estimate is not null && estimate.Count > 3 && string.IsNullOrEmpty(statistics.Key) is false)
										{
											var normalize = estimate.Last(o => o.Key.ToString(Base.FullDateFormat).StartsWith(statistics.Date)).Value;
											var near = Base.FindTheNearestQuarter(DateTime.TryParseExact(statistics.Date, Base.FullDateFormat.Substring(0, 6), CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date) ? date : DateTime.Now);

											if (await api.PutContextAsync(new Catalog.Models.Consensus
											{
												Code = ch.Code,
												Strategics = statistics.Key,
												Date = statistics.Date,
												FirstQuarter = estimate.Last(o => o.Key.ToString(Base.FullDateFormat).StartsWith(near[0])).Value - normalize,
												SecondQuarter = estimate.Last(o => o.Key.ToString(Base.FullDateFormat).StartsWith(near[1])).Value - normalize,
												ThirdQuarter = estimate.Last(o => o.Key.ToString(Base.FullDateFormat).StartsWith(near[2])).Value - normalize,
												Quarter = estimate.Last(o => o.Key.ToString(Base.FullDateFormat).StartsWith(near[3])).Value - normalize,
												TheNextYear = estimate.Last(o => o.Key.ToString(Base.FullDateFormat).StartsWith(near[4])).Value - normalize,
												TheYearAfterNext = estimate.Last(o => o.Key.ToString(Base.FullDateFormat).StartsWith(near[5])).Value - normalize

											}) is int status && status == 0xC8 && statistics.Base > 0 && await api.PutContextAsync(new StocksStrategics
											{
												Code = ch.Code,
												Strategics = statistics.Key,
												Date = statistics.Date,
												MaximumInvestment = (long)statistics.Base,
												CumulativeReturn = statistics.Cumulative / statistics.Base,
												WeightedAverageDailyReturn = statistics.Statistic / statistics.Base,
												DiscrepancyRateFromExpectedStockPrice = statistics.Price

											}) is double coin && double.IsNaN(coin))
												Message = ch.Name;
										}
									}
								}
								catch (Exception ex)
								{
									Base.SendMessage(sender.GetType(), ch.Name, ex.StackTrace);
								}
								finally
								{
									await Task.Delay(0x400);
									now = DateTime.Now;
								}
							if (now.Hour == 8)
							{
								if (now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || Array.Exists(Base.Holidays, o => o.Equals(now.ToString(Base.DateFormat))))
									continue;

								break;
							}
						}
						break;
				}
			}
			(sender as BackgroundWorker).Dispose();
		}
		void Dispose(short param)
		{
			if (param == -0x6A)
			{
				var count = 0;

				while (Base.IsDebug is false && CheckAccessRights(key) && count < 0xC)
				{
					Thread.Sleep(++count * 0x100);

					if (Process.GetProcessesByName(securities.Split('.')[0]).Length == 0)
					{
						if (Base.IsDebug is false)
							Process.Start("shutdown.exe", "-r");

						count = int.MaxValue;
					}
				}
				if (theme is BackgroundWorker)
					theme.Dispose();

				if (search is BackgroundWorker)
					search.Dispose();

				if (big is BackgroundWorker)
					big.Dispose();

				if (worker is BackgroundWorker)
					worker.Dispose();

				Dispose();
			}
		}
		readonly string key;
		readonly API api;
		readonly Pipe pipe;
		readonly BackgroundWorker big;
		readonly BackgroundWorker theme;
		readonly BackgroundWorker search;
		readonly Icon[] icon;
	}
}