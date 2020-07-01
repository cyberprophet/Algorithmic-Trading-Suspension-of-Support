using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;
using ShareInvest.GoblinBatControls;
using ShareInvest.Message;
using ShareInvest.Strategy;

namespace ShareInvest
{
    partial class GoblinBat : Form
    {
        internal GoblinBat(char initial, Secret secret, string key, CancellationTokenSource cts, Strategy.Retrieve retrieve)
        {
            var collect = ((char)Port.Collecting).Equals(initial);
            this.key = key;
            this.initial = initial;
            this.secret = secret;
            this.cts = cts;
            this.retrieve = retrieve;
            InitializeComponent();
            Opacity = 0;

            if (collect)
            {
                Open = OpenAPI.ConnectAPI.GetInstance(key, 205);
                Open.SetAPI(axAPI);
                Open.SendCount += OnReceiveNotifyIcon;
            }
            switch (initial)
            {
                case collecting:
                case trading:
                case (char)83:
                    if (Statistical == null)
                    {
                        Statistical = initial.Equals((char)Port.Trading) ? new StatisticalControl(Strategy.Retrieve.Code, secret.strategy, secret.rate, secret.commission) : new StatisticalControl(Strategy.Retrieve.Code, secret.rate, secret.commission);
                        panel.Controls.Add(Statistical);
                        Statistical.Dock = DockStyle.Fill;
                        Statistical.Show();
                    }
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
                        Open.StartProgress(new OpenAPI.Temporary(Open, new StringBuilder(1024), key));
                    }
                    else
                    {
                        Specify = Statistical.Statistics(retrieve.GetUserStrategy());
                        Xing = XingAPI.ConnectAPI.GetInstance(initial, Strategy.Retrieve.Code, Strategy.Retrieve.Date);
                        Xing.Send += OnReceiveNotifyIcon;
                        notifyIcon.Text = string.Concat("Trading Code_", Strategy.Retrieve.Code);
                        Text = Xing.Account;
                        BeginInvoke(new Action(() => OnEventConnect()));
                        OnClickMinimized = quo;
                    }
                    Size = new Size(281, 5);
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
                        foreach (var ctor in Xing.orders)
                        {
                            if (initial.Equals(collecting) == false)
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
                                    if (initial.Equals(collecting) == false)
                                        ((ITrends<Trends>)Xing.reals[i]).SendTrend += Quotes.OnReceiveTrend;

                                    continue;

                                case 2:
                                    Text = XingAPI.ConnectAPI.Code;
                                    continue;

                                default:
                                    if (initial.Equals(collecting) == false)
                                        ((IStates<State>)Xing.reals[i]).SendState += Quotes.OnReceiveState;

                                    continue;
                            }
                        if (initial.Equals(collecting) == false)
                            Quotes.SendState += Xing.OnReceiveOperatingState;
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
                        Text = Xing.AccountName;

                    Size = new Size(1350, 255);
                    Statistical.OnEventConnect();
                    Statistical.SendStatistics += OnReceiveStrategy;
                    Statistical.Show();
                    break;

                case acc:
                    if (Array.Exists(XingConnect, o => o.Equals(initial)))
                    {
                        Text = Xing.Account;
                        var query = Xing.querys[0];
                        ((IEvents<Deposit>)query).Send += Account.OnReceiveDeposit;
                        ((IMessage<NotifyIconText>)query).SendMessage += OnReceiveNotifyIcon;
                        query.QueryExcute();
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
                        Text = Xing.DetailName;
                        var query = Xing.querys[1];
                        ((IEvents<Balance>)query).Send += Balance.OnReceiveBalance;
                        ((IMessage<NotifyIconText>)query).SendMessage += OnReceiveNotifyIcon;
                        query.QueryExcute();
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
        void OnReceiveStrategy(object sender, EventHandler.BackTesting.Statistics e)
        {
            if (string.IsNullOrEmpty(e.Setting.Code) == false && string.IsNullOrEmpty(e.Setting.Strategy) == false && retrieve.SetIdentify(e.Setting) >= 0)
            {
                Cursor = Cursors.Default;
                retrieve.SetStatistics(e.Setting, new List<string>(secret.strategy), secret.rate[0]);

                return;
            }
            if (Chart == null)
            {
                Chart = new ChartControl();
                panel.Controls.Add(Chart);
                Chart.Dock = DockStyle.Fill;
            }
            if (retrieve.OnReceiveRepositoryID(e.Game) == false)
            {
                var message = string.Empty;
                Task = new Task(() => message = new BackTesting((char)86, retrieve.GetImitationModel(e.Game), key).Message);
                Task.Start();

                if (TimerBox.Show(secret.BackTesting, e.Game.Strategy, MessageBoxButtons.OK, MessageBoxIcon.Warning, (uint)15E+3).Equals(DialogResult.OK))
                {
                    Cursor = Cursors.AppStarting;
                    Task.Wait();

                    if (string.IsNullOrEmpty(message) == false && TimerBox.Show(message, e.Game.Strategy, MessageBoxButtons.OK, MessageBoxIcon.Warning, 3251).Equals(DialogResult.OK))
                    {
                        Cursor = Cursors.Default;

                        return;
                    }
                }
            }
            var task = new Task<Dictionary<DateTime, string>>(() => retrieve.OnReceiveInformation(e.Game));
            task.Start();
            Cursor = Cursors.WaitCursor;
            SendInformation += Chart.OnReceiveChartValue;
            task.Wait();
            SuspendLayout();
            SendInformation?.Invoke(this, new EventHandler.BackTesting.Statistics(task.Result));
            Size = Chart.SetChartValue();
            Statistical.Hide();
            Chart.Show();
            ResumeLayout();
            CenterToScreen();
            Cursor = Cursors.Default;
            SendInformation -= Chart.OnReceiveChartValue;
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
                case GoblinBat.array:
                    var array = (string[])e.NotifyIcon;
                    notifyIcon.Text = string.Concat(array[0], array[1]);
                    return;

                case tuple:
                    BeginInvoke(new Action(() => new Strategy.OpenAPI.Consecutive(key, ((Tuple<string, string>)e.NotifyIcon).Item1.Split(';'))));
                    notifyIcon.Text = ((Tuple<string, string>)e.NotifyIcon).Item2.Replace(';', '\n');
                    return;

                case dic:
                    var temp = (Dictionary<int, string>)e.NotifyIcon;

                    if (temp.TryGetValue(0, out string code))
                    {
                        if (secret.GetIsSever(key))
                        {
                            notifyIcon.Text = checkDataBase;
                            Open.StartProgress(3605);
                        }
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
                            Statistical = new StatisticalControl(Strategy.Retrieve.Code, secret.strategy, secret.rate, secret.commission);
                            panel.Controls.Add(Statistical);
                            Statistical.Dock = DockStyle.Fill;
                        }
                        var chart = Retrieve.GetInstance(key, Open.Code).Chart;
                        var check = e.NotifyIcon.ToString().Split((char)59);
                        Acc = new string[check.Length - 3];
                        Server = check[check.Length - 1].Equals(secret.Mock);

                        if (!Server && new VerifyIdentity().Identify(check[check.Length - 3], check[check.Length - 2]) == false)
                        {
                            if (TimerBox.Show(new Secret(check[check.Length - 2]).Identify, secret.GoblinBat, MessageBoxButtons.OK, MessageBoxIcon.Warning, 3750).Equals(DialogResult.OK))
                            {
                                ClosingForm = true;
                                Dispose();
                            }
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
                            Xing = XingAPI.ConnectAPI.GetInstance(initial, initial.Equals(collecting) ? Open.Code : Strategy.Retrieve.Code, Strategy.Retrieve.Date);
                            Xing.Send += OnReceiveNotifyIcon;
                            notifyIcon.Text = string.Concat("Trading Code_", initial.Equals(collecting) ? Open.Code : Strategy.Retrieve.Code);
                            OnEventConnect();
                            OnClickMinimized = quo;
                            Application.DoEvents();

                            if (secret.GetIsSever(key))
                                retrieve.SetInitializeTheChart();
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
                            new ExceptionMessage(((char)e.NotifyIcon).ToString());
                            Dispose();
                            return;

                        case (char)41:
                            if (initial.Equals(collecting) == false && ClosingForm == false)
                            {
                                if (cts == null)
                                    Process.Start("shutdown.exe", "-r");

                                new ExceptionMessage(((char)e.NotifyIcon).ToString());
                                Dispose();
                            }
                            break;

                        case (char)21:
                            new ExceptionMessage(((char)e.NotifyIcon).ToString());
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
                        if (Account == null)
                        {
                            Account = new AccountControl();
                            panel.Controls.Add(Account);
                            Account.Dock = DockStyle.Fill;
                            Account.Show();
                        }
                        ((IEvents<Deposit>)ctor).Send += Account.OnReceiveDeposit;
                        ((IMessage<NotifyIconText>)ctor).SendMessage += OnReceiveNotifyIcon;
                        break;

                    case t0441:
                    case cceaq50600:
                        if (Balance == null)
                        {
                            Balance = new BalanceControl();
                            panel.Controls.Add(Balance);
                            Balance.Dock = DockStyle.Fill;
                        }
                        ((IEvents<Balance>)ctor).Send += Balance.OnReceiveBalance;
                        ((IMessage<NotifyIconText>)ctor).SendMessage += OnReceiveNotifyIcon;
                        break;
                }
                ctor.QueryExcute();
            }
            foreach (var ctor in Xing.reals)
                switch (ctor.GetType().Name)
                {
                    case fc0:
                    case nc0:
                        if (initial.Equals(collecting) == false)
                            ((ITrends<Trends>)ctor).SendTrend += Quotes.OnReceiveTrend;

                        ctor.OnReceiveRealTime(initial.Equals(collecting) ? Open.Code : Strategy.Retrieve.Code);
                        continue;

                    case fh0:
                    case nh0:
                        if (initial.Equals(collecting))
                            Open.SendQuotes -= Quotes.OnReceiveQuotes;

                        ((IEvents<EventHandler.XingAPI.Quotes>)ctor).Send += Quotes.OnReceiveQuotes;
                        ctor.OnReceiveRealTime(initial.Equals(collecting) ? Open.Code : Strategy.Retrieve.Code);
                        continue;

                    case jif:
                        ((IEvents<NotifyIconText>)ctor).Send += OnReceiveNotifyIcon;
                        ctor.OnReceiveRealTime(initial.Equals(collecting) ? Open.Code : Strategy.Retrieve.Code);
                        continue;

                    default:
                        if (initial.Equals(collecting) == false)
                        {
                            ((IStates<State>)ctor).SendState += Quotes.OnReceiveState;
                            ctor.OnReceiveRealTime(Strategy.Retrieve.Code);
                        }
                        continue;
                }
            if (initial.Equals(collecting) == false)
            {
                Opacity = 0;

                foreach (var ctor in Xing.orders)
                {
                    ((IMessage<NotifyIconText>)ctor).SendMessage += OnReceiveNotifyIcon;
                    ((IStates<State>)ctor).SendState += Quotes.OnReceiveState;
                }
                if (initial.Equals(collecting) == false)
                    Parallel.ForEach(Specify, new Action<Catalog.XingAPI.Specify>((param) =>
                    {
                        switch (param.Strategy)
                        {
                            case basic:
                                new Strategy.XingAPI.Base(param);
                                break;

                            case bantam:
                                new Strategy.XingAPI.Bantam(param);
                                break;

                            case feather:
                                new Strategy.XingAPI.Feather(param);
                                break;

                            case fly:
                                new Strategy.XingAPI.Fly(param);
                                break;

                            case sFly:
                                new Strategy.XingAPI.SuperFly(param);
                                break;

                            case heavy:
                                new Strategy.XingAPI.Heavy(param);
                                break;

                            default:
                                if (param.Time > 0)
                                    new Strategy.XingAPI.Consecutive(param);

                                break;
                        }
                    }));
            }
            WindowState = Xing.SendNotifyIconText((int)Math.Pow((initial.Equals(collecting) ? Open.Code : Strategy.Retrieve.Code).Length, 4));

            if ((DateTime.Now.Hour > 16 || DateTime.Now.Hour == 15 && DateTime.Now.Minute > 45) && initial.Equals(collecting))
                Temporary = new XingAPI.Temporary(Xing.reals[0], Xing.reals[1], new StringBuilder(1024), key);
        }
        void GoblinBatFormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(secret.Exit, secret.GetIdentify(), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning).Equals(DialogResult.Cancel))
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;

