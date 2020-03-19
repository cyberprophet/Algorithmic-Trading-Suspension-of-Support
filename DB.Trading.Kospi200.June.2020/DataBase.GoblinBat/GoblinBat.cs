using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;
using ShareInvest.GoblinBatControls;
using ShareInvest.Message;

namespace ShareInvest
{
    internal partial class GoblinBat : Form
    {
        internal GoblinBat(char initial, Secret secret)
        {
            this.initial = initial;
            this.secret = secret;
            InitializeComponent();
            Opacity = 0;
            Open = OpenAPI.ConnectAPI.GetInstance(initial);
            Open.SetAPI(axAPI);
            Open.SendCount += OnReceiveNotifyIcon;

            switch (initial)
            {
                case collecting:
                case backTesting:
                case trading:
                    if (Quotes == null)
                    {
                        Quotes = new QuotesControl();
                        panel.Controls.Add(Quotes);
                        Quotes.Dock = DockStyle.Fill;
                        Quotes.Show();
                        strip.ItemClicked += OnItemClick;
                    }
                    Open.SendQuotes += Quotes.OnReceiveQuotes;
                    Open.StartProgress(new OpenAPI.Temporary(Open, new Queue<string>(1024), initial));
                    Size = new Size(5, 5);
                    break;

                default:
                    Open.StartProgress();
                    Size = new Size(238, 35);
                    break;
            }
            CenterToScreen();
        }
        private void OnReceiveItem(string item)
        {
            var connect = Array.Exists(XingConnect, o => o.Equals(initial));

            switch (item)
            {
                case quo:
                    switch (connect)
                    {
                        case true:
                            if (Xing != null)
                            {
                                Text = XingAPI.ConnectAPI.Code;
                                ((IEvents<EventHandler.XingAPI.Quotes>)Real[quo]).Send += Quotes.OnReceiveQuotes;
                                ((ITrends<Trends>)Real[datum]).SendTrend += Quotes.OnReceiveTrend;
                            }
                            break;

                        case false:
                            Open.SendQuotes += Quotes.OnReceiveQuotes;
                            Open.SendState += Quotes.OnReceiveState;
                            Open.SendTrend += Quotes.OnReceiveTrend;
                            break;
                    }
                    Size = new Size(323, 493);
                    Quotes.Show();
                    break;

                case ex:
                    Size = new Size(241, 0);
                    CenterToScreen();
                    Close();
                    return;

                case st:
                    if (Xing != null && connect)
                        Text = Xing.GetAccountName(Xing.Accounts.Length == 1 ? Xing.Accounts[0] : Array.Find(Xing.Accounts, o => o.Substring(o.Length - 2, 2).Equals("02")));

                    Size = new Size(775, 375);
                    Statistical.Show();
                    break;

                case acc:
                    switch (connect)
                    {
                        case true:
                            if (Xing != null)
                            {
                                Text = (Xing.Accounts.Length == 1 ? Xing.Accounts[0] : Array.Find(Xing.Accounts, o => o.Substring(o.Length - 2, 2).Equals("02"))).Insert(5, "-").Insert(3, "-");
                                var query = Xing.query[0];
                                ((IEvents<Deposit>)query).Send += Account.OnReceiveDeposit;
                                ((IMessage<NotifyIconText>)query).SendMessage += OnReceiveNotifyIcon;
                                query.QueryExcute();
                            }
                            break;

                        case false:
                            Open.SendDeposit += Account.OnReceiveDeposit;
                            Open.LookUpTheDeposit(Acc);
                            break;
                    }
                    Size = new Size(749, 372);
                    Account.Show();
                    break;

                case bal:
                    switch (connect)
                    {
                        case true:
                            if (Xing != null)
                            {
                                Text = Xing.DetailName;
                                var query = Xing.query[1];
                                ((IEvents<Balance>)query).Send += Balance.OnReceiveBalance;
                                ((IMessage<NotifyIconText>)query).SendMessage += OnReceiveNotifyIcon;
                                query.QueryExcute();
                            }
                            break;

                        case false:
                            Open.SendBalance += Balance.OnReceiveBalance;
                            Open.LookUpTheBalance(Acc);
                            break;
                    }
                    Size = new Size(249, 0);
                    Balance.SendReSize += OnReceiveSize;
                    break;
            };
        }
        private void OnItemClick(object sender, ToolStripItemClickedEventArgs e)
        {
            if (Xing == null && Array.Exists(XingConnect, o => o.Equals(initial)))
                switch (e.ClickedItem.Name)
                {
                    case quo:
                    case bal:
                    case acc:
                        if (MessageBox.Show(secret.CME, secret.GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Warning).Equals(DialogResult.OK))
                            return;

                        break;
                }
            SuspendLayout();
            OnClickMinimized = e.ClickedItem.Name;
            Visible = true;
            ShowIcon = true;
            notifyIcon.Visible = false;
            WindowState = FormWindowState.Normal;
            BeginInvoke(new Action(() => OnReceiveItem(e.ClickedItem.Name)));
            Application.DoEvents();
            ResumeLayout();
            CenterToScreen();
        }
        private void OnReceiveSize(object sender, GridResize e)
        {
            var connect = Array.Exists(XingConnect, o => o.Equals(initial));
            Size = new Size(Server ? 591 : (connect ? 604 : 599), e.ReSize + 34);
            Balance.Show();

            if (connect)
            {

            }
            else
                Open.SendCurrent += Balance.OnRealTimeCurrentPriceReflect;
        }
        private void OnReceiveNotifyIcon(object sender, NotifyIconText e)
        {
            switch (e.NotifyIcon.GetType().Name)
            {
                case dic:
                    var temp = (Dictionary<int, string>)e.NotifyIcon;

                    if (temp.ContainsKey(0))
                    {
                        notifyIcon.Text = checkDataBase;
                        Open.StartProgress(3605);
                        notifyIcon.Text = secret.GoblinBat;

                        return;
                    }
                    var first = temp.First();
                    notifyIcon.Text = string.Concat(DateTime.Now.ToShortTimeString(), " Remains_", first.Key, " Code_", first.Value);
                    return;

                case sb:
                    strip.ItemClicked += OnItemClick;
                    BeginInvoke(new Action(() =>
                    {
                        if (Quotes == null)
                        {
                            Quotes = new QuotesControl();
                            panel.Controls.Add(Quotes);
                            Open.SendQuotes += Quotes.OnReceiveQuotes;
                            Quotes.Dock = DockStyle.Fill;
                        }
                        if (Account == null)
                        {
                            Account = new AccountControl();
                            panel.Controls.Add(Account);
                            Account.Dock = DockStyle.Fill;
                            Open.SendDeposit += Account.OnReceiveDeposit;
                        }
                        if (Balance == null)
                        {
                            Balance = new BalanceControl();
                            panel.Controls.Add(Balance);
                            Balance.Dock = DockStyle.Fill;
                            Open.SendBalance += Balance.OnReceiveBalance;
                        }
                        if (Statistical == null)
                        {
                            Statistical = new StatisticalAnalysis();
                            panel.Controls.Add(Statistical);
                            Statistical.Dock = DockStyle.Fill;
                        }
                        var chart = Retrieve.GetInstance(initial, Open.Code).Chart;
                        var check = e.NotifyIcon.ToString().Split((char)59);
                        Acc = new string[check.Length - 3];
                        Server = check[check.Length - 1].Equals(secret.Mock);

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
                                Code = Open.Code,
                                Strategy = "TF",
                                Time = 30,
                                Short = 4,
                                Long = 60
                            };
                            new Strategy.OpenAPI.Trading(Open, specify, new Strategy.OpenAPI.Quotes(specify, Open), chart);
                        }).Start();
                        new Task(() =>
                        {
                            var liquidate = new Specify
                            {
                                Account = Acc,
                                Assets = 17500000,
                                Code = Open.Code,
                                Strategy = "WU",
                                Time = 15,
                                Short = 4,
                                Long = 60
                            };
                            new Strategy.OpenAPI.Trading(Open, liquidate, new Strategy.OpenAPI.Quotes(liquidate, Open), chart);
                        }).Start();
                        new Task(() => new Strategy.OpenAPI.Trading(Open, new Specify
                        {
                            Account = Acc,
                            Assets = 17500000,
                            Code = Open.Code,
                            Strategy = "DL",
                            Time = 1440,
                            Short = 4,
                            Long = 60,
                            Reaction = 531
                        }, chart)).Start();
                        Open.SendState += Quotes.OnReceiveState;
                        Open.SendTrend += Quotes.OnReceiveTrend;
                        Retrieve.Dispose();
                    }));
                    return;

