using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.Analysis.SecondaryIndicators;
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
            cultureInfo = CultureInfo.GetCultureInfo("en-US");
            client = GoblinBatClient.GetInstance(cookie);
            strip.ItemClicked += OnItemClick;
            StartProgress(new Catalog.Privacies { Security = cookie });
        }
        void OnReceiveAnalysisData(object sender, SendSecuritiesAPI e)
        {
            if (e.Convey is Tuple<dynamic, Catalog.Statistics> tuple)
                switch (tuple.Item1)
                {
                    case Catalog.TrendFollowingBasicFutures tf:
                        break;

                    case Catalog.TrendsInStockPrices ts:
                        Console.WriteLine(ts.Code + "\t" + tuple.Item2.Date + "\t" + (tuple.Item2.Cumulative / tuple.Item2.Base).ToString("P2") + "\t" + (tuple.Item2.Statistic / tuple.Item2.Base).ToString("P2"));
                        break;
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
                WindowState = FormWindowState.Minimized;

                if (form.ShowDialog().Equals(DialogResult.Cancel))
                {
                    hs.SendBalance -= OnReceiveAnalysisData;
                    strip.Items.Find(st, false).First(o => o.Name.Equals(st)).PerformClick();
                }
            }
        }
        async void StartProgress(IParameters param)
        {
            switch ((int)await client.PostContext<Catalog.Privacies>(param))
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
                {

                }
            }
            notifyIcon.Text = GoblinBatClient.Coin.ToString("C0", cultureInfo);
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
                if (Statistical.Controls.Find("tab", true).First().Controls.Count == 0 && await client.GetContext<Catalog.Privacies>(Privacy) is Catalog.Privacies privacy)
                    Text = await Statistical.SetPrivacy(privacy);

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
        readonly GoblinBatClient client;
        readonly CultureInfo cultureInfo;
    }
}