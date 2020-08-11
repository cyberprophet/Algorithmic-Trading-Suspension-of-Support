using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.Analysis.SecondaryIndicators;
using ShareInvest.Catalog.Request;
using ShareInvest.Client;
using ShareInvest.EventHandler;
using ShareInvest.Interface;

namespace ShareInvest.Strategics
{
    public sealed partial class GoblinBat : Form
    {
        public GoblinBat(dynamic cookie)
        {
            InitializeComponent();
            random = new Random();
            cultureInfo = CultureInfo.GetCultureInfo("en-US");
            client = GoblinBatClient.GetInstance(cookie);
            strip.ItemClicked += OnItemClick;
            StartProgress(new Catalog.Privacies { Security = cookie });
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
                        {
                            coin = await client.PutContext(new StocksStrategics
                            {
                                Code = ts.Code,
                                Strategics = string.Concat("TS.", ts.Short, '.', ts.Long, '.', ts.Trend, '.', (int)(ts.RealizeProfit * 0x2710), '.', (int)(ts.AdditionalPurchase * 0x2710), '.', ts.QuoteUnit, '.', (char)ts.LongShort, '.', (char)ts.TrendType, '.', (char)ts.Setting),
                                Date = tuple.Item2.Date,
                                MaximumInvestment = (long)tuple.Item2.Base,
                                CumulativeReturn = tuple.Item2.Cumulative / tuple.Item2.Base,
                                WeightedAverageDailyReturn = tuple.Item2.Statistic / tuple.Item2.Base
                            });
                        }
                        break;
                }
                if (double.IsNaN(coin) == false)
                {
                    var remain = await client.PutContext(new Catalog.Privacies
                    {
                        Security = Privacy.Security,
                        SecuritiesAPI = Privacy.SecuritiesAPI,
                        SecurityAPI = Privacy.SecurityAPI,
                        Account = Privacy.Account,
                        Commission = Privacy.Commission,
                        CodeStrategics = Privacy.CodeStrategics,
                        Coin = coin - GoblinBatClient.Coin
                    });
                    if (remain < 0)
                    {
                        Console.WriteLine(remain);
                    }
                    else
                        notifyIcon.Text = remain.ToString("C0", cultureInfo);
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
                    Result = DialogResult.OK;
                    break;

                case 0xC8:
                    Result = MessageBox.Show("", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    break;

                default:
                    Result = DialogResult.Cancel;
                    break;
            }
            if (Result.Equals(DialogResult.OK) && IsApplicationAlreadyRunning(param.Security))
            {
                Privacy = new Catalog.Privacies
                {
                    Security = param.Security
                };
                Opacity = 0;
                timer.Start();
            }
            else if (Result.Equals(DialogResult.Cancel))
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
                {
                    Statistical.Hide();
                    timer.Start();
                }
                Opacity = 0.8135;
                BackColor = Color.FromArgb(0x79, 0x85, 0x82);
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;
            }
        }
        void GoblinBatFormClosing(object sender, FormClosingEventArgs e)
        {
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
            if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) == false && Result.Equals(DialogResult.OK))
            {
                FormBorderStyle = FormBorderStyle.FixedSingle;
                WindowState = FormWindowState.Minimized;
            }
            else if (Visible == false && ShowIcon == false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
            {
                notifyIcon.Icon = (Icon)resources.GetObject(Change ? upload : download);
                Change = !Change;

                if (IsApplicationAlreadyRunning(Privacy.Security))
                    notifyIcon.Text = Text;
            }
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
                    notifyIcon.Text = privacy.Coin.ToString("C0", cultureInfo);
                }
                Size = new Size(0x245, 0x208);
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                Statistical.Show();
                WindowState = FormWindowState.Normal;
                timer.Stop();
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
        readonly Random random;
        readonly GoblinBatClient client;
        readonly CultureInfo cultureInfo;
    }
}