using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.Interface.Struct;
using ShareInvest.Statistic;
using ShareInvest.Strategy;
using ShareInvest.XingAPI;
using ShareInvest.XingControls;
using XA_DATASETLib;
using XA_SESSIONLib;

namespace ShareInvest.GoblinBat
{
    internal partial class StartUp : Form
    {
        internal StartUp(XASessionClass session)
        {
            secret = new Secret();

            if (session.ConnectServer(secret.Server[1], secret.Port) && session.Login(secret.InfoToConnect[0], secret.InfoToConnect[1], secret.InfoToConnect[2], 0, true))
            {
                this.session = session;
                session._IXASessionEvents_Event_Login += OnEventConnect;
                session.Disconnect += OnReceiveDisconnect;
            }
            else
            {

            }
            InitializeComponent();
        }
        private void OnEventConnect(string szCode, string szMsg)
        {
            if (szCode.Equals(secret.Code) && session.IsConnected())
            {
                secret.Accounts = new string[session.GetAccountListCount()];

                for (int i = 0; i < secret.Accounts.Length; i++)
                    secret.Accounts[i] = session.GetAccountList(i);
                                
                API = ConnectXingAPI.GetInstance();
                API.SetAPI(new XAQueryClass());
                API.SetAPI(new XARealClass());
                API.StartProgress(secret.Path, secret.Accounts);
                API.SendCount += OnReceiveNotifyIcon;
            }
            else
            {

            }
            Size = new Size(238, 35);
            CenterToScreen();
        }
        private void OnReceiveNotifyIcon(object sender, NotifyIconText e)
        {
            switch (e.NotifyIcon.GetType().Name)
            {
                case "StringBuilder":
                    strip.ItemClicked += OnItemClick;
                    BeginInvoke(new Action(() =>
                    {
                        Quotes = new QuotesControl();
                        panel.Controls.Add(Quotes);
                        API.SendQuotes += Quotes.OnReceiveQuotes;
                        Quotes.Dock = DockStyle.Fill;
                        Account = new AccountControl();
                        panel.Controls.Add(Account);
                        Account.Dock = DockStyle.Fill;
                        API.SendDeposit += Account.OnReceiveDeposit;
                        Balance = new BalanceControl();
                        panel.Controls.Add(Balance);
                        Balance.Dock = DockStyle.Fill;
                        API.SendBalance += Balance.OnReceiveBalance;
                        Statistical = new StatisticalControl();
                        panel.Controls.Add(Statistical);
                        Statistical.Dock = DockStyle.Fill;
                        var code = e.NotifyIcon.ToString().Split(';');
                        var chart = Retrieve.GetInstance(code[code.Length - 1]).Chart;
                        new Task(() =>
                        {
                            var specify = new Specify
                            {
                                Account = secret.Accounts,
                                Assets = 17500000,
                                Code = code[code.Length - 1],
                                Strategy = "TF",
                                Time = 30,
                                Short = 4,
                                Long = 60
                            };
                            new Trading(API, specify, new Statistic.Quotes(specify, API), chart);
                        }).Start();
                        new Task(() =>
                        {
                            var liquidate = new Specify
                            {
                                Account = secret.Accounts,
                                Assets = 17500000,
                                Code = code[code.Length - 1],
                                Strategy = "WU",
                                Time = 15,
                                Short = 4,
                                Long = 60
                            };
                            new Trading(API, liquidate, new Statistic.Quotes(liquidate, API), chart);
                        }).Start();
                        new Task(() => new Trading(API, new Specify
                        {
                            Account = secret.Accounts,
                            Assets = 17500000,
                            Code = code[code.Length - 1],
                            Strategy = "DL",
                            Time = 1440,
                            Short = 4,
                            Long = 60,
                            Reaction = 531
                        }, chart)).Start();
                        API.SendState += Quotes.OnReceiveState;
                        API.SendTrend += Quotes.OnReceiveTrend;
                        Retrieve.Dispose();
                    }));
                    return;

                case "String":
                    BeginInvoke(new Action(() => Quotes.OnReceiveOrderMsg(e.NotifyIcon.ToString())));
                    return;

                case "Byte":
                    Account.Show();
                    API.SendDeposit -= Account.OnReceiveDeposit;
                    Account.Hide();
                    Balance.Show();
                    API.SendBalance -= Balance.OnReceiveBalance;
                    Balance.Hide();
                    BackColor = Color.FromArgb(121, 133, 130);
                    Opacity = 0.8135;
                    OnClickMinimized = "quotes";
                    WindowState = FormWindowState.Minimized;
                    API.SendState -= Quotes.OnReceiveState;
                    API.SendTrend -= Quotes.OnReceiveTrend;
                    return;

                case "Char":
                    Dispose();
                    break;
            };
        }
        private void Disconnect()
        {
            session.DisconnectServer();
        }
        private void OnReceiveDisconnect()
        {
            Dispose();
        }
        private void OnReceiveItem(string item)
        {
            switch (item)
            {
                case "quotes":
                    API.SendQuotes += Quotes.OnReceiveQuotes;
                    API.SendState += Quotes.OnReceiveState;
                    API.SendTrend += Quotes.OnReceiveTrend;
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
                    API.SendDeposit += Account.OnReceiveDeposit;
                    Size = new Size(749, 372);
                    Account.Show();
                    break;

                case "balance":
                    Size = new Size(249, 0);
                    API.SendBalance += Balance.OnReceiveBalance;
                    Balance.SendReSize += OnReceiveSize;
                    Balance.Show();
                    break;
            };
            CenterToScreen();
        }
        private void OnReceiveSize(object sender, GridResize e)
        {
            Size = new Size(599, e.ReSize + e.Count + 33);
            API.SendCurrent += Balance.OnRealTimeCurrentPriceReflect;
        }
        private void StartUpResize(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                if (WindowState.Equals(FormWindowState.Minimized))
                {
                    switch (OnClickMinimized)
                    {
                        case "quotes":
                            API.SendQuotes -= Quotes.OnReceiveQuotes;
                            API.SendState -= Quotes.OnReceiveState;
                            API.SendTrend -= Quotes.OnReceiveTrend;
                            Quotes.Hide();
                            break;

                        case "account":
                            API.SendDeposit -= Account.OnReceiveDeposit;
                            Account.Hide();
                            break;

                        case "balance":
                            API.SendCurrent -= Balance.OnRealTimeCurrentPriceReflect;
                            API.SendBalance -= Balance.OnReceiveBalance;
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
        private void StartUpFormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(secret.Exit, secret.GoblinBat, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning).Equals(DialogResult.Cancel))
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;

                return;
            }
            Dispose();
        }
        private string OnClickMinimized
        {
            get; set;
        }
        private ConnectXingAPI API
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
        private StatisticalControl Statistical
        {
            get; set;
        }
        private readonly Secret secret;
        private readonly XASessionClass session;
    }
}