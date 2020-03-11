using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;
using ShareInvest.GoblinBatControls;
using ShareInvest.Message;
using ShareInvest.Strategy;

namespace ShareInvest
{
    internal partial class GoblinBat : Form
    {
        internal GoblinBat(char initial, Secret secret)
        {
            this.secret = secret;
            InitializeComponent();
            open = OpenAPI.ConnectAPI.GetInstance();
            open.SetAPI(axAPI);
            open.SendCount += OnReceiveNotifyIcon;

            switch (initial)
            {
                case 'T':
                    open.StartProgress();
                    Size = new Size(238, 35);
                    CenterToScreen();
                    break;

                case 'C':
                    open.StartProgress(new OpenAPI.Temporary(open, new Queue<string>(1024)));
                    Size = new Size(5, 5);
                    Opacity = 0;
                    xing = XingAPI.ConnectAPI.GetInstance();
                    break;
            }
        }
        private void OnReceiveItem(string item)
        {
            switch (item)
            {
                case "quotes":
                    open.SendQuotes += Quotes.OnReceiveQuotes;
                    open.SendState += Quotes.OnReceiveState;
                    open.SendTrend += Quotes.OnReceiveTrend;
                    Size = new Size(323, 493);
                    Quotes.Show();
                    break;

                case "exit":
                    Size = new Size(241, 0);
                    CenterToScreen();
                    Close();
                    return;

                case "strategy":
                    Size = new Size(775, 375);
                    Statistical.Show();
                    break;

                case "account":
                    open.SendDeposit += Account.OnReceiveDeposit;
                    open.LookUpTheDeposit(Acc);
                    Size = new Size(749, 372);
                    Account.Show();
                    break;

                case "balance":
                    Size = new Size(249, 0);
                    open.SendBalance += Balance.OnReceiveBalance;
                    Balance.SendReSize += OnReceiveSize;
                    open.LookUpTheBalance(Acc);
                    Balance.Show();
                    break;
            };
            CenterToScreen();
        }
        private void OnItemClick(object sender, ToolStripItemClickedEventArgs e)
        {
            SuspendLayout();
            BeginInvoke(new Action(() => OnReceiveItem(e.ClickedItem.Name)));
            OnClickMinimized = e.ClickedItem.Name;
            Visible = true;
            ShowIcon = true;
            notifyIcon.Visible = false;
            WindowState = FormWindowState.Normal;
            Application.DoEvents();
            ResumeLayout();
        }
        private void OnReceiveSize(object sender, GridResize e)
        {
            Size = new Size(Server ? 591 : 599, e.ReSize + e.Count + 33);
            open.SendCurrent += Balance.OnRealTimeCurrentPriceReflect;
        }
        private void OnReceiveNotifyIcon(object sender, NotifyIconText e)
        {
            switch (e.NotifyIcon.GetType().Name)
            {
                case "Dictionary`2":
                    var temp = (Dictionary<int, string>)e.NotifyIcon;

                    if (temp.ContainsKey(0))
                    {
                        notifyIcon.Text = "CheckDataBase";
                        open.StartProgress(3605);
                        notifyIcon.Text = secret.GoblinBat;

                        return;
                    }
                    var first = temp.First();
                    notifyIcon.Text = string.Concat(first.Key, '_', first.Value);
                    return;

                case "StringBuilder":
                    strip.ItemClicked += OnItemClick;
                    BeginInvoke(new Action(() =>
                    {
                        Quotes = new QuotesControl();
                        panel.Controls.Add(Quotes);
                        open.SendQuotes += Quotes.OnReceiveQuotes;
                        Quotes.Dock = DockStyle.Fill;
                        Account = new AccountControl();
                        panel.Controls.Add(Account);
                        Account.Dock = DockStyle.Fill;
                        open.SendDeposit += Account.OnReceiveDeposit;
                        Balance = new BalanceControl();
                        panel.Controls.Add(Balance);
                        Balance.Dock = DockStyle.Fill;
                        open.SendBalance += Balance.OnReceiveBalance;
                        Statistical = new StatisticalAnalysis();
                        panel.Controls.Add(Statistical);
                        Statistical.Dock = DockStyle.Fill;
                        var chart = Retrieve.GetInstance(open.Strategy).Chart;
                        var check = e.NotifyIcon.ToString().Split(';');
                        Acc = new string[check.Length - 3];
                        Server = check[check.Length - 1].Equals("1");

                        if (Server ? false : new VerifyIdentity().Identify(check[check.Length - 3], check[check.Length - 2]) == false)
                        {
                            TimerBox.Show(new Secret(check[check.Length - 2]).Identify, secret.GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Warning, 3750);
                            Dispose();

                            return;
                        }
                        for (int i = 0; i < check.Length - 3; i++)
                            Acc[i] = check[i];

                        new Task(() =>
                        {
                            var specify = new Specify
                            {
                                Account = Acc,
                                Assets = 17500000,
                                Code = open.Strategy,
                                Strategy = "TF",
                                Time = 30,
                                Short = 4,
                                Long = 60
                            };
                            new Trading(open, specify, new Quotes(specify, open), chart);
                        }).Start();
                        new Task(() =>
                        {
                            var liquidate = new Specify
                            {
                                Account = Acc,
                                Assets = 17500000,
                                Code = open.Strategy,
                                Strategy = "WU",
                                Time = 15,
                                Short = 4,
                                Long = 60
                            };
                            new Trading(open, liquidate, new Quotes(liquidate, open), chart);
                        }).Start();
                        new Task(() => new Trading(open, new Specify
                        {
                            Account = Acc,
                            Assets = 17500000,
                            Code = open.Strategy,
                            Strategy = "DL",
                            Time = 1440,
                            Short = 4,
                            Long = 60,
                            Reaction = 531
                        }, chart)).Start();
                        open.SendState += Quotes.OnReceiveState;
                        open.SendTrend += Quotes.OnReceiveTrend;
                        Retrieve.Dispose();
                    }));
                    return;

                case "String":
                    BeginInvoke(new Action(() => Quotes.OnReceiveOrderMsg(e.NotifyIcon.ToString())));
                    return;

                case "Byte":
                    if (open.Temporary == null)
                    {
                        Account.Show();
                        open.SendDeposit -= Account.OnReceiveDeposit;
                        Account.Hide();
                        Balance.Show();
                        open.SendBalance -= Balance.OnReceiveBalance;
                        Balance.Hide();
                        BackColor = Color.FromArgb(121, 133, 130);
                        Opacity = 0.8135;
                        OnClickMinimized = "quotes";
                        open.SendState -= Quotes.OnReceiveState;
                        open.SendTrend -= Quotes.OnReceiveTrend;
                    }
                    WindowState = FormWindowState.Minimized;
                    notifyIcon.Text = open.Code;
                    return;

                case "Char":
                    Dispose();
                    break;
            };
        }
        private void GoblinBatFormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(secret.Exit, secret.GoblinBat, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning).Equals(DialogResult.Cancel))
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;

