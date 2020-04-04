using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;
using ShareInvest.GoblinBatControls;
using ShareInvest.Message;
using ShareInvest.Verify;

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
            var collect = ((char)Port.Collecting).Equals(initial);

            if (collect)
            {
                Open = OpenAPI.ConnectAPI.GetInstance(KeyDecoder.GetWindowsProductKeyFromRegistry());
                Open.SetAPI(axAPI);
                Open.SendCount += OnReceiveNotifyIcon;
            }
            switch (initial)
            {
                case collecting:
                case trading:
                    if (Quotes == null)
                    {
                        Quotes = new QuotesControl();
                        panel.Controls.Add(Quotes);
                        Quotes.Dock = DockStyle.Fill;
                        Quotes.Show();
                        strip.ItemClicked += OnItemClick;
                    }
                    if (collect)
                    {
                        Open.SendQuotes += Quotes.OnReceiveQuotes;
                        Open.StartProgress(new OpenAPI.Temporary(Open, new StringBuilder(1024), KeyDecoder.GetWindowsProductKeyFromRegistry()));
                    }
                    else
                        BeginInvoke(new Action(() =>
                        {
                            Task = new Task(() =>
                            {
                                Specify = new Catalog.XingAPI.Specify[]
                                {
                                    new Catalog.XingAPI.Specify
                                    {
                                        Assets = 90000000,
                                        Code = Strategy.Retrieve.Code,
                                        Reaction = 500,
                                        Quantity = "1",
                                        RollOver = 'A',
                                        Time = 1,
                                        Short = 4,
                                        Long = 60
                                    },
                                    new Catalog.XingAPI.Specify
                                    {
                                        Assets = 90000000,
                                        Code = Strategy.Retrieve.Code,
                                        Reaction = 500,
                                        Quantity = "1",
                                        RollOver = 'A',
                                        Time = 5,
                                        Short = 4,
                                        Long = 60
                                    },
                                    new Catalog.XingAPI.Specify
                                    {
                                        Assets = 90000000,
                                        Code = Strategy.Retrieve.Code,
                                        Reaction = 500,
                                        Quantity = "1",
                                        RollOver = 'A',
                                        Time = 15,
                                        Short = 4,
                                        Long = 60
                                    },
                                    new Catalog.XingAPI.Specify
                                    {
                                        Assets = 90000000,
                                        Code = Strategy.Retrieve.Code,
                                        Reaction = 500,
                                        Quantity = "1",
                                        RollOver = 'A',
                                        Time = 30,
                                        Short = 4,
                                        Long = 60
                                    },
                                    new Catalog.XingAPI.Specify
                                    {
                                        Assets = 90000000,
                                        Code = Strategy.Retrieve.Code,
                                        Reaction = 500,
                                        Quantity = "1",
                                        RollOver = 'A',
                                        Time = 1440,
                                        Short = 4,
                                        Long = 60
                                    }
                                };
                            });
                            Task.Start();
                            Xing = XingAPI.ConnectAPI.GetInstance(initial.Equals(trading) ? Strategy.Retrieve.Code : Open.Code, Strategy.Retrieve.Date);
                            Xing.Send += OnReceiveNotifyIcon;
                            notifyIcon.Text = string.Concat("Trading Code_", initial.Equals(trading) ? Strategy.Retrieve.Code : Open.Code);
                            OnEventConnect();
                            OnClickMinimized = quo;
                            Text = gs;
                            Application.DoEvents();
                        }));
                    Size = new Size(5, 5);
                    break;

                default:
                    Open.StartProgress();
                    Size = new Size(238, 35);
                    break;
            }
            CenterToScreen();
        }
        void OnReceiveItem(string item)
        {
            switch (item)
            {
                case quo:
                    if (Array.Exists(XingConnect, o => o.Equals(initial)))
                    {
                        if (Xing != null)
                        {
                            foreach (var ctor in Xing.orders)
                            {
                                if (initial.Equals(trading))
                                    ((IStates<State>)ctor).SendState += Quotes.OnReceiveState;

                                ((IMessage<NotifyIconText>)ctor).SendMessage += OnReceiveNotifyIcon;
                            }
                            for (int i = 0; i < Xing.reals.Length; i++)
                                switch (i)
                                {
                                    case 0:
                                        ((IEvents<EventHandler.XingAPI.Quotes>)Xing.reals[i]).Send += Quotes.OnReceiveQuotes;
                                        continue;

                                    case 1:
                                        if (initial.Equals(trading))
                                            ((ITrends<Trends>)Xing.reals[i]).SendTrend += Quotes.OnReceiveTrend;

                                        continue;

                                    case 2:
                                        Text = XingAPI.ConnectAPI.Code;
                                        continue;

                                    default:
                                        if (initial.Equals(trading))
                                            ((IStates<State>)Xing.reals[i]).SendState += Quotes.OnReceiveState;

                                        continue;
                                }
                            Xing.OnReceiveBalance = true;
                        }
                    }
                    else
                    {
                        Open.SendQuotes += Quotes.OnReceiveQuotes;
                        Open.SendState += Quotes.OnReceiveState;
                        Open.SendTrend += Quotes.OnReceiveTrend;
                    }
                    Size = new Size(323, 493);
                    Quotes.Show();
                    break;

                case ex:
                    Text = secret.GetIdentify();
                    Size = new Size(241, 0);
                    CenterToScreen();
                    Close();
                    return;

                case st:
                    if (Xing != null && Array.Exists(XingConnect, o => o.Equals(initial)))
                        Text = Xing.GetAccountName(Xing.Accounts.Length == 1 ? Xing.Accounts[0] : Array.Find(Xing.Accounts, o => o.Substring(o.Length - 2, 2).Equals("02")));

                    Size = new Size(795, 433);
                    Statistical.Show();
                    break;

                case acc:
                    if (Array.Exists(XingConnect, o => o.Equals(initial)))
                    {
                        if (Xing != null)
                        {
                            Text = (Xing.Accounts.Length == 1 ? Xing.Accounts[0] : Array.Find(Xing.Accounts, o => o.Substring(o.Length - 2, 2).Equals("02"))).Insert(5, "-").Insert(3, "-");
                            var query = Xing.querys[0];
                            ((IEvents<Deposit>)query).Send += Account.OnReceiveDeposit;
                            ((IMessage<NotifyIconText>)query).SendMessage += OnReceiveNotifyIcon;
                            query.QueryExcute();
                        }
                    }
                    else
                    {
                        Open.SendDeposit += Account.OnReceiveDeposit;
                        Open.LookUpTheDeposit(Acc);
                    }
                    Size = new Size(749, 372);
                    Account.Show();
                    break;

                case bal:
                    if (Array.Exists(XingConnect, o => o.Equals(initial)))
                    {
                        if (Xing != null)
                        {
                            Text = Xing.DetailName;
                            var query = Xing.querys[1];
                            ((IEvents<Balance>)query).Send += Balance.OnReceiveBalance;
                            ((IMessage<NotifyIconText>)query).SendMessage += OnReceiveNotifyIcon;
                            query.QueryExcute();
                        }
                    }
                    else
                    {
                        Open.SendBalance += Balance.OnReceiveBalance;
                        Open.LookUpTheBalance(Acc);
                    }
                    Size = new Size(249, 0);
                    Balance.SendReSize += OnReceiveSize;
                    break;
            }
        }
        void OnItemClick(object sender, ToolStripItemClickedEventArgs e)
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
        void OnReceiveSize(object sender, GridResize e)
        {
            var connect = Array.Exists(XingConnect, o => o.Equals(initial));
            Size = new Size(Server ? 591 : (connect ? 604 : 599), e.ReSize + 34);
            Balance.Show();

            if (connect == false)
                Open.SendCurrent += Balance.OnRealTimeCurrentPriceReflect;
        }
        void OnReceiveNotifyIcon(object sender, NotifyIconText e)
        {
            switch (e.NotifyIcon.GetType().Name)
            {
                case dic:
                    var temp = (Dictionary<int, string>)e.NotifyIcon;

                    if (temp.TryGetValue(0, out string code))
                    {
                        notifyIcon.Text = checkDataBase;
                        Open.StartProgress(3605);
                        notifyIcon.Text = secret.GetIdentify();

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
                        var chart = Retrieve.GetInstance(KeyDecoder.GetWindowsProductKeyFromRegistry(), Open.Code).Chart;
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

                case bt:
                    if (Array.Exists(XingConnect, o => o.Equals(initial)))
                        BeginInvoke(new Action(() =>
                        {
                            Xing = XingAPI.ConnectAPI.GetInstance(initial.Equals(trading) ? Strategy.Retrieve.Code : Open.Code, Strategy.Retrieve.Date);
                            Xing.Send += OnReceiveNotifyIcon;
                            notifyIcon.Text = string.Concat("Trading Code_", initial.Equals(trading) ? Strategy.Retrieve.Code : Open.Code);
                            OnEventConnect();
                            OnClickMinimized = quo;
                            Application.DoEvents();
                        }));
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
                    {
                        if (Temporary != null && initial.Equals(collecting))
                            Temporary.SetStorage(Open.Code);

                        Process.Start("shutdown.exe", "-r");
                        Dispose();
                    }
                    else
                    {
                        foreach (var ctor in Xing.querys)
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
                        Statistical.Hide();
                        return;
                    }
                    break;

                case cha:
                    switch ((char)e.NotifyIcon)
                    {
                        case (char)69:
                            new ExceptionMessage(e.NotifyIcon.ToString());
                            Dispose();
                            return;

                        case (char)41:
                            if (initial.Equals(trading))
                            {
                                Xing.OnReceiveBalance = false;
                                Process.Start("shutdown.exe", "-r");
                                Dispose();
                            }
                            break;

                        case (char)21:
                            if (initial.Equals(trading))
                                Xing.OnReceiveBalance = true;

                            break;

                        default:
                            return;
                    }
                    break;

                case boolean:
                    break;
            }
        }
        void OnEventConnect()
        {
            foreach (var ctor in Xing.querys)
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
            foreach (var ctor in Xing.reals)
            {
                switch (ctor.GetType().Name)
                {
                    case fc0:
                    case nc0:
                        if (initial.Equals(trading))
                            ((ITrends<Trends>)ctor).SendTrend += Quotes.OnReceiveTrend;

                        ctor.OnReceiveRealTime(initial.Equals(trading) ? Strategy.Retrieve.Code : Open.Code);
                        break;

                    case fh0:
                    case nh0:
                        if (initial.Equals(collecting))
                            Open.SendQuotes -= Quotes.OnReceiveQuotes;

                        ((IEvents<EventHandler.XingAPI.Quotes>)ctor).Send += Quotes.OnReceiveQuotes;
                        ctor.OnReceiveRealTime(initial.Equals(trading) ? Strategy.Retrieve.Code : Open.Code);
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
                            ctor.OnReceiveRealTime(initial.Equals(trading) ? Strategy.Retrieve.Code : Open.Code);
                        }));
                        break;

                    default:
                        if (initial.Equals(trading))
                        {
                            ((IStates<State>)ctor).SendState += Quotes.OnReceiveState;
                            ctor.OnReceiveRealTime(Strategy.Retrieve.Code);
                        }
                        break;
                }
            }
            if (initial.Equals(trading))
            {
                foreach (var ctor in Xing.orders)
                {
                    ((IMessage<NotifyIconText>)ctor).SendMessage += OnReceiveNotifyIcon;
                    ((IStates<State>)ctor).SendState += Quotes.OnReceiveState;
                }
                Task.Wait();
                Parallel.ForEach(Specify, new Action<Catalog.XingAPI.Specify>((param) => new Strategy.XingAPI.Base(param)));
            }
            WindowState = Xing.SendNotifyIconText((int)Math.Pow((initial.Equals(trading) ? Strategy.Retrieve.Code : Open.Code).Length, 4));

            if ((DateTime.Now.Hour > 16 || DateTime.Now.Hour == 15 && DateTime.Now.Minute > 45) && initial.Equals(collecting))
                Temporary = new XingAPI.Temporary(Xing.reals[0], Xing.reals[1], new StringBuilder(1024), KeyDecoder.GetWindowsProductKeyFromRegistry());
        }
        void GoblinBatFormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(secret.Exit, secret.GetIdentify(), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning).Equals(DialogResult.Cancel))
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;

                return;
            }
            Dispose();
        }
        void GoblinBatResize(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                if (WindowState.Equals(FormWindowState.Minimized))
                {
                    switch (OnClickMinimized)
                    {
                        case quo:
                            if (Array.Exists(XingConnect, o => o.Equals(initial)))
                            {
                                if (initial.Equals(trading))
                                    foreach (var ctor in Xing.orders)
                                    {
                                        ((IStates<State>)ctor).SendState -= Quotes.OnReceiveState;
                                        ((IMessage<NotifyIconText>)ctor).SendMessage -= OnReceiveNotifyIcon;
                                    }
                                for (int i = 0; i < Xing.reals.Length; i++)
                                {
                                    if (i == 2)
                                        continue;

                                    if (i > 2 && initial.Equals(trading))
                                    {
                                        ((IStates<State>)Xing.reals[i]).SendState -= Quotes.OnReceiveState;

                                        continue;
                                    }
                                    if (i == 0)
                                    {
                                        ((IEvents<EventHandler.XingAPI.Quotes>)Xing.reals[i]).Send -= Quotes.OnReceiveQuotes;

                                        continue;
                                    }
                                    if (i == 1 && initial.Equals(trading))
                                    {
                                        ((ITrends<Trends>)Xing.reals[i]).SendTrend -= Quotes.OnReceiveTrend;

                                        continue;
                                    }
                                }
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
                            if (Array.Exists(XingConnect, o => o.Equals(initial)))
                            {
                                var query = Xing.querys[0];
                                ((IEvents<Deposit>)query).Send -= Account.OnReceiveDeposit;
                                ((IMessage<NotifyIconText>)query).SendMessage -= OnReceiveNotifyIcon;
                            }
                            else
                                Open.SendDeposit -= Account.OnReceiveDeposit;

                            Account.Hide();
                            break;

                        case bal:
                            if (Array.Exists(XingConnect, o => o.Equals(initial)))
                            {
                                var query = Xing.querys[1];
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
                    }
                    Opacity = 0.8135;
                    BackColor = Color.FromArgb(121, 133, 130);
                    Visible = false;
                    ShowIcon = false;
                    notifyIcon.Visible = true;
                }
            }));
        }
        char[] XingConnect => new char[]
        {
            collecting,
            trading
        };
        string[] Acc
        {
            get; set;
        }
        string OnClickMinimized
        {
            get; set;
        }
        bool Server
        {
            get; set;
        }
        Task Task
        {
            get; set;
        }
        AccountControl Account
        {
            get; set;
        }
        BalanceControl Balance
        {
            get; set;
        }
        QuotesControl Quotes
        {
            get; set;
        }
        StatisticalAnalysis Statistical
        {
            get; set;
        }
        XingAPI.ConnectAPI Xing
        {
            get; set;
        }
        OpenAPI.ConnectAPI Open
        {
            get; set;
        }
        XingAPI.Temporary Temporary
        {
            get; set;
        }
        Catalog.XingAPI.Specify[] Specify
        {
            get; set;
        }
        readonly char initial;
        readonly Secret secret;
        const char trading = (char)Port.Trading;
        const char collecting = (char)Port.Collecting;
        const string cfobq10500 = "CFOBQ10500";
        const string ccebq10500 = "CCEBQ10500";
        const string cceaq50600 = "CCEAQ50600";
        const string t0441 = "T0441";
        const string fc0 = "FC0";
        const string fh0 = "FH0";
        const string nc0 = "NC0";
        const string nh0 = "NH0";
        const string jif = "JIF";
        const string acc = "account";
        const string quo = "quotes";
        const string bal = "balance";
        const string st = "strategy";
        const string ex = "exit";
        const string dic = "Dictionary`2";
        const string sb = "StringBuilder";
        const string str = "String";
        const string bt = "Byte";
        const string int32 = "Int32";
        const string cha = "Char";
        const string boolean = "Boolean";
        const string checkDataBase = "CheckDataBase";
        const string gs = "GodSword";
    }
}