                case str:
                    BeginInvoke(new Action(() => Quotes.OnReceiveOrderMsg(e.NotifyIcon.ToString())));
                    return;

                case boolean:
                    WorkOnTheDeadLine(e.NotifyIcon);
                    break;

                case bt:
                    if (Array.Exists(XingConnect, o => o.Equals(initial)))
                    {
                        while ((DateTime.Now.Minute == 49 && (DateTime.Now.Hour == 8 || DateTime.Now.Hour == 17)) == false)
                            if (TimerBox.Show("", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, 30000U).Equals(DialogResult.OK))
                                if (Statistical == null)
                                {
                                    Quotes.Hide();
                                    Statistical = new StatisticalAnalysis();
                                    panel.Controls.Add(Statistical);
                                    Statistical.Dock = DockStyle.Fill;
                                    Statistical.Show();
                                    Visible = false;
                                    Size = new Size(775, 375);
                                    Opacity = 0.85;
                                    BackColor = Color.FromArgb(121, 133, 130);
                                    Application.DoEvents();
                                    ShowDialog();
                                }
                        BeginInvoke(new Action(() =>
                        {
                            Real = new Dictionary<string, IReals>();
                            Xing = XingAPI.ConnectAPI.GetInstance(Open.Code);
                            Xing.Send += OnReceiveNotifyIcon;
                            Task = new Task(() =>
                            {
                                var retrieve = new Strategy.Retrieve(initial);
                                retrieve.SetInitializeTheChart();
                                retrieve.SetInitialzeTheCode(Open.Code);
                            });
                            Task.Start();
                            notifyIcon.Text = string.Concat("Trading Code_", Open.Code);
                            OnEventConnect();
                            OnClickMinimized = quo;
                            Application.DoEvents();
                        }));
                    }
                    else
                    {
                        Account.Show();
                        Open.SendDeposit -= Account.OnReceiveDeposit;
                        Account.Hide();
                        Balance.Show();
                        Open.SendBalance -= Balance.OnReceiveBalance;
                        Balance.Hide();
                        Open.SendState -= Quotes.OnReceiveState;
                        Open.SendTrend -= Quotes.OnReceiveTrend;
                    }
                    return;

