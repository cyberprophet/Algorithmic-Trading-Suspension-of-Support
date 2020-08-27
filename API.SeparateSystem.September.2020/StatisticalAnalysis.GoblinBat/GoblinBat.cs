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
                Console.WriteLine(e.Error.StackTrace);
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
            Cancel.Dispose();
        }
        void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e) => Statistical.SetProgressRate(e.ProgressPercentage);
        void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var list = client.GetContext(new Catalog.Codes(), 6).Result as List<Catalog.Codes>;
            var stack = new Stack<IStrategics>();
            var array = new IStrategics[]
            {
                new Catalog.TrendsInStockPrices()
            };
            foreach (var strategics in array.OrderBy(o => random.Next(array.Length)))
                foreach (var enumerable in client.GetContext(strategics).Result)
                    stack.Push(enumerable);

            var maximum = list.Count * stack.Count;
            var rate = 0;
            var po = new ParallelOptions
            {
                CancellationToken = Cancel.Token,
                MaxDegreeOfParallelism = (int)(Environment.ProcessorCount * 0.25)
            };
            while (stack.Count > 0)
                try
                {
                    var strategics = stack.Pop();
                    Parallel.ForEach(list.OrderBy(o => Guid.NewGuid()), po, new Action<Catalog.Codes>((w) =>
                    {
                        if (backgroundWorker.CancellationPending)
                        {
                            e.Cancel = true;

                            if (Cancel.IsCancellationRequested)
                                po.CancellationToken.ThrowIfCancellationRequested();
                        }
                        else if (IsTheCorrectInformation(w))
                        {
                            var now = DateTime.Now;
                            HoldingStocks hs = null;

                            switch (strategics)
                            {
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
                        backgroundWorker.ReportProgress((int)(rate++ * 0x64 / (double)maximum));
                    }));
                }
                catch (OperationCanceledException ex)
                {
                    Statistical.SetProgressRate(Color.Ivory);
                    Console.WriteLine("Count_" + rate + "\t" + ex.TargetSite.Name);
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                    Console.WriteLine(ex.StackTrace);
                }
            e.Result = rate == maximum || rate - 1 == maximum;
        }
        async void OnReceiveAnalysisData(object sender, SendSecuritiesAPI e)
        {
            if (e.Convey is Tuple<dynamic, Catalog.Statistics> tuple)
            {
                var coin = double.NaN;

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
                }
                if (double.IsNaN(coin) == false)
                {
                    if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday) && DateTime.Now.Hour < 3)
                    {
                        ClosingForm = true;
                        Cancel.Cancel();
                        backgroundWorker.CancelAsync();
                        WindowState = FormWindowState.Minimized;
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
                    Console.WriteLine((tuple.Item1 as IStrategics).Code);
            }
        }
        void OnReceiveTheChangedSize(object sender, SendHoldingStocks e)
        {
            if (sender is Controls.Strategics)
            {
                Form form = null;
                HoldingStocks hs = null;

                switch (e.Strategics)
                {
                    case Size size:
                        SuspendLayout();
                        Console.WriteLine(Size.Height + "\t" + Size.Width + "\t" + size.Height + "\t" + size.Width);

                        ResumeLayout();
                        return;

                    case Tuple<int, Catalog.Privacies> tuple when tuple.Item2 is Catalog.Privacies privacy && (string.IsNullOrEmpty(privacy.Account) || string.IsNullOrEmpty(privacy.SecuritiesAPI) || string.IsNullOrEmpty(privacy.SecurityAPI)) == false:
                        if (tuple.Item1 == 0)
                        {
                            Cancel = new CancellationTokenSource();
                            backgroundWorker.RunWorkerAsync();
                        }
                        else if (tuple.Item1 > 0)
                        {
                            Cancel.Cancel();
                            backgroundWorker.CancelAsync();
                        }
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
                    strip.Items.Find(st, false).First(o => o.Name.Equals(st)).PerformClick();
                }
            }
        }
        async void StartProgress(IParameters param)
        {
            switch ((await client.PostContext<Catalog.Privacies>(param)).Item1)
            {
                case 0xCA:
                    if (Statistical == null)
                    {
                        Statistical = new Controls.Strategics(client);
                        Controls.Add(Statistical);
                        Statistical.Dock = DockStyle.Fill;
                        Statistical.SendSize += OnReceiveTheChangedSize;
                    }
                    Result = DialogResult.Yes;
                    break;

                case 0xC8:
                    foreach (var url in termsOfUse)
                        Process.Start(url);

                    Result = ChooseBox.Show(string.Concat(welcomeTo, (await client.GetContext()).ToString("N0"), theGoblinBat), welcome, agree, fExit);
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
            else if (DateTime.Now.Hour < 3 && backgroundWorker.IsBusy == false && DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                timer.Stop();
                strip.ItemClicked -= OnItemClick;
                Dispose();
            }
            else if (Visible == false && ShowIcon == false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
            {
                notifyIcon.Icon = (Icon)resources.GetObject(Change ? upload : download);
                Change = !Change;

                if (IsApplicationAlreadyRunning(Privacy.Security) && backgroundWorker.IsBusy == false && (string.IsNullOrEmpty(Privacy.Account) || string.IsNullOrEmpty(Privacy.SecuritiesAPI) || string.IsNullOrEmpty(Privacy.SecurityAPI)) == false)
                {
                    Cancel = new CancellationTokenSource();
                    GetSettleTheFare();
                    backgroundWorker.RunWorkerAsync();
                    Statistical.SetProgressRate();
                }
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
                    Statistical = new Controls.Strategics(client);
                    Controls.Add(Statistical);
                    Statistical.Dock = DockStyle.Fill;
                }
                if (Statistical.Controls.Find("tab", true).First().Controls.Count == 0 && await client.GetContext<Catalog.Privacies>(Privacy) is Catalog.Privacies privacy && privacy.Coin > 0)
                {
                    Privacy = privacy;
                    Text = await Statistical.SetPrivacy(privacy);
                    notifyIcon.Text = ConvertTheFare(privacy.Coin);

                    if (backgroundWorker.IsBusy == false)
                    {
                        Cancel = new CancellationTokenSource();
                        backgroundWorker.RunWorkerAsync();
                        Statistical.SetProgressRate();
                    }
                    ClosingForm = true;
                }
                Size = new Size(0x245, 0x208);
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                Statistical.Show();
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
        CancellationTokenSource Cancel
        {
            get; set;
        }
        readonly dynamic cookie;
        readonly Random random;
        readonly GoblinBatClient client;
    }
}