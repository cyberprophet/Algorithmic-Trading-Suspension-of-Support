using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

using Newtonsoft.Json;

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
			var processor = Environment.ProcessorCount;
			var initial = Initialize(param);
			key = initial.Item1;
			api = API.GetInstance(key);
			pipe = new Pipe(api.GetType().Name, typeof(CoreAPI).Name, initial.Item2);
			random = new Random(Guid.NewGuid().GetHashCode());

			if (processor > 2)
			{
				if (processor > 4)
				{
					if (processor > 6)
					{
						if (api.IsInsider)
							theme = new BackgroundWorker();

						else
							search = new BackgroundWorker();
					}
					log = Path.Combine(Repository.R, "Log.txt");
					script = @"C:\R\R-4.0.4\bin\x64\rscript --verbose ";
					keywords = new BackgroundWorker();
					server = GoblinBat.GetInstance(key);
				}
				if (Status is HttpStatusCode.OK)
					big = new BackgroundWorker();
			}
			timer.Start();
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

				if (keywords is BackgroundWorker)
					keywords.DoWork += new DoWorkEventHandler(WorkerDoWork);
			}
			else if (string.IsNullOrEmpty(pipe.Message))
			{
				BeginInvoke(new Action(async () =>
				{
					if (await api.GetContextAsync(new Catalog.TrendsToCashflow()) is IEnumerable<Interface.IStrategics> enumerable)
						worker.RunWorkerAsync(enumerable);

					if (theme is BackgroundWorker)
						theme.RunWorkerAsync(uint.MinValue);

					if (big is BackgroundWorker)
						big.RunWorkerAsync(Status);

					if (keywords is BackgroundWorker && await api.GetConfirmAsync(new Catalog.Dart.Theme()) is List<Catalog.Models.Theme> shuffle)
					{
						if (new FileInfo(log).Exists)
							File.Delete(log);

						keywords.RunWorkerAsync(shuffle.OrderBy(o => Guid.NewGuid()));
					}
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

				do
					try
					{
						await Task.Delay(Base.IsDebug ? random.Next(0x400, 0x1000) : random.Next(0x32000, 0x64000));
						await new Advertise(key).StartAdvertisingInTheDataCollectionSection(random.Next(7 + now.Hour, 0x1F9));
					}
					catch (Exception ex)
					{
						Base.SendMessage(sender.GetType(), ex.StackTrace, list.Count);
					}
					finally
					{
						now = DateTime.Now;
					}
				while (now.Hour is 0x11 or 0x10 or 0xF && Base.IsDebug is false);

				switch (e.Argument)
				{
					case List<Catalog.Models.Theme> theme:
						foreach (var cn in list.OrderBy(o => Guid.NewGuid()))
						{
							if (cn.MaturityMarketCap.Contains(Base.TransactionSuspension) is false && cn.MarginRate > 0)
								try
								{
									var contents = new Stack<string>();

									if (int.TryParse(cn.Code, out int length) && await new Naver.Search(key).SearchForKeyword(HttpUtility.UrlEncode(cn.Name), length) is Dictionary<string, string> response)
										foreach (var kv in response)
											if (await new Naver.Search(key).BrowseTheSite(kv.Value) is Stack<string> stack)
												while (stack.TryPop(out string text))
													contents.Push(text);

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
						if (Base.IsDebug is false && api.IsAdministrator is false)
						{
							Repository.CreateTheDirectory(new DirectoryInfo(directory));
							File.WriteAllBytes(Path.Combine(directory, initialize), Properties.Resources.initialize);
							File.WriteAllBytes(Path.Combine(directory, tags), Properties.Resources.Tags);
							File.WriteAllBytes(Path.Combine(directory, cloud), Properties.Resources.Cloud);
						}
						foreach (var file in new[] { (@"C:\R\R-4.0.4", @"bin\x64\Rscript.exe", @"https://cran.r-project.org/bin/windows/base"), (@"C:\rtools40", @"usr\bin\make.exe", @"https://cran.r-project.org/bin/windows/Rtools") })
							if (new FileInfo(Path.Combine(file.Item1, file.Item2)).Exists is false)
								using (var process = new Process { StartInfo = new ProcessStartInfo(file.Item3) { UseShellExecute = true } })
								{
									if (process.Start() && DialogResult.OK.Equals(MessageBox.Show("An essential element for program operation is missing.\n\nDownload from the link you are connecting to,\ninstall it in the captioned location\nat the top of the message box,\nand click the 'OK' button.\n\nIf you move on to the next step\nbefore the installation is finished,\nthe program will be terminated.", file.Item1, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)))
									{
										if (new FileInfo(Path.Combine(file.Item1, file.Item2)).Exists)
											continue;
									}
									(sender as BackgroundWorker).Dispose();

									return;
								}
						foreach (var package in new[] { "multilinguer", "hash", "tau", "Sejong", "RSQLite", "devtools", "bit", "rex", "lazyeval", "crosstalk", "promises", "later", "sessioninfo", "xopen", "bit64", "blob", "DBI", "memoise", "plogr", "covr", "DT", "rcmdcheck", "rversions", "wordcloud", "RColorBrewer", "tm", "stringr", "SnowballC", "webshot", "remotes", "htmlwidgets", string.Empty })
							using (var process = new Process
							{
								StartInfo = new ProcessStartInfo
								{
									FileName = "cmd",
									UseShellExecute = false,
									RedirectStandardError = true,
									RedirectStandardInput = true,
									RedirectStandardOutput = true,
									WorkingDirectory = directory
								}
							})
							{
								process.ErrorDataReceived += SortOutputHandler;

								if (process.Start())
								{
									process.BeginErrorReadLine();
									process.StandardInput.WriteLine(string.Concat(script, initialize, string.IsNullOrEmpty(package) ? string.Empty : string.Concat(' ', package)));
									process.StandardInput.Close();
									process.WaitForExit();
								}
								process.ErrorDataReceived -= SortOutputHandler;
							}
						using (var sw = new StreamWriter(Path.Combine(directory, "Codes.csv"), false))
							foreach (var param in list)
								if (param.Name.Split(' ').Length == 1 && param.Name[^1] is not '우' && (param.Name[^1] is '호' && char.IsDigit(param.Name[^2])) is false && param.Name[^1] is not 'B' && param.Name[^1] is not ')' && param.Name.EndsWith("스팩") is false)
									sw.WriteLine(string.Concat(param.Name.Replace("(Reg.S)", string.Empty), '\t', "ncn"));

						foreach (var theme in enumerable)
						{
							try
							{
								var keywords = new Dictionary<string, string>();
								var json = string.Empty;

								if (await api.GetConfirmAsync(theme) is Url url)
								{
									foreach (var str in theme.Name.Split(')'))
										if (string.IsNullOrEmpty(str) is false)
											foreach (var name in str.Split('('))
												if (string.IsNullOrEmpty(name) is false)
													foreach (var sep in name.Split('/'))
														if (string.IsNullOrEmpty(sep) is false)
															foreach (var comma in sep.Split(','))
																if (string.IsNullOrEmpty(comma) is false)
																	foreach (var em in comma.Split('&'))
																		if (string.IsNullOrEmpty(em) is false)
																		{
																			var search = new Stack<string>();

																			if (Array.Exists(exception, o => em.EndsWith(o)) is false)
																				search.Push(em);

																			if (em.Split(' ') is string[] ws && ws.Length > 1)
																				foreach (var tn in ws)
																					if (string.IsNullOrEmpty(tn) is false && Array.Exists(exception, o => tn.EndsWith(o)) is false)
																						search.Push(tn);

																			while (search.TryPop(out string pop))
																			{
																				if (await new Naver.Search(key).SearchForKeyword(HttpUtility.UrlEncode(pop), (int)Math.Pow(key.Length, theme.Index.Length)) is Dictionary<string, string> response)
																					foreach (var kv in response)
																						if (await new Naver.Search(key).BrowseTheSite(kv.Key) is Stack<string> stack)
																						{
																							var sb = new StringBuilder();

																							while (stack.TryPop(out string text))
																								sb.Append(text).Append('\n');

																							keywords[kv.Key] = sb.Remove(sb.Length - 1, 1).ToString();
																						}
																				using var sw = new StreamWriter(Path.Combine(directory, "Codes.csv"), true);

																				if (pop.Split(' ').Length == 1)
																					sw.WriteLine(string.Concat(pop, '\t', "ncn"));
																			}
																		}
									json = string.IsNullOrEmpty(url.Json) ? string.Empty : url.Json;
								}
								if (keywords.Count > 0)
									WorkerProgress(theme, keywords, json);
							}
							catch (Exception ex)
							{
								Base.SendMessage(sender.GetType(), theme.Name, ex.StackTrace);
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

					case HttpStatusCode:
						foreach (var cn in list.OrderBy(o => Guid.NewGuid()))
						{
							if (Status is HttpStatusCode.OK && cn.MaturityMarketCap.Contains(Base.TransactionSuspension) is false && cn.MarginRate > 0)
								try
								{
									if (int.TryParse(cn.Code, out int _) && Refuse is false && (api.IsInsider is false || await api.GetResponseAsync(HttpUtility.UrlEncode(Repository.GetPath(cn, now))) is false) && await new Naver.Search(key).VisualizeTheResultsOfAnAnalysis(cn.Name) is (List<Catalog.KRX.Cloud>, Dictionary<string, string>) cloud)
									{
										switch (await new Advertise(cn, key).TransmitCollectedInformation(cloud.Item1, cloud.Item2))
										{
											case Response response when await api.PutContextAsync(new Response { Status = cn.Code, Post = response.Post, Url = response.Url }) is > 0 && api.IsInsider:
												Base.SendMessage(sender.GetType(), cn.Name, response.Post, response.Url);
												break;

											case int status:
												Refuse = status == 0x196;
												break;

											default:
												continue;
										}
										var index = 0;
										var tags = new Cloud[cloud.Item1.Count];
										var news = new News[cloud.Item2.Count];

										foreach (var kv in cloud.Item2)
											if (string.IsNullOrEmpty(kv.Key) is false && string.IsNullOrEmpty(kv.Value) is false)
												news[index++] = new News
												{
													Title = kv.Value,
													Link = kv.Key
												};
										index = 0;

										foreach (var tidy in cloud.Item1)
											if (tidy.Style.Split(';') is string[] str)
												tags[index++] = new Cloud
												{
													Anchor = tidy.Anchor,
													Frequency = tidy.Frequency,
													Text = tidy.Text,
													Transform = tidy.Transform,
													Size = str[0].Split(':')[^1].Trim(),
													Style = str[2].Split(':')[^1].Trim()
												};
										if (await api.PostContextAsync(new Response
										{
											Status = cn.Code,
											Url = Repository.GetPath(cn, now),
											Post = JsonConvert.SerializeObject(new { tags, news })

										}) is 0xC8)
											Base.SendMessage(sender.GetType(), cn.Name, JsonConvert.SerializeObject(new
											{
												tags,
												news
											},
											Formatting.Indented));
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
							if (now.Hour == 8 || Status is HttpStatusCode.Forbidden)
							{
								if (Status is HttpStatusCode.OK && (now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || Array.Exists(Base.Holidays, o => o.Equals(now.ToString(Base.DateFormat)))))
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
							foreach (var ch in list.OrderBy(o => Guid.NewGuid()))
							{
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
		void WorkerProgress<T>(T param, IEnumerable<KeyValuePair<string, string>> enumerable, string json) where T : class
		{
			string fn, content;

			switch (param)
			{
				case Catalog.Models.Theme theme:
					fn = theme.Index;
					content = theme.Name;
					break;

				default:
					return;
			}
			foreach (var kv in enumerable)
				using (var sw = new StreamWriter(Path.Combine(Repository.R, string.Concat(fn, ".txt")), true))
					sw.WriteLine(kv.Value);

			new Task(async () =>
			{
				try
				{
					using (var process = new Process
					{
						StartInfo = new ProcessStartInfo
						{
							FileName = "cmd",
							UseShellExecute = false,
							RedirectStandardError = true,
							RedirectStandardInput = true,
							RedirectStandardOutput = true,
							WorkingDirectory = directory
						}
					})
					{
						process.ErrorDataReceived += SortOutputHandler;

						if (process.Start())
						{
							process.BeginErrorReadLine();
							process.StandardInput.WriteLine(string.Concat(script, tags, ' ', fn, ' ', nameof(Codes)));
							process.StandardInput.Close();
							process.WaitForExit();
						}
						process.ErrorDataReceived -= SortOutputHandler;
					}
					var dictionary = string.IsNullOrEmpty(json) || json.StartsWith("{\"http") ? new Dictionary<string, int>() : JsonConvert.DeserializeObject<Dictionary<string, int>>(json);

					if (dictionary.Count > 0)
					{
						foreach (var kv in dictionary.Where(o => o.Value < 2))
							if (dictionary.Remove(kv.Key))
								continue;

						foreach (var kv in dictionary)
							dictionary[kv.Key] = kv.Value - 1;
					}
					using (var process = new Process
					{
						StartInfo = new ProcessStartInfo
						{
							FileName = "cmd",
							UseShellExecute = false,
							RedirectStandardError = true,
							RedirectStandardInput = true,
							RedirectStandardOutput = true,
							WorkingDirectory = directory
						}
					})
					{
						var tags = Path.Combine(Repository.R, string.Concat(fn, ".csv"));
						using (var sr = new StreamReader(tags))
							while (sr.EndOfStream is false)
							{
								var line = sr.ReadLine();

								if (string.IsNullOrEmpty(line) is false)
								{
									var param = line.Split(',');

									if (int.TryParse(param[^1], out int num))
									{
										if (dictionary.TryGetValue(param[0], out int count))
											dictionary[param[0]] = count + num - 1;

										else
											dictionary[param[0]] = num;
									}
								}
							}
						int index = 0, division;
						var enumerable = dictionary.OrderByDescending(o => o.Value).Take(0x90);
						division = enumerable.Min(o => o.Value);
						process.ErrorDataReceived += SortOutputHandler;

						foreach (var kv in enumerable)
							if (kv.Value > 1 && index < 0x90 && division > 0)
							{
								using (var sw = new StreamWriter(tags, index > 0))
								{
									if (index++ == 0)
									{
										sw.WriteLine(string.Concat("word", ',', "freq"));
										dictionary.Clear();
									}
									sw.WriteLine(string.Concat(kv.Key, ',', kv.Value / division));
								}
								dictionary[kv.Key] = kv.Value / division;
							}
						if (await api.PutContextAsync(DateTime.Now, param, dictionary) is > 0 && process.Start())
						{
							process.BeginErrorReadLine();
							process.StandardInput.WriteLine(string.Concat(script, cloud, ' ', fn));
							process.StandardInput.Close();
							process.WaitForExit();
							var files = new[] { "png", html, "json" };

							for (index = 0; index < files.Length; index++)
							{
								if (index > 0)
								{

								}
								else
								{
									string name = string.Concat(fn, '.', files[index]), info = Path.Combine(directory, name);

									if (await server.PostContextAsync(new Files
									{
										ID = index == 0 ? null : string.Empty,
										Path = string.Concat(index == 0 ? Path.Combine(directory[0..^2], @"server\wwwroot\Images") : directory, '\\'),
										Name = name,
										LastWriteTime = new FileInfo(info).LastWriteTime,
										Contents = File.ReadAllBytes(info)

									}) is 0xC8)
										Base.SendMessage(param.GetType(), content);
								}
							}
						}
						process.ErrorDataReceived -= SortOutputHandler;
					}
					foreach (var file in Directory.GetFiles(directory, string.Concat(fn, ".*"), SearchOption.TopDirectoryOnly))
						File.Delete(file);

					if (Path.Combine(directory, string.Concat(fn, '_', "files")) is string path && new DirectoryInfo(path).Exists)
						Directory.Delete(path, true);
				}
				catch (Exception ex)
				{
					Base.SendMessage(param.GetType(), content, ex.StackTrace);
				}
			}).Start();
		}
		void SortOutputHandler(object sender, DataReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Data) is false && e.Data[0] is not '[')
				try
				{
					using var sw = new StreamWriter(log, true);
					sw.WriteLine(e.Data);
				}
				catch (Exception ex)
				{
					Base.SendMessage(sender.GetType(), e.Data, ex.StackTrace);
				}
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

				if (keywords is BackgroundWorker)
					keywords.Dispose();

				Dispose();
			}
		}
		HttpStatusCode Status => new Maturity(key).GetContext();
		bool Refuse
		{
			get; set;
		}
		string Message
		{
			get; set;
		}
		const string html = "html";
		const string tags = "Tags.R";
		const string cloud = "Cloud.R";
		const string initialize = "initialize.R";
		const string directory = @"C:\Algorithmic Trading\Res\R";
		readonly string log;
		readonly string key;
		readonly string script;
		readonly API api;
		readonly Pipe pipe;
		readonly Random random;
		readonly GoblinBat server;
		readonly BackgroundWorker big;
		readonly BackgroundWorker theme;
		readonly BackgroundWorker search;
		readonly BackgroundWorker keywords;
		readonly Icon[] icon;
	}
}