                case int32:
                    if ((int)e.NotifyIcon < 0)
                        WorkOnTheDeadLine(e.NotifyIcon);

                    else
                    {
                        foreach (var ctor in Xing.query)
                            switch (ctor.GetType().Name)
                            {
                                case cfobq10500:
                                case ccebq10500:
                                    ((IEvents<Deposit>)ctor).Send -= Account.OnReceiveDeposit;
                                    ((IMessage<NotifyIconText>)ctor).SendMessage -= OnReceiveNotifyIcon;
                                    Account.Hide();
                                    break;

                                case t0441:
                                case cceaq50600:
                                    ((IEvents<Balance>)ctor).Send -= Balance.OnReceiveBalance;
                                    ((IMessage<NotifyIconText>)ctor).SendMessage -= OnReceiveNotifyIcon;
                                    Balance.Hide();
                                    break;
                            }
                        return;
                    }
                    break;

                case cha:
                    switch ((char)e.NotifyIcon)
                    {
                        case (char)69:
                            Dispose();
                            return;

                        case (char)41:
                            if (Temporary != null)
                            {
                                Task = new Task(() => Temporary.SetStorage(Open.Code));
                                Task.Start();
                            }
                            break;

                        default:
                            return;
                    }
                    break;
            };
        }
        private void WorkOnTheDeadLine(object param)
        {
            if (param.GetType().Name.Equals(int32))
            {
                Open.Dispose(true);
                axAPI.Dispose();
            }
            Task.Wait();
            timer.Start();
            Xing.DisconnectServer();
            Xing.Dispose(true);
            Xing = null;
        }
        private void OnEventConnect()
        {
            foreach (var ctor in Xing.query)
            {
                switch (ctor.GetType().Name)
                {
                    case cfobq10500:
                    case ccebq10500:
                        BeginInvoke(new Action(() =>
                        {
                            if (Account == null)
                            {
                                Account = new AccountControl();
                                panel.Controls.Add(Account);
                                Account.Dock = DockStyle.Fill;
                                Account.Show();
                            }
                            ((IEvents<Deposit>)ctor).Send += Account.OnReceiveDeposit;
                            ((IMessage<NotifyIconText>)ctor).SendMessage += OnReceiveNotifyIcon;
                        }));
                        break;

                    case t0441:
                    case cceaq50600:
                        BeginInvoke(new Action(() =>
                        {
                            if (Balance == null)
                            {
                                Balance = new BalanceControl();
                                panel.Controls.Add(Balance);
                                Balance.Dock = DockStyle.Fill;
                                Balance.SendReSize += OnReceiveSize;
                            }
                            ((IEvents<Balance>)ctor).Send += Balance.OnReceiveBalance;
                            ((IMessage<NotifyIconText>)ctor).SendMessage += OnReceiveNotifyIcon;
                        }));
                        break;
                }
                ctor.QueryExcute();
            }
            foreach (var ctor in Xing.real)
            {
                switch (ctor.GetType().Name)
                {
                    case fc0:
                    case nc0:
                        Real[datum] = ctor;
                        ((ITrends<Trends>)ctor).SendTrend += Quotes.OnReceiveTrend;
                        break;

                    case fh0:
                    case nh0:
                        Open.SendQuotes -= Quotes.OnReceiveQuotes;
                        Real[quo] = ctor;
                        ((IEvents<EventHandler.XingAPI.Quotes>)ctor).Send += Quotes.OnReceiveQuotes;
                        break;

                    case jif:
                        BeginInvoke(new Action(() =>
                        {
                            ((IEvents<NotifyIconText>)ctor).Send += OnReceiveNotifyIcon;

                            if (Statistical == null)
                            {
                                Statistical = new StatisticalAnalysis();
                                panel.Controls.Add(Statistical);
                                Statistical.Dock = DockStyle.Fill;
                            }
                        }));
                        break;
                }
                ctor.OnReceiveRealTime(Open.Code);
            }
            Task.Wait();
            WindowState = Xing.SendNotifyIconText(notifyIcon.Text.Length * Open.Code.Length);
            var strategy = new Catalog.Strategy
            {
                Assets = 17500000,
                Code = Open.Code,
                Contents = new Dictionary<string, string>()
                {
                    {
                        "DL",
                        "1440;4;60;531"
                    },
                    {
                        "WU",
                        "15;4;60;0"
                    },
                    {
                        "TF",
                        "30;4;60;0"
                    }
                }
            };
            foreach (var kv in strategy.Contents)
            {
                var temp = kv.Value.Split(';');
                new Task(() => new Strategy.XingAPI.Quotes((IEvents<EventHandler.XingAPI.Quotes>)Real[quo], (IEvents<EventHandler.XingAPI.Datum>)Real[datum], new Specify
                {
                    Assets = strategy.Assets,
                    Code = strategy.Code,
                    Strategy = kv.Key,
                    Time = int.Parse(temp[0]),
                    Short = int.Parse(temp[1]),
                    Long = int.Parse(temp[2]),
                    Reaction = int.Parse(temp[3])
                })).Start();
            }
            if (DateTime.Now.Hour > 16)
                Temporary = new XingAPI.Temporary(Real[quo], Real[datum], new Queue<string>(), initial);
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
                    var connect = Array.Exists(XingConnect, o => o.Equals(initial));

                    switch (OnClickMinimized)
                    {
                        case quo:
                            if (connect)
                            {
                                ((IEvents<EventHandler.XingAPI.Quotes>)Real[quo]).Send -= Quotes.OnReceiveQuotes;
                                ((ITrends<Trends>)Real[datum]).SendTrend -= Quotes.OnReceiveTrend;
                            }
                            else
                            {
                                Open.SendQuotes -= Quotes.OnReceiveQuotes;
                                Open.SendState -= Quotes.OnReceiveState;
                                Open.SendTrend -= Quotes.OnReceiveTrend;
                            }
                            Quotes.Hide();
                            break;

                        case acc:
                            if (connect)
                            {
                                var query = Xing.query[0];
                                ((IEvents<Deposit>)query).Send -= Account.OnReceiveDeposit;
                                ((IMessage<NotifyIconText>)query).SendMessage -= OnReceiveNotifyIcon;
                            }
                            else
                                Open.SendDeposit -= Account.OnReceiveDeposit;

                            Account.Hide();
                            break;

                        case bal:
                            if (connect)
                            {
                                var query = Xing.query[1];
                                ((IEvents<Balance>)query).Send -= Balance.OnReceiveBalance;
                                ((IMessage<NotifyIconText>)query).SendMessage -= OnReceiveNotifyIcon;
                            }
                            else
                            {
                                Open.SendCurrent -= Balance.OnRealTimeCurrentPriceReflect;
                                Open.SendBalance -= Balance.OnReceiveBalance;
                            }
                            Balance.SendReSize -= OnReceiveSize;
                            Balance.Hide();
                            break;

                        case st:
                            Statistical.Hide();
                            break;

                        default:
                            break;
                    };
                    Opacity = 0.8135;
                    BackColor = Color.FromArgb(121, 133, 130);
                    Visible = false;
                    ShowIcon = false;
                    notifyIcon.Visible = true;
                }
            }));
        }
        private void TimerTick(object sender, EventArgs e)
        {
            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday))
                timer.Stop();

            else if ((DateTime.Now.Hour == 8 || DateTime.Now.Hour == 17) && DateTime.Now.Minute == 35)
            {
                timer.Stop();
                Process.Start("shutdown.exe", "-r");
                Dispose();
            }
        }
        private char[] XingConnect
        {
            get
            {
                return new char[]
                {
                    collecting,
                    backTesting,
                    trading
                };
            }
        }
        private string[] Acc
        {
            get; set;
        }
        private string OnClickMinimized
        {
            get; set;
        }
        private bool Server
        {
            get; set;
        }
        private Task Task
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
        private XingAPI.ConnectAPI Xing
        {
            get; set;
        }
        private OpenAPI.ConnectAPI Open
        {
            get;
        }
        private XingAPI.Temporary Temporary
        {
            get; set;
        }
        private Dictionary<string, IReals> Real
        {
            get; set;
        }
        private readonly char initial;
        private readonly Secret secret;
        private const char trading = (char)84;
        private const char backTesting = (char)66;
        private const char collecting = (char)67;
        private const string cfobq10500 = "CFOBQ10500";
        private const string ccebq10500 = "CCEBQ10500";
        private const string cceaq50600 = "CCEAQ50600";
        private const string t0441 = "T0441";
        private const string fc0 = "FC0";
        private const string fh0 = "FH0";
        private const string nc0 = "NC0";
        private const string nh0 = "NH0";
        private const string jif = "JIF";
        private const string acc = "account";
        private const string quo = "quotes";
        private const string datum = "datum";
        private const string bal = "balance";
        private const string st = "strategy";
        private const string ex = "exit";
        private const string dic = "Dictionary`2";
        private const string sb = "StringBuilder";
        private const string str = "String";
        private const string bt = "Byte";
        private const string int32 = "Int32";
        private const string cha = "Char";
        private const string boolean = "Boolean";
        private const string checkDataBase = "CheckDataBase";
    }
}