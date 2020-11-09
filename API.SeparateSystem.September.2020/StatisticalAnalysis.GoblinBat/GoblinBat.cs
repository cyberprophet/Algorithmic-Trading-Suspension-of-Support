using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.Analysis.SecondaryIndicators;
using ShareInvest.Catalog.Request;
using ShareInvest.Client;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Message;

namespace ShareInvest.Strategics
{
    public sealed partial class GoblinBat : Form
    {
        public GoblinBat(dynamic cookie)
        {
            this.cookie = cookie;
            InitializeComponent();
            random = new Random();
            client = GoblinBatClient.GetInstance(cookie);
            strip.ItemClicked += OnItemClick;
            StartProgress(new Catalog.Privacies { Security = cookie });
        }
        bool IsTheCorrectInformation(Catalog.Codes param)
        {
            switch (param.Code.Length)
            {
                case int length when length == 6 && int.TryParse(param.Price, out int price) && param.MaturityMarketCap.StartsWith("증거금"):
                    return (param.MarginRate == 1 || param.MarginRate == 2) && price > 0 && (param.MaturityMarketCap.Contains("거래정지") || string.IsNullOrEmpty(param.Price)) == false;

                default:
                    return false;
            }
        }
        void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Statistical.SetProgressRate(Color.Maroon);
                SendMessage(e.Error.StackTrace);
            }
            else if (e.Cancelled)
            {
                Statistical.SetProgressRate(e.Cancelled);
                Statistical.SetProgressRate(Color.Ivory);
            }
            else
            {
                Statistical.SetProgressRate((bool)e.Result);
                Statistical.SetProgressRate(Color.Ivory);
            }
        }
        void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e) => Statistical.SetProgressRate(e.ProgressPercentage);
        void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var list = client.GetContext(new Catalog.Codes(), 6).Result as List<Catalog.Codes>;
            var stack = new Stack<IStrategics>();

            foreach (var strategics in new IStrategics[]
            {
                new Catalog.TrendsInStockPrices(),
                new Catalog.ScenarioAccordingToTrend(),
                new Catalog.TrendToCashflow()
            })
                foreach (var enumerable in client.GetContext(strategics).Result)
                    stack.Push(enumerable);

            ulong maximum = (ulong)(list.Count * stack.Count), rate = 0;

            while (stack.Count > 0)
                try
                {
                    if (backgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        SendMessage(string.Concat(rate, '/', maximum));

                        break;
                    }
                    else
                    {
                        var strategics = stack.Pop();

                        foreach (var w in list.OrderBy(o => Guid.NewGuid()))
                        {
                            if (backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                SendMessage((rate / (double)maximum).ToString("P5"));

                                break;
                            }
                            else if (IsTheCorrectInformation(w))
                            {
                                var now = DateTime.Now;
                                HoldingStocks hs = null;

                                switch (strategics)
                                {
                                    case Catalog.ScenarioAccordingToTrend st:
                                        var consensus = client.GetContext(new Catalog.ConvertConsensus { Code = w.Code }).Result;

                                        if (consensus != null && consensus.Any(o => o.Date.EndsWith("(E)") && o.Date.StartsWith(now.AddYears(2).ToString("yy"))) && client.PostContext(new ConfirmStrategics
                                        {
                                            Code = w.Code,
                                            Date = now.Hour > 0xF ? now.ToString(format) : now.AddDays(-1).ToString(format),
                                            Strategics = string.Concat("ST.", st.Calendar, '.', st.Trend, '.', st.CheckSales.ToString().Substring(0, 1), '.', (uint)(st.Sales * 0x64), '.', st.CheckOperatingProfit.ToString().Substring(0, 1), '.', (uint)(st.OperatingProfit * 0x64), '.', st.CheckNetIncome.ToString().Substring(0, 1), '.', (uint)(st.NetIncome * 0x64))
                                        }).Result == false)
                                        {
                                            st.Code = w.Code;
                                            hs = new HoldingStocks(st, new Catalog.ConvertConsensus().PresumeToConsensus(consensus), client)
                                            {
                                                Code = st.Code
                                            };
                                            hs.SendBalance += OnReceiveAnalysisData;
                                            hs.StartProgress(0D);
                                        }
                                        break;

                                    case Catalog.TrendToCashflow tc:
                                        if (client.PostContext(new ConfirmStrategics
                                        {
                                            Code = w.Code,
                                            Date = now.Hour > 0xF ? now.ToString(format) : now.AddDays(-1).ToString(format),
                                            Strategics = string.Concat("TC.", tc.AnalysisType)
                                        }).Result == false)
                                        {
                                            tc.Code = w.Code;
                                            hs = new HoldingStocks(tc, client)
                                            {
                                                Code = tc.Code
                                            };
                                            hs.SendBalance += OnReceiveAnalysisData;
                                            hs.StartProgress(0D);
                                        }
                                        break;

                                    case Catalog.TrendsInStockPrices ts:
                                        if (client.PostContext(new ConfirmStrategics
                                        {
                                            Code = w.Code,
                                            Date = now.Hour > 0xF ? now.ToString(format) : now.AddDays(-1).ToString(format),
                                            Strategics = string.Concat("TS.", ts.Short, '.', ts.Long, '.', ts.Trend, '.', (int)(ts.RealizeProfit * 0x2710), '.', (int)(ts.AdditionalPurchase * 0x2710), '.', ts.QuoteUnit, '.', (char)ts.LongShort, '.', (char)ts.TrendType, '.', (char)ts.Setting)
                                        }).Result == false)
                                        {
                                            ts.Code = w.Code;
                                            hs = new HoldingStocks(ts)
                                            {
                                                Code = ts.Code
                                            };
                                            hs.SendBalance += OnReceiveAnalysisData;
                                            hs.StartProgress(0D);
                                        }
                                        break;
                                }
                                if (hs != null)
                                    hs.SendBalance -= OnReceiveAnalysisData;
                            }
                            Statistical.SetProgressRate(Color.Gold);
                            backgroundWorker.ReportProgress((int)(rate++ * 1e+2 / maximum));
                        }
                    }
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                    SendMessage(ex.StackTrace);

                    break;
                }
            e.Result = rate == maximum || rate - 1 == maximum;
        }
        [Conditional("DEBUG")]
        void SendMessage(string message) => Console.WriteLine(message);
        async void OnReceiveAnalysisData(object sender, SendSecuritiesAPI e)
        {
            if (e.Convey is Tuple<dynamic, Catalog.Statistics> tuple)
            {
                var coin = double.NaN;

                if (string.IsNullOrEmpty(tuple.Item2.Key) == false)
                    switch (tuple.Item1)
                    {
                        case Catalog.TrendFollowingBasicFutures tf:
                            break;

                        case Catalog.TrendsInStockPrices ts:
                            if (tuple.Item2.Base > 0 && (ts.Setting.Equals(Setting.Both) || ts.Setting.Equals(Setting.Reservation)))
                                coin = await client.PutContext(new StocksStrategics
                                {
                                    Code = ts.Code,
                                    Strategics = tuple.Item2.Key,
                                    Date = tuple.Item2.Date,
                                    MaximumInvestment = (long)tuple.Item2.Base,
                                    CumulativeReturn = tuple.Item2.Cumulative / tuple.Item2.Base,
                                    WeightedAverageDailyReturn = tuple.Item2.Statistic / tuple.Item2.Base
                                });
                            break;

                        case Catalog.ScenarioAccordingToTrend st:
                            if (tuple.Item2.Base > 0)
                                coin = await client.PutContext(new StocksStrategics
                                {
                                    Code = st.Code,
                                    Strategics = tuple.Item2.Key,
                                    Date = tuple.Item2.Date,
                                    MaximumInvestment = (long)tuple.Item2.Base,
                                    CumulativeReturn = tuple.Item2.Cumulative / tuple.Item2.Base,
                                    WeightedAverageDailyReturn = tuple.Item2.Statistic / tuple.Item2.Base,
                                    DiscrepancyRateFromExpectedStockPrice = tuple.Item2.Price
                                });
                            break;

                        case Catalog.TrendToCashflow tc:
                            if (tuple.Item2.Base > 0)
                                coin = await client.PutContext(new StocksStrategics
                                {
                                    Code = tc.Code,
                                    Strategics = tuple.Item2.Key,
                                    Date = tuple.Item2.Date,
                                    MaximumInvestment = (long)tuple.Item2.Base,
                                    CumulativeReturn = tuple.Item2.Cumulative / tuple.Item2.Base,
                                    WeightedAverageDailyReturn = tuple.Item2.Statistic / tuple.Item2.Base,
                                    DiscrepancyRateFromExpectedStockPrice = tuple.Item2.Price
                                });
                            break;
                    }
                if (double.IsNaN(coin) == false)
                {
                    if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday) && DateTime.Now.Hour < 3 && (cookie as string).Equals(admin) == false)
                    {
                        ClosingForm = true;
                        backgroundWorker.CancelAsync();

                        if (WindowState.Equals(FormWindowState.Minimized))
                            WindowState = FormWindowState.Normal;
                    }
                    var remain = await client.PutContext(new Catalog.Privacies
                    {
                        Security = Privacy.Security,
                        SecuritiesAPI = Privacy.SecuritiesAPI,
                        SecurityAPI = Privacy.SecurityAPI,
                        Account = Privacy.Account,
                        Commission = Privacy.Commission,
                        CodeStrategics = Privacy.CodeStrategics,
                        Coin = coin + GoblinBatClient.Coin
                    });
                    if (remain < 0)
                    {
                        if (ChooseBox.Show(bill, money, charge, fExit).Equals(DialogResult.Yes))
                        {

                        }
                        else
                        {
                            Thread.Sleep((int)Math.Pow(await client.DeleteContext<Catalog.Privacies>(Privacy), charge.Length));
                            ClosingForm = true;
                            strip.ItemClicked -= OnItemClick;
                            Dispose();
                        }
                    }
                    else
                        notifyIcon.Text = ConvertTheFare(remain);
                }
                else
                    SendMessage((tuple.Item1 as IStrategics).Code);
            }
            else if (e.Convey is Tuple<dynamic, double, uint> strategics)
                switch (strategics.Item1)
                {
                    case Catalog.TrendsInStockPrices ts:
                        Statistical.SetDataGridView(ts, strategics.Item3, strategics.Item2);
                        return;
                }
        }
        void OnReceiveTheChangedSize(object sender, SendHoldingStocks e)
        {
            if (sender is Controls.Strategics)
            {
                if (e.Strategics != null)
                {
                    Form form = null;
                    HoldingStocks hs = null;

                    switch (e.Strategics)
                    {
                        case Catalog.TrendsInStockPrices ts when string.IsNullOrEmpty(e.Code) == false && uint.TryParse(e.Code, out uint price):
                            hs = new HoldingStocks(ts)
                            {
                                Code = ts.Code
                            };
                            hs.SendBalance += OnReceiveAnalysisData;
                            hs.StartProgress(price);
                            return;

                        case Catalog.Privacies privacies:
                            new SatisfyConditionsAccordingToTrends(privacies, client).PerformClick().ShowDialog();
                            return;

                        case Size size:
                            var height = 0x2DC;

                            switch (size.Height)
                            {
                                case 0xCD:
                                    height -= 0x1E;
                                    break;

                                case 0x145:
                                    height += 0x59;
                                    break;

                                case 0x145 - 0x23:
                                    height += 0x60;
                                    break;
                            }
                            Size = new Size(0x2B9, height);
                            return;

                        case Tuple<int, Catalog.Privacies> tuple when tuple.Item2 is Catalog.Privacies privacy && (string.IsNullOrEmpty(privacy.Account) || string.IsNullOrEmpty(privacy.SecuritiesAPI) || string.IsNullOrEmpty(privacy.SecurityAPI)) == false:
                            if (tuple.Item1 == 0)
                                backgroundWorker.RunWorkerAsync();

                            else if (tuple.Item1 > 0)
                                backgroundWorker.CancelAsync();

                            return;

                        case Tuple<Tuple<List<Catalog.ConvertConsensus>, List<Catalog.ConvertConsensus>>, Catalog.ScenarioAccordingToTrend> consensus:
                            hs = new HoldingStocks(consensus.Item2, consensus.Item1, client)
                            {
                                Code = consensus.Item2.Code
                            };
                            form = new ScenarioAccordingToTrend(hs);
                            hs.SendBalance += OnReceiveAnalysisData;
                            new Task(() => hs.StartProgress(Privacy.Commission)).Start();
                            break;

                        case Catalog.TrendFollowingBasicFutures tf:
                            hs = new HoldingStocks(tf)
                            {
                                Code = tf.Code
                            };
                            form = new TrendFollowingBasicFutures(hs);
                            hs.SendBalance += OnReceiveAnalysisData;
                            new Task(() => hs.StartProgress(Privacy.Commission)).Start();
                            break;

                        case Catalog.TrendToCashflow tc:
                            hs = new HoldingStocks(tc, client)
                            {
                                Code = tc.Code
                            };
                            form = new TrendsInStockPrices(hs);
                            hs.SendBalance += OnReceiveAnalysisData;
                            new Task(() => hs.StartProgress(Privacy.Commission)).Start();
                            break;

                        case Catalog.TrendsInValuation tv:
                            hs = new HoldingStocks(tv, client)
                            {
                                Code = tv.Code
                            };
                            form = new TrendsInStockPrices(hs);
                            hs.SendBalance += OnReceiveAnalysisData;
                            new Task(() => hs.StartProgress(Privacy.Commission)).Start();
                            break;

                        case Catalog.TrendsInStockPrices ts:
                            hs = new HoldingStocks(ts)
                            {
                                Code = ts.Code
                            };
                            form = new TrendsInStockPrices(hs);
                            hs.SendBalance += OnReceiveAnalysisData;
                            new Task(() => hs.StartProgress(Privacy.Commission)).Start();
                            break;
                    }
                    Cursor = Cursors.AppStarting;
                    WindowState = FormWindowState.Minimized;

                    if (form.ShowDialog().Equals(DialogResult.Cancel))
                    {
                        hs.SendBalance -= OnReceiveAnalysisData;
                        Cursor = Cursors.Default;
                        IAsyncResult result = null;

                        switch (e.Strategics)
                        {
                            case Catalog.TrendToCashflow tc:
                                result = Statistical.SetProgressRate(new Catalog.Request.Consensus { Strategics = string.Concat("TC.", tc.AnalysisType) });
                                break;

                            case Catalog.TrendsInValuation tv:
                                result = Statistical.SetProgressRate(new Catalog.Request.Consensus { Strategics = string.Concat("TC.", tv.AnalysisType) });
                                break;

                            case Catalog.TrendFollowingBasicFutures tf:
                                result = Statistical.SetProgressRate(DateTime.Now);
                                break;
                        }
                        if (result != null && result.AsyncWaitHandle.WaitOne(0xED3))
                        {
                            result.AsyncWaitHandle.Close();
                            result.AsyncWaitHandle.Dispose();
                            GC.Collect();
                        }
                        strip.Items.Find(st, false).First(o => o.Name.Equals(st)).PerformClick();
                    }
                }
                else if (string.IsNullOrEmpty(e.Code) == false && e.Color != null)
                    Statistical.SetDataGridView(e.Code, e.Color);
            }
        }
        async void StartProgress(IParameters param)
        {
            switch ((await client.PostContext<Catalog.Privacies>(param)).Item1)
            {
                case 0xCA:
                    if (Statistical == null)
                    {
                        Statistical = new Controls.Strategics(client, new Disclosure(cookie, 'D'));
                        Controls.Add(Statistical);
                        Statistical.Dock = DockStyle.Fill;
                        Statistical.SendSize += OnReceiveTheChangedSize;
                    }
                    Result = DialogResult.Yes;
                    break;

                case 0xC8:
                    Process.Start(@"http://www.pgsolution.kr/");
                    Result = ChooseBox.Show(string.Concat(welcomeTo, (1 + await client.GetContext()).ToString("N0"), theGoblinBat), welcome, agree, fExit);
                    break;

                default:
                    Result = DialogResult.No;
                    break;
            }
            if (Result.Equals(DialogResult.Yes) && IsApplicationAlreadyRunning(param.Security))
            {
                Privacy = new Catalog.Privacies
                {
                    Security = param.Security
                };
                Opacity = 0;
                timer.Start();
            }
            else if (Result.Equals(DialogResult.No))
            {
                strip.ItemClicked -= OnItemClick;
                Dispose();
            }
            else
            {
                Opacity = 0;
                notifyIcon.Text = text;
                WindowState = FormWindowState.Minimized;
            }
        }
        void GoblinBatResize(object sender, EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                if (string.IsNullOrEmpty(OnClickMinimized) == false && OnClickMinimized.Equals(st))
                    Statistical.Hide();

                Opacity = 0.8135;
                BackColor = Color.FromArgb(0x79, 0x85, 0x82);
                Visible = false;
                ShowIcon = false;
                ClosingForm = false;
                notifyIcon.Visible = true;
            }
        }
        void GoblinBatFormClosing(object sender, FormClosingEventArgs e)
        {
            GetSettleTheFare();

            if (MessageBox.Show(notifyIcon.Text.Equals(text) ? nExit : rExit, notifyIcon.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning).Equals(DialogResult.Cancel))
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                Application.DoEvents();

                return;
            }
            ClosingForm = true;
            strip.ItemClicked -= OnItemClick;
            Dispose();
        }
        void TimerTick(object sender, EventArgs e)
        {
            if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) == false && Result.Equals(DialogResult.Yes))
            {
                FormBorderStyle = FormBorderStyle.FixedSingle;
                WindowState = FormWindowState.Minimized;
            }
            else if (DateTime.Now.Hour < 3 && backgroundWorker.IsBusy == false && DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday) && (cookie as string).Equals(admin) == false)
            {
                timer.Stop();
                strip.ItemClicked -= OnItemClick;
                GetSettleTheFare();
                Dispose();
            }
            else if (Visible == false && ShowIcon == false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
            {
                notifyIcon.Icon = (Icon)resources.GetObject(Change ? upload : download);
                Change = !Change;

                if (IsApplicationAlreadyRunning(Privacy.Security) == false && backgroundWorker.IsBusy == false && string.IsNullOrEmpty(Privacy.Account) && string.IsNullOrEmpty(Privacy.SecuritiesAPI) && string.IsNullOrEmpty(Privacy.SecurityAPI))
                    strip.Items.Find(st, false).First(o => o.Name.Equals(st)).PerformClick();
            }
            else
                Statistical.CheckForSurvival(colors[DateTime.Now.Second % 3]);
        }
        void OnItemClick(object sender, ToolStripItemClickedEventArgs e) => BeginInvoke(new Action(async () =>
        {
            if (e.ClickedItem.Name.Equals(st) && notifyIcon.Text.Equals(text) == false)
            {
                if (Statistical == null)
                {
                    Statistical = new Controls.Strategics(client, new Disclosure(cookie, 'D'));
                    Controls.Add(Statistical);
                    Statistical.Dock = DockStyle.Fill;
                }
                if (Statistical.Controls.Find("tab", true).First().Controls.Count == 0 && await client.GetContext<Catalog.Privacies>(Privacy) is Catalog.Privacies privacy && privacy.Coin > 0)
                {
                    Privacy = privacy;
                    Text = await Statistical.SetPrivacy(privacy);
                    notifyIcon.Text = ConvertTheFare(privacy.Coin);

                    if (backgroundWorker.IsBusy == false && string.IsNullOrEmpty(Privacy.Account) == false && string.IsNullOrEmpty(Privacy.SecuritiesAPI) == false && string.IsNullOrEmpty(Privacy.SecurityAPI) == false)
                    {
                        backgroundWorker.RunWorkerAsync();
                        Statistical.SetProgressRate();
                    }
                    ClosingForm = true;
                }
                Size = new Size(0x2B9, 0x329 - 0xF4);
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                Statistical.Show();
                Statistical.SetProgressRate(false);
                WindowState = FormWindowState.Normal;
            }
            else
                Close();

            OnClickMinimized = e.ClickedItem.Name;
        }));
        bool Change
        {
            get; set;
        }
        bool ClosingForm
        {
            get; set;
        }
        string OnClickMinimized
        {
            get; set;
        }
        DialogResult Result
        {
            get; set;
        }
        Controls.Strategics Statistical
        {
            get; set;
        }
        Catalog.Privacies Privacy
        {
            get; set;
        }
        readonly dynamic cookie;
        readonly Random random;
        readonly GoblinBatClient client;
    }
}