                return;
            }
            ClosingForm = true;
            strip.ItemClicked -= OnItemClick;
            Dispose();
        }
        void GoblinBatResize(object sender, EventArgs e) => BeginInvoke(new Action(() =>
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                switch (OnClickMinimized)
                {
                    case quo:
                        if (Array.Exists(XingConnect, o => o.Equals(initial)))
                        {
                            foreach (var ctor in Xing.orders)
                            {
                                if (initial.Equals(collecting) == false)
                                    ((IStates<State>)ctor).SendState -= Quotes.OnReceiveState;

                                ((IMessage<NotifyIconText>)ctor).SendMessage -= OnReceiveNotifyIcon;
                            }
                            for (int i = 0; i < Xing.reals.Length; i++)
                            {
                                if (i == 2)
                                    continue;

                                if (i > 2 && initial.Equals(collecting) == false)
                                {
                                    ((IStates<State>)Xing.reals[i]).SendState -= Quotes.OnReceiveState;

                                    continue;
                                }
                                if (i == 0)
                                {
                                    ((IEvents<EventHandler.XingAPI.Quotes>)Xing.reals[i]).Send -= Quotes.OnReceiveQuotes;

                                    continue;
                                }
                                if (i == 1 && initial.Equals(collecting) == false)
                                {
                                    ((ITrends<Trends>)Xing.reals[i]).SendTrend -= Quotes.OnReceiveTrend;

                                    continue;
                                }
                            }
                            if (initial.Equals(collecting) == false)
                                Quotes.SendState -= Xing.OnReceiveOperatingState;
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
                        if (Chart != null)
                            Chart.Hide();

                        Statistical.SendStatistics -= OnReceiveStrategy;
                        Statistical.OnEventDisconnect();
                        Statistical.Hide();
                        break;

                    case chart:
                        Chart.Hide();
                        break;
                }
                Opacity = 0.8135;
                BackColor = Color.FromArgb(121, 133, 130);
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;
            }
        }));
        char[] XingConnect => new char[]
        {
            collecting,
            trading,
            (char)83
        };
        string[] Acc
        {
            get; set;
        }
        string OnClickMinimized
        {
            get; set;
        }
        bool ClosingForm
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
        ChartControl Chart
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
        StatisticalControl Statistical
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
        readonly Strategy.Retrieve retrieve;
        readonly string key;
        readonly char initial;
        readonly Secret secret;
        readonly CancellationTokenSource cts;
        public event EventHandler<EventHandler.BackTesting.Statistics> SendInformation;
    }
}