                return;
            }
            Dispose();
        }
        private void GoblinBatResize(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                if (WindowState.Equals(FormWindowState.Minimized))
                {
                    switch (OnClickMinimized)
                    {
                        case "quotes":
                            open.SendQuotes -= Quotes.OnReceiveQuotes;
                            open.SendState -= Quotes.OnReceiveState;
                            open.SendTrend -= Quotes.OnReceiveTrend;
                            Quotes.Hide();
                            break;

                        case "account":
                            open.SendDeposit -= Account.OnReceiveDeposit;
                            Account.Hide();
                            break;

                        case "balance":
                            open.SendCurrent -= Balance.OnRealTimeCurrentPriceReflect;
                            open.SendBalance -= Balance.OnReceiveBalance;
                            Balance.SendReSize -= OnReceiveSize;
                            Balance.Hide();
                            break;

                        case "strategy":
                            Statistical.Hide();
                            break;

                        default:
                            break;
                    };
                    Visible = false;
                    ShowIcon = false;
                    notifyIcon.Visible = true;
                }
            }));
        }
        private string OnClickMinimized
        {
            get; set;
        }
        private string[] Acc
        {
            get; set;
        }
        private bool Server
        {
            get; set;
        }
        private AccountControl Account
        {
            get; set;
        }
        private BalanceControl Balance
        {
            get; set;
        }
        private QuotesControl Quotes
        {
            get; set;
        }
        private StatisticalAnalysis Statistical
        {
            get; set;
        }
        private readonly XingAPI.ConnectAPI xing;
        private readonly OpenAPI.ConnectAPI open;
        private readonly Secret secret;
    }
}