using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
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
			}
			else if (string.IsNullOrEmpty(pipe.Message))
			{
				BeginInvoke(new Action(async () => worker.RunWorkerAsync(await api.GetContextAsync(new Catalog.TrendsToCashflow()))));
				pipe.StartProgress();
			}
			else if (Visible is false && ShowIcon is false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
			{
				if (string.IsNullOrEmpty(Message))
					Message = pipe.Message;

				else
				{
					notifyIcon.Icon = Message.EndsWith(false.ToString()) ? icon[^1] : icon[DateTime.Now.Second % 2];
					notifyIcon.Text = Message;
				}
			}
		}
		async void WorkerDoWork(object sender, DoWorkEventArgs e)
		{
			if (e.Argument is IEnumerable<Interface.IStrategics> enumerable && await api.GetContextAsync(new Codes(), 6) is List<Codes> list)
				foreach (Catalog.TrendsToCashflow analysis in enumerable)
					foreach (var ch in list.OrderBy(o => Guid.NewGuid()))
						try
						{
							var now = DateTime.Now;

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
								var name = await bring.StartProgress();
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
							Base.SendMessage(sender.GetType(), ex.StackTrace);
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
				Dispose();
			}
		}
		readonly string key;
		readonly API api;
		readonly Pipe pipe;
		readonly Icon[] icon;
	}
}