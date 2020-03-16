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

namespace ShareInvest
{
    internal partial class GoblinBat : Form
    {
        internal GoblinBat(char initial, Secret secret)
        {
            Real = new Dictionary<string, IReals>();
            this.initial = initial;
            this.secret = secret;
            InitializeComponent();
            open = OpenAPI.ConnectAPI.GetInstance();
            open.SetAPI(axAPI);
            open.SendCount += OnReceiveNotifyIcon;

            switch (initial)
            {
                case xing:
                    Quotes = new QuotesControl();
                    panel.Controls.Add(Quotes);
                    open.SendQuotes += Quotes.OnReceiveQuotes;
                    Quotes.Dock = DockStyle.Fill;
                    Quotes.Show();
                    strip.ItemClicked += OnItemClick;
                    open.StartProgress(new OpenAPI.Temporary(open, new Queue<string>(1024)));
                    Size = new Size(5, 5);
                    Opacity = 0;
                    break;

                default:
                    open.StartProgress();
                    Size = new Size(238, 35);
                    break;
            }
            CenterToScreen();
        }
        private void OnReceiveItem(string item)
        {
            switch (item)
            {
                case quo:
                    if (initial.Equals(xing))
                    {
                        Text = XingAPI.ConnectAPI.Code;
                        ((IEvents<EventHandler.XingAPI.Quotes>)Real[quo]).Send += Quotes.OnReceiveQuotes;
                        ((ITrends<Trends>)Real[datum]).SendTrend += Quotes.OnReceiveTrend;
                    }
                    else
                    {
                        open.SendQuotes += Quotes.OnReceiveQuotes;
                        open.SendState += Quotes.OnReceiveState;
                        open.SendTrend += Quotes.OnReceiveTrend;
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
                    if (initial.Equals(xing))
                        Text = Xing.GetAccountName(Xing.Accounts.Length == 1 ? Xing.Accounts[0] : Array.Find(Xing.Accounts, o => o.Substring(o.Length - 2, 2).Equals("02")));

                    Size = new Size(775, 375);
                    Statistical.Show();
                    break;

                case acc:
                    if (initial.Equals(xing))
                    {
                        Text = (Xing.Accounts.Length == 1 ? Xing.Accounts[0] : Array.Find(Xing.Accounts, o => o.Substring(o.Length - 2, 2).Equals("02"))).Insert(5, "-").Insert(3, "-");
                        var query = Xing.query[0];
                        ((IEvents<Deposit>)query).Send += Account.OnReceiveDeposit;
                        ((IMessage<NotifyIconText>)query).SendMessage += OnReceiveNotifyIcon;
                        query.QueryExcute();
                    }
                    else
                    {
                        open.SendDeposit += Account.OnReceiveDeposit;
                        open.LookUpTheDeposit(Acc);
                    }
                    Size = new Size(749, 372);
                    Account.Show();
                    break;

                case bal:
                    if (initial.Equals(xing))
                    {
                        Text = Xing.DetailName;
                        var query = Xing.query[1];
                        ((IEvents<Balance>)query).Send += Balance.OnReceiveBalance;
                        ((IMessage<NotifyIconText>)query).SendMessage += OnReceiveNotifyIcon;
                        query.QueryExcute();
                    }
                    else
                    {
                        open.SendBalance += Balance.OnReceiveBalance;
                        open.LookUpTheBalance(Acc);
                    }
                    Size = new Size(249, 0);
                    Balance.SendReSize += OnReceiveSize;
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
            if (initial.Equals(xing))
            {

            }
            else
                open.SendCurrent += Balance.OnRealTimeCurrentPriceReflect;

            Size = new Size(Server ? 591 : (initial.Equals(xing) ? 604 : 599), e.ReSize + 34);
            Balance.Show();
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
                        open.StartProgress(3605);
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
                                Code = open.Strategy,
                                Strategy = "TF",
                                Time = 30,
                                Short = 4,
                                Long = 60
                            };
                            new Strategy.OpenAPI.Trading(open, specify, new Strategy.OpenAPI.Quotes(specify, open), chart);
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
                            new Strategy.OpenAPI.Trading(open, liquidate, new Strategy.OpenAPI.Quotes(liquidate, open), chart);
                        }).Start();
                        new Task(() => new Strategy.OpenAPI.Trading(open, new Specify
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

                case str:
                    BeginInvoke(new Action(() => Quotes.OnReceiveOrderMsg(e.NotifyIcon.ToString())));
                    return;

                case bt:
                    if (initial.Equals(xing))
                    {
                        if (DateTime.Now.Hour > 16 && (byte)e.NotifyIcon > 0 && (byte)e.NotifyIcon < 6)
                        {
                            Xing.DisconnectServer();
                            Xing.Dispose((byte)e.NotifyIcon);
                        }
                        Xing = XingAPI.ConnectAPI.GetInstance(open.Code);
                        Xing.Send += OnReceiveNotifyIcon;
                        BeginInvoke(new Action(() =>
                        {
                            new Strategy.Retrieve().SetInitialzeTheCode(open.Code);
                            notifyIcon.Text = string.Concat("Trading Code_", open.Code);
                            OnEventConnect();
                        }));
                        OnClickMinimized = quo;
                        WindowState = Xing.SendNotifyIconText(3715);
                        Opacity = 0.8135;
                        BackColor = Color.FromArgb(121, 133, 130);
                    }
                    else
                    {
                        Account.Show();
                        open.SendDeposit -= Account.OnReceiveDeposit;
                        Account.Hide();
                        Balance.Show();
                        open.SendBalance -= Balance.OnReceiveBalance;
                        Balance.Hide();
                        open.SendState -= Quotes.OnReceiveState;
                        open.SendTrend -= Quotes.OnReceiveTrend;
                    }
                    return;

                case int32:
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

                case cha:
                    switch ((char)e.NotifyIcon)
                    {
                        case (char)69:
                            Dispose();
                            return;

                        case (char)41:
                        case (char)61:
                            if (Temporary != null)
                                Temporary.SetStorage(open.Code);

                            break;
                    }
                    break;
            };
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
                            Account = new AccountControl();
                            panel.Controls.Add(Account);
                            ((IEvents<Deposit>)ctor).Send += Account.OnReceiveDeposit;
                            ((IMessage<NotifyIconText>)ctor).SendMessage += OnReceiveNotifyIcon;
                            Account.Dock = DockStyle.Fill;
                            Account.Show();
                        }));
                        break;

                    case t0441:
                    case cceaq50600:
                        BeginInvoke(new Action(() =>
                        {
                            Balance = new BalanceControl();
                            panel.Controls.Add(Balance);
                            ((IEvents<Balance>)ctor).Send += Balance.OnReceiveBalance;
                            ((IMessage<NotifyIconText>)ctor).SendMessage += OnReceiveNotifyIcon;
                            Balance.Dock = DockStyle.Fill;
                            Balance.SendReSize += OnReceiveSize;
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
                        open.SendQuotes -= Quotes.OnReceiveQuotes;
                        Real[quo] = ctor;
                        ((IEvents<EventHandler.XingAPI.Quotes>)ctor).Send += Quotes.OnReceiveQuotes;
                        break;

                    case jif:
                        BeginInvoke(new Action(() =>
                        {
                            ((IEvents<NotifyIconText>)ctor).Send += OnReceiveNotifyIcon;
                            Statistical = new StatisticalAnalysis();
                            panel.Controls.Add(Statistical);
                            Statistical.Dock = DockStyle.Fill;
                        }));
                        break;
                }
                ctor.OnReceiveRealTime(open.Code);
            }
            var strategy = new Catalog.Strategy
            {
                Assets = 17500000,
                Code = open.Code,
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
                Temporary = new XingAPI.Temporary(Real[quo], Real[datum], new Queue<string>());
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
                        case quo:
                            if (initial.Equals(xing))
                            {
                                ((IEvents<EventHandler.XingAPI.Quotes>)Real[quo]).Send -= Quotes.OnReceiveQuotes;
                                ((ITrends<Trends>)Real[datum]).SendTrend -= Quotes.OnReceiveTrend;
                            }
                            else
                            {
                                open.SendQuotes -= Quotes.OnReceiveQuotes;
                                open.SendState -= Quotes.OnReceiveState;
                                open.SendTrend -= Quotes.OnReceiveTrend;
                            }
                            Quotes.Hide();
                            break;

                        case acc:
                            if (initial.Equals(xing))
                            {
                                var query = Xing.query[0];
                                ((IEvents<Deposit>)query).Send -= Account.OnReceiveDeposit;
                                ((IMessage<NotifyIconText>)query).SendMessage -= OnReceiveNotifyIcon;
                            }
                            else
                                open.SendDeposit -= Account.OnReceiveDeposit;

                            Account.Hide();
                            break;

                        case bal:
                            if (initial.Equals(xing))
                            {
                                var query = Xing.query[1];
                                ((IEvents<Balance>)query).Send -= Balance.OnReceiveBalance;
                                ((IMessage<NotifyIconText>)query).SendMessage -= OnReceiveNotifyIcon;
                            }
                            else
                            {
                                open.SendCurrent -= Balance.OnRealTimeCurrentPriceReflect;
                                open.SendBalance -= Balance.OnReceiveBalance;
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
        private XingAPI.ConnectAPI Xing
        {
            get; set;
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
        private readonly OpenAPI.ConnectAPI open;
        private readonly Secret secret;
        private const char xing = (char)67;
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
        private const string checkDataBase = "CheckDataBase";
    }
}