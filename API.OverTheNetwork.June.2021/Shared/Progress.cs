using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading.Tasks;

using ShareInvest.Catalog.Models;
using ShareInvest.Client;
using ShareInvest.Interface;

namespace ShareInvest
{
	public static class Progress
	{
		public static GoblinBat Client
		{
			get; set;
		}
		public static Client.Consensus Consensus
		{
			get; private set;
		}
		public static Dictionary<string, Analysis> Collection
		{
			get; private set;
		}
		public static Dictionary<string, IStrategics> Library
		{
			get; private set;
		}
		public static Dictionary<string, (Stack<double>, Stack<double>, Stack<double>)> Storage
		{
			get; private set;
		}
		public static Privacies Key
		{
			get; set;
		}
		public static char Company
		{
			get; private set;
		}
		public static string Account
		{
			internal get; set;
		}
		public static string Access
		{
			private get; set;
		}
		public static async Task<bool> GetUpdateVisionAsync()
		{
			var path = @"C:\Algorithmic Trading\Res\Update\";
			var file = new string[] { "x64.zip", "x86.zip" };
			var exist = false;

			foreach (var name in file)
			{
				var full = string.Concat(path, name);

				if (await Client.PostConfirmAsync(new Files
				{
					Path = path,
					Name = name,
					LastWriteTime = new FileInfo(full).LastWriteTime.AddHours(1),
					Contents = null
				}) is Files neo)
				{
					using (var stream = new FileStream(full, FileMode.Create))
						await stream.WriteAsync(neo.Contents.AsMemory(0, neo.Contents.Length));

					exist = exist || neo.Contents is not null && neo.Contents.Length > 0;
				}
			}
			return exist;
		}
		[SupportedOSPlatform("windows")]
		public static void TrendsToValuation(dynamic privacy) => new Task(async () =>
		{
			if (privacy is Privacies p && await Client.GetContextAsync(new Codes(), 6) is List<Codes> list)
				foreach (Catalog.TrendsToCashflow analysis in await Client.GetContextAsync(new Catalog.TrendsToCashflow()) as IEnumerable<IStrategics>)
					foreach (var ch in list.FindAll(o => Library.Keys.ToArray().Contains(o.Code)).Union(list.OrderBy(o => Guid.NewGuid())))
						try
						{
							var now = DateTime.Now;

							if (string.IsNullOrEmpty(ch.Price) == false && (ch.MarginRate == 1 || ch.MarginRate == 2)
								&& ch.MaturityMarketCap.StartsWith("증거금") && ch.MaturityMarketCap.Contains(Base.TransactionSuspension) == false
									&& await Client.PostConfirmAsync(new Catalog.ConfirmStrategics
									{
										Code = ch.Code,
										Date = now.Hour > 0xF ? now.ToString(Base.DateFormat) : now.AddDays(-1).ToString(Base.DateFormat),
										Strategics = string.Concat("TC.", analysis.AnalysisType)
									}) is bool confirm && (confirm == false || Storage.ContainsKey(ch.Code) == false))
							{
								var estimate = Strategics.AnalyzeFinancialStatements(await Client.GetContextAsync(new Catalog.FinancialStatement
								{
									Code = ch.Code
								}) as List<Catalog.FinancialStatement>, analysis.AnalysisType.ToCharArray());
								var cf = new Indicators.TrendsToCashflow
								{
									Code = ch.Code,
									Strategics = Library.TryGetValue(ch.Code, out IStrategics st) && st is Catalog.SatisfyConditionsAccordingToTrends sc ? new Catalog.TrendsToCashflow
									{
										Code = sc.Code,
										Short = sc.Short,
										Long = sc.Long,
										Trend = sc.Trend,
										Unit = 1,
										ReservationQuantity = 0,
										ReservationRevenue = 0xA,
										Addition = 0xB,
										Interval = 1,
										TradingQuantity = 1,
										PositionRevenue = 5.25e-3,
										PositionAddition = 7.25e-3,
										AnalysisType = analysis.AnalysisType
									} : analysis,
									Balance = new Balance
									{
										Market = ch.MarginRate == 1,
										Name = ch.Name,
										Purchase = 0,
										Quantity = 0,
										Rate = 0,
										Revenue = 0
									}
								};
								var bring = new BringInInformation(ch);
								bring.Send += cf.OnReceiveDrawChart;
								cf.StartProgress(p.Commission);
								var name = await bring.StartProgress();
								var statistics = cf.SendMessage;

								if (Storage.ContainsKey(ch.Code) == false)
								{
									Storage[ch.Code] = cf.ReturnTheUsedStack;
									Base.SendMessage(name, Storage.Count, typeof(BringInInformation));
								}
								if (estimate != null && estimate.Count > 3 && string.IsNullOrEmpty(statistics.Key) == false)
								{
									var normalize = estimate.Last(o => o.Key.ToString(Base.FullDateFormat).StartsWith(statistics.Date)).Value;
									var near = Base.FindTheNearestQuarter(DateTime.TryParseExact(statistics.Date, Base.FullDateFormat.Substring(0, 6), CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date) ? date : DateTime.Now);

									if (await Client.PutContextAsync(new Catalog.Models.Consensus
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
									}) is int status && status == 0xC8 && statistics.Base > 0 && await Client.PutContextAsync(new StocksStrategics
									{
										Code = ch.Code,
										Strategics = statistics.Key,
										Date = statistics.Date,
										MaximumInvestment = (long)statistics.Base,
										CumulativeReturn = statistics.Cumulative / statistics.Base,
										WeightedAverageDailyReturn = statistics.Statistic / statistics.Base,
										DiscrepancyRateFromExpectedStockPrice = statistics.Price
									}) is double coin && double.IsNaN(coin))
										Base.SendMessage(ch.Code, typeof(StocksStrategics));
								}
							}
						}
						catch (Exception ex)
						{
							Base.SendMessage(ex.StackTrace, typeof(Progress));
						}
		}).Start();
		public static void SetPrivacy(Privacies privacy)
		{
			Collection = new Dictionary<string, Analysis>();
			Company = char.TryParse(privacy.SecuritiesAPI, out char company) ? company : char.MaxValue;
			Consensus = new Client.Consensus(privacy.Security);
			Library = new Dictionary<string, IStrategics>();
			Storage = new Dictionary<string, (Stack<double>, Stack<double>, Stack<double>)>();
			Key = privacy;

			if (string.IsNullOrEmpty(privacy.CodeStrategics) == false && privacy.CodeStrategics[2] is '|')
				foreach (var strategics in Strategics.SetStrategics(privacy.CodeStrategics.Split(';')))
					Library[strategics.Code] = strategics;
		}
		[SupportedOSPlatform("windows")]
		public static void TryToConnectThePipeStream(dynamic name)
		{
			var server
				= new NamedPipeServerStream(name is string ? name : Process.GetCurrentProcess().ProcessName, PipeDirection.Out, 0x11, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
			var client = new NamedPipeClientStream(".", Access, PipeDirection.In, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
			new Task(async () =>
			{
				await server.WaitForConnectionAsync();
				Pipe.TellTheClientConnectionStatus(server.GetType().Name, server.IsConnected);

				if (server.IsConnected)
				{
					Pipe.Server = new StreamWriter(server)
					{
						AutoFlush = true
					};
					Pipe.Server.WriteLine("{0}API Connects via Pipe. . .", Company is 'O' ? "Open" : "Xing");
				}
			}).Start();
			new Task(async () =>
			{
				await client.ConnectAsync();
				Pipe.TellTheClientConnectionStatus(client.GetType().Name, client.IsConnected);

				if (client.IsConnected)
					Pipe.OnReceivePipeClientMessage(client, server);

			}).Start();
		}
	}
}