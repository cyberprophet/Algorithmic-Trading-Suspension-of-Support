using System;
using System.Drawing;
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
            Size = new Size(249, 35);
        }
        private void OnItemClick(object sender, ToolStripItemClickedEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                switch (e.ClickedItem.Name)
                {
                    case "quotes":
                        api.SendQuotes += Quotes.OnReceiveQuotes;
                        Size = new Size(314, 435);
                        Quotes.Show();
                        break;

                    case "exit":
                        Close();
                        return;

                    case "strategy":
                        Statistical.Show();
                        break;

                    case "account":
                        api.SendDeposit += Account.OnReceiveDeposit;
                        api.LookUpTheDeposit(Acc);
                        Size = new Size(750, 370);
                        Account.Show();
                        break;

                    case "balance":
                        api.SendBalance += Balance.OnReceiveBalance;
                        Balance.SendReSize += OnReceiveSize;
                        api.LookUpTheBalance(Acc);
                        Balance.Show();
                        break;
                };
                CenterToScreen();
                SuspendLayout();
                OnClickMinimized = e.ClickedItem.Name;
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                WindowState = FormWindowState.Normal;
                ResumeLayout(true);
                PerformLayout();
            }));
        }
        private void OnReceiveSize(object sender, GridResize e)
        {
            Size = new Size(Server ? 594 : 602, e.ReSize + (e.Count - 7) * 21);
            api.SendCurrent += Balance.OnRealTimeCurrentPriceReflect;
        }
        private void OnReceiveNotifyIcon(object sender, NotifyIconText e)
        {
            switch (e.NotifyIcon.GetType().Name)
            {
                case "Int32":
                    if ((int)e.NotifyIcon == 0)
                    {
                        notifyIcon.Text = "CheckDataBase";
                        api.StartProgress(3605);
                        notifyIcon.Text = "GoblinBat";

                        return;
                    }
                    notifyIcon.Text = e.NotifyIcon.ToString();
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
                        Balance = new BalanceControl();
                        panel.Controls.Add(Balance);
                        Balance.Dock = DockStyle.Fill;
                        Statistical = new StatisticalAnalysis();
                        panel.Controls.Add(Statistical);
                        Statistical.Dock = DockStyle.Fill;
                    }));
                    var check = e.NotifyIcon.ToString().Split(';');
                    Acc = new string[check.Length - 3];
                    Server = check[check.Length - 1].Equals("1");

                    if (Server ? false : new VerifyIdentity().Identify(check[check.Length - 3], check[check.Length - 2]) == false)
                    {
                        TimerBox.Show(new Message(check[check.Length - 2]).Identify, "Caution", MessageBoxButtons.OK, MessageBoxIcon.Warning, 3750);
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
                        Time = 30,
                        Short = 4,
                        Long = 60
                    };
                    new Trading(api, specify, new Strategy.Quotes(specify, api));

                    return;

                case "String":
                    BeginInvoke(new Action(() => Quotes.OnReceiveOrderMsg(e.NotifyIcon.ToString())));
                    return;

                case "Byte":
                    CenterToScreen();
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
            if (MessageBox.Show(new Message().Exit, "Caution", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning).Equals(DialogResult.Cancel))
            {
                e.Cancel = true;

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
                    Visible = false;
                    ShowIcon = false;
                    notifyIcon.Visible = true;

                    switch (OnClickMinimized)
                    {
                        case "quotes":
                            api.SendQuotes -= Quotes.OnReceiveQuotes;
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
                    return;
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