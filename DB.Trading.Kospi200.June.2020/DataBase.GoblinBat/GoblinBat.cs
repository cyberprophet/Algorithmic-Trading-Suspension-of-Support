using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.GoblinBatControls;
using ShareInvest.Interface.Struct;
using ShareInvest.Message;
using ShareInvest.OpenAPI;
using ShareInvest.Strategy;

namespace ShareInvest.GoblinBatForms
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
            api = ConnectAPI.GetInstance();
            api.SetAPI(axAPI);
            api.StartProgress();
            new Temporary();
            api.SendCount += OnReceiveNotifyIcon;
            Size = new Size(238, 35);
            CenterToScreen();
        }
        private void OnReceiveItem(string item)
        {
            switch (item)
            {
                case "quotes":
                    api.SendQuotes += Quotes.OnReceiveQuotes;
                    api.SendState += Quotes.OnReceiveState;
                    api.SendTrend += Quotes.OnReceiveTrend;
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
                    api.SendDeposit += Account.OnReceiveDeposit;
                    api.LookUpTheDeposit(Acc);
                    Size = new Size(749, 372);
                    Account.Show();
                    break;

                case "balance":
                    Size = new Size(249, 0);
                    api.SendBalance += Balance.OnReceiveBalance;
                    Balance.SendReSize += OnReceiveSize;
                    api.LookUpTheBalance(Acc);
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
            api.SendCurrent += Balance.OnRealTimeCurrentPriceReflect;
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
                        api.StartProgress(3605);
                        notifyIcon.Text = new Message().GoblinBat;

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
                        api.SendQuotes += Quotes.OnReceiveQuotes;
                        Quotes.Dock = DockStyle.Fill;
                        Account = new AccountControl();
                        panel.Controls.Add(Account);
                        Account.Dock = DockStyle.Fill;
                        api.SendDeposit += Account.OnReceiveDeposit;
                        Balance = new BalanceControl();
                        panel.Controls.Add(Balance);
                        Balance.Dock = DockStyle.Fill;
                        api.SendBalance += Balance.OnReceiveBalance;
                        Statistical = new StatisticalAnalysis();
                        panel.Controls.Add(Statistical);
                        Statistical.Dock = DockStyle.Fill;
                        var check = e.NotifyIcon.ToString().Split(';');
                        Acc = new string[check.Length - 3];
                        Server = check[check.Length - 1].Equals("1");

                        if (Server ? false : new VerifyIdentity().Identify(check[check.Length - 3], check[check.Length - 2]) == false)
                        {
                            TimerBox.Show(new Message(check[check.Length - 2]).Identify, new Message().GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Warning, 3750);
                            Dispose();

                            return;
                        }
                        for (int i = 0; i < check.Length - 3; i++)
                            Acc[i] = check[i];

                        var specify = new Specify
                        {
                            Account = Acc,
                            Assets = 35000000,
                            Code = api.Strategy,
                            Strategy = "TF",
                            Time = 30,
                            Short = 4,
                            Long = 60
                        };
                        new Trading(api, specify, new Strategy.Quotes(specify, api));
                        var liquidate = new Specify
                        {
                            Account = Acc,
                            Assets = 35000000,
                            Code = api.Strategy,
                            Strategy = "WU",
                            Time = 5,
                            Short = 4,
                            Long = 60
                        };
                        new Trading(api, liquidate, new Strategy.Quotes(liquidate, api));
                        new Trading(api, new Specify
                        {
                            Account = Acc,
                            Assets = 35000000,
                            Code = api.Strategy,
                            Strategy = "DL",
                            Time = 1440,
                            Short = 4,
                            Long = 60
                        });
                        api.SendState += Quotes.OnReceiveState;
                        api.SendTrend += Quotes.OnReceiveTrend;
                    }));
                    return;

                case "String":
                    BeginInvoke(new Action(() => Quotes.OnReceiveOrderMsg(e.NotifyIcon.ToString())));
                    return;

                case "Byte":
                    Account.Show();
                    api.SendDeposit -= Account.OnReceiveDeposit;
                    Account.Hide();
                    Balance.Show();
                    api.SendBalance -= Balance.OnReceiveBalance;
                    Balance.Hide();
                    BackColor = Color.FromArgb(121, 133, 130);
                    Opacity = 0.8135;
                    OnClickMinimized = "quotes";
                    WindowState = FormWindowState.Minimized;
                    return;

                case "Char":
                    Dispose();
                    break;
            };
        }
        private void GoblinBatFormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(new Message().Exit, new Message().GoblinBat, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning).Equals(DialogResult.Cancel))
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
                            api.SendQuotes -= Quotes.OnReceiveQuotes;
                            api.SendState -= Quotes.OnReceiveState;
                            api.SendTrend -= Quotes.OnReceiveTrend;
                            Quotes.Hide();
                            break;

                        case "account":
                            api.SendDeposit -= Account.OnReceiveDeposit;
                            Account.Hide();
                            break;

                        case "balance":
                            api.SendCurrent -= Balance.OnRealTimeCurrentPriceReflect;
                            api.SendBalance -= Balance.OnReceiveBalance;
                            Balance.SendReSize -= OnReceiveSize;
                            Balance.Hide();
                            break;

                        case "strategy":
                            Statistical.Hide();
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
        private readonly ConnectAPI api;
    }
}