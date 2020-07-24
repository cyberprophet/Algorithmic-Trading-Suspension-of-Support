using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Client;
using ShareInvest.Controls;
using ShareInvest.EventHandler;

namespace ShareInvest
{
    sealed partial class SecuritiesAPI : Form
    {
        internal SecuritiesAPI(GoblinBatClient client, Privacies privacy, ISecuritiesAPI com)
        {
            this.com = com;
            this.privacy = privacy;
            this.client = client;
            random = new Random();
            InitializeComponent();
            icon = new string[] { mono, duo, tri, quad };
            colors = new Color[] { Color.Maroon, Color.Ivory, Color.DeepSkyBlue };
            infoCodes = new Dictionary<string, Codes>();
            strip.ItemClicked += OnItemClick;
            timer.Start();
        }
        void StartProgress()
        {
            var control = (Control)com;
            Controls.Add(control);
            control.Dock = DockStyle.Fill;
            control.Show();
            Size = new Size(0x177, com is XingAPI.ConnectAPI ? 0x127 : 0xC3);
            Opacity = 0.8135;
            BackColor = Color.FromArgb(0x79, 0x85, 0x82);
            FormBorderStyle = FormBorderStyle.None;
            CenterToScreen();
            com.Send += OnReceiveSecuritiesAPI;
        }
        void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e)
        {
            if (e.Accounts == null && Balance != null)
                BeginInvoke(new Action(async () =>
                {
                    Retention retention;
                    Catalog.XingAPI.ICharts<SendSecuritiesAPI> chart = null;

                    switch (e.Convey)
                    {
                        case string message:
                            Balance.OnReceiveMessage(message);
                            return;

                        case Tuple<string, string, int, dynamic, dynamic, long, double> balance:
                            SuspendLayout();
                            var strategics = string.Empty;

                            switch (com)
                            {
                                case XingAPI.ConnectAPI x when x.Strategics.Count > 0:
                                    strategics = x.Strategics.First(o => o.Code.Equals(balance.Item1)).GetType().Name;
                                    break;

                                case OpenAPI.ConnectAPI o:
                                    break;
                            }
                            Size = new Size(0x3B8, 0x63 + 0x28 + Balance.OnReceiveBalance(balance, strategics));
                            ResumeLayout();
                            return;

                        case long available:
                            Balance.OnReceiveDeposit(available);
                            return;

                        case Tuple<long, long> tuple:
                            Balance.OnReceiveDeposit(tuple);
                            return;

                        case Tuple<int, string> kw:
                            if (com is OpenAPI.ConnectAPI open)
                            {
                                var connect = open.InputValueRqData(optkwFID, kw.Item2, kw.Item1);

                                if (connect != null)
                                    connect.Send += OnReceiveSecuritiesAPI;
                            }
                            return;

                        case Tuple<string, string, string> code:
                            infoCodes[code.Item1] = new Codes
                            {
                                Code = code.Item1,
                                Name = code.Item2,
                                Price = code.Item3
                            };
                            return;

                        case Dictionary<string, Tuple<string, string>> dictionary:
                            var futures = double.MinValue;
                            int index = 0;

                            foreach (var kv in dictionary)
                                if (infoCodes.TryGetValue(kv.Key, out Codes info) && double.TryParse(kv.Value.Item2, out double rate) && com is XingAPI.ConnectAPI xing)
                                {
                                    info.MarginRate = rate * 1e-2;
                                    info.Name = kv.Value.Item1;
                                    infoCodes[kv.Key] = info;
                                    xing.StartProgress(info);

                                    if (kv.Key.Substring(1, 1).Equals("0") && double.TryParse(info.Price, out double price))
                                        futures = price;
                                }
                            foreach (var kv in infoCodes)
                                if (futures > double.MinValue && kv.Key.StartsWith("2") && double.TryParse(kv.Key.Substring(kv.Key.Length - 3), out double oPrice) && oPrice < futures + 0x14 && oPrice > futures - 0x14 && index++ < 0xF && infoCodes.TryGetValue(string.Concat("3", kv.Key.Substring(1)), out Codes codes))
                                {
                                    var option = com as XingAPI.ConnectAPI;
                                    option?.StartProgress(kv.Value);
                                    option?.StartProgress(codes);
                                }
                            return;

                        case Tuple<string[], string[], string[], string[]> tuple:
                            for (int i = 0; i < tuple.Item1.Length; i++)
                                if (int.TryParse(tuple.Item3[i], out int gubun))
                                {
                                    var statusCode = await client.PutContext<Codes>(new Codes
                                    {
                                        Code = tuple.Item1[i],
                                        Name = tuple.Item2[i],
                                        MaturityMarketCap = string.Empty,
                                        MarginRate = gubun,
                                        Price = tuple.Item4[i]
                                    });
                                    SendMessage(statusCode);
                                }
                            return;

                        case Tuple<string, string, string, string> tuple:
                            var statusOptionsCode = await client.PutContext<Codes>(new Codes
                            {
                                Code = tuple.Item1,
                                Name = tuple.Item2,
                                MaturityMarketCap = tuple.Item3,
                                Price = tuple.Item4
                            });
                            SendMessage(statusOptionsCode);
                            return;

                        case Tuple<byte, byte> tuple:
                            switch (tuple)
                            {
                                case Tuple<byte, byte> tp when tp.Item2 == 21:
                                    if (WindowState.Equals(FormWindowState.Minimized))
                                        strip.Items.Find(st, false).First(o => o.Name.Equals(st)).PerformClick();

                                    return;

                                case Tuple<byte, byte> tp when tp.Item2 == 41 && tp.Item1 == 1:
                                    retention = await client.GetContext(new Stocks());
                                    chart = (com as XingAPI.ConnectAPI)?.Stocks;
                                    break;

                                case Tuple<byte, byte> tp when tp.Item2 == 41 && tp.Item1 == 5:
                                    retention = await client.GetContext(new Options());
                                    chart = (com as XingAPI.ConnectAPI)?.Options;
                                    break;
                            }
                            break;

                        case Tuple<string, Stack<string>> charts:
                            switch (charts.Item1.Length)
                            {
                                case 6:
                                    chart = (com as XingAPI.ConnectAPI)?.Stocks;
                                    chart.Send -= OnReceiveSecuritiesAPI;
                                    retention = await client.PostContext(new Catalog.Convert().ToStoreInStocks(charts.Item1, charts.Item2));
                                    break;

                                case 8:
                                    chart = (com as XingAPI.ConnectAPI)?.Options;
                                    chart.Send -= OnReceiveSecuritiesAPI;
                                    retention = await client.PostContext(new Catalog.Convert().ToStoreInOptions(charts.Item1, charts.Item2));
                                    break;
                            }
                            break;
                    }
                    if (string.IsNullOrEmpty(retention.Code) == false && chart != null)
                    {
                        chart.Send += OnReceiveSecuritiesAPI;
                        chart?.QueryExcute(retention);
                    }
                }));
            else if (e.Convey is FormWindowState state)
            {
                WindowState = state;
                com.Send -= OnReceiveSecuritiesAPI;
                ((Control)com).Hide();
                Controls.Add(e.Accounts);
                e.Accounts.Dock = DockStyle.Fill;
                e.Accounts.Show();
                Size = new Size(0x13B, 0x7D);
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                WindowState = FormWindowState.Normal;
                CenterToScreen();

                if (e.Accounts is Accounts accounts)
                    accounts.Send += OnReceiveSecuritiesAPI;
            }
            else if (e.Convey is string str && e.Accounts is Accounts accounts)
            {
                Opacity = 0;
                FormBorderStyle = FormBorderStyle.FixedSingle;
                WindowState = FormWindowState.Minimized;
                strategy.Text = balance;
                accounts.Hide();
                accounts.Send -= OnReceiveSecuritiesAPI;
                var param = str.Split(';');
                var info = com.SetPrivacy(com is OpenAPI.ConnectAPI ? new Privacies { AccountNumber = param[0] } : new Privacies
                {
                    AccountNumber = param[0],
                    AccountPassword = param[1]
                });
                Balance = new Balance(info);
                Controls.Add(Balance);
                Balance.Dock = DockStyle.Fill;
                Text = info.Nick;
                notifyIcon.Text = info.Nick;
                Opacity = 0.79315;
                backgroundWorker.RunWorkerAsync();
            }
        }
        async void BackgroundWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (com is OpenAPI.ConnectAPI api)
            {
                Stack = new Stack<string>();
                double basePrice = double.MinValue;

                foreach (var code in api.InputValueRqData())
                    if (code.Length == 8 && (code.StartsWith("1") || (code.StartsWith("2") || code.StartsWith("3")) && double.TryParse(code.Substring(code.Length - 3), out double oPrice) && oPrice < basePrice + 0x41 && oPrice > basePrice - 0x41))
                    {
                        if (code.StartsWith("101") && double.TryParse((await client.GetContext(new Codes { Code = code })).Price, out double price))
                            basePrice = price;

                        else
                        {
                            Stack.Push(code);
                            api.InputValueRqData(string.Concat(instance, opt50001), code).Send += OnReceiveSecuritiesAPI;
                        }
                    }
            }
            else if (com is XingAPI.ConnectAPI connect)
            {
                foreach (var ctor in connect.ConvertTheCodeToName)
                {
                    ctor.Send += OnReceiveSecuritiesAPI;
                    ctor.QueryExcute();
                }
                if (privacy.CodeStrategics is string cStrategics)
                    foreach (var strategics in cStrategics?.Split(';'))
                    {
                        IStrategics temp = null;
                        var stParam = strategics?.Split('.');

                        if (stParam[0].Length > 0xC)
                        {
                            switch (strategics.Substring(0, 2))
                            {
                                case "TF":
                                    if (int.TryParse(stParam[0].Substring(0xB), out int ds) & int.TryParse(stParam[1], out int dl) & int.TryParse(stParam[2], out int m) & int.TryParse(stParam[3], out int ms) & int.TryParse(stParam[4], out int ml) & int.TryParse(stParam[5], out int rs) & int.TryParse(stParam[6], out int rl) & int.TryParse(stParam[7], out int qs) & int.TryParse(stParam[8], out int ql))
                                        temp = new TrendFollowingBasicFutures
                                        {
                                            Code = strategics.Substring(2, 8),
                                            RollOver = stParam[0].Substring(0xA, 1).Equals("1"),
                                            DayShort = ds,
                                            DayLong = dl,
                                            Minute = m,
                                            MinuteShort = ms,
                                            MinuteLong = ml,
                                            ReactionShort = rs,
                                            ReactionLong = rl,
                                            QuantityShort = qs,
                                            QuantityLong = ql
                                        };
                                    break;
                            }
                            if (temp != null && connect.Strategics.Add(temp) && connect.SetStrategics(temp) > 0)
                                foreach (var real in connect.Reals)
                                    real.OnReceiveRealTime(temp.Code);
                        }
                        else
                        {

                        }
                    }
                foreach (var conclusion in connect.Conclusion)
                    conclusion.OnReceiveRealTime(string.Empty);

                var alarm = connect.JIF;
                alarm.Send += OnReceiveSecuritiesAPI;
                alarm.QueryExcute();
            }
            Connect = int.MaxValue;
        }
        void GoblinBatResize(object sender, EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                SuspendLayout();
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;

                if (strategy.Text.Equals(balance) && Balance != null)
                {
                    Balance.Hide();

                    if (com is OpenAPI.ConnectAPI openAPI)
                    {
                        openAPI.OnConnectErrorMessage.Send -= OnReceiveSecuritiesAPI;
                        openAPI.Send -= OnReceiveSecuritiesAPI;
                        openAPI.InputValueRqData(false, opw00005).Send -= OnReceiveSecuritiesAPI;

                        var connect = openAPI.InputValueRqData(optkwFID, null, 0);

                        if (connect != null)
                            connect.Send -= OnReceiveSecuritiesAPI;

                        foreach (var ctor in openAPI.HoldingStocks)
                        {
                            Balance.SetDisconnectHoldingStock(ctor);
                            ctor.SendBalance -= OnReceiveSecuritiesAPI;
                        }
                    }
                    else if (com is XingAPI.ConnectAPI xing)
                    {
                        foreach (var ctor in xing.HoldingStocks)
                        {
                            Balance.SetDisconnectHoldingStock(ctor);
                            ctor.SendBalance -= OnReceiveSecuritiesAPI;
                        }
                        foreach (var ctor in xing.querys)
                            ctor.Send -= OnReceiveSecuritiesAPI;
                    }
                }
                ResumeLayout();
            }
        }
        void GoblinBatFormClosing(object sender, FormClosingEventArgs e)
        {
            switch (MessageBox.Show(rExit, notifyIcon.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3))
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    WindowState = FormWindowState.Minimized;
                    return;

                case DialogResult.Yes:
                    Invoke(new Action(async () =>
                    {
                        var code = await client.DeleteContext<Privacies>(privacy);

                        if (code > 0xC8)
                        {
                            notifyIcon.Text = OnReceiveErrorMessage(code);
                            e.Cancel = true;
                            WindowState = FormWindowState.Minimized;

                            return;
                        }
                    }));
                    break;
            }
            timer.Stop();
            strip.ItemClicked -= OnItemClick;
            Dispose();
        }
        void TimerTick(object sender, EventArgs e)
        {
            if (com == null)
            {
                timer.Stop();
                strip.ItemClicked -= OnItemClick;
                Dispose();
            }
            else if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) == false)
                WindowState = FormWindowState.Minimized;

            else if (Controls.Contains((Control)com) == false && WindowState.Equals(FormWindowState.Minimized))
                strip.Items.Find(st, false).First(o => o.Name.Equals(st)).PerformClick();

            else if (Visible && ShowIcon && notifyIcon.Visible == false && FormBorderStyle.Equals(FormBorderStyle.None) && WindowState.Equals(FormWindowState.Normal) && (com is XingAPI.ConnectAPI || com is OpenAPI.ConnectAPI))
            {
                var now = DateTime.Now;
                var day = 0;

                switch (now.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        day = now.AddDays(1).Day;
                        break;

                    case DayOfWeek.Saturday:
                        day = now.AddDays(2).Day;
                        break;

                    case DayOfWeek weeks when weeks.Equals(DayOfWeek.Friday) && now.Hour > 8:
                        day = now.AddDays(3).Day;
                        break;

                    default:
                        day = (now.Hour > 8 || Array.Exists(holidays, o => o.Equals(now.ToString(dFormat))) ? now.AddDays(1) : now).Day;
                        break;
                }
                var remain = new DateTime(now.Year, now.Month, day, 9, 0, 0) - DateTime.Now;
                com.SetForeColor(colors[DateTime.Now.Second % 3], GetRemainingTime(remain));

                if (remain.TotalMinutes < 0x1F && com.Start == false && DateTime.Now.Hour == 8 && DateTime.Now.Minute > 0x1E && (Connect > 0x4B0 || random.Next(Connect++, 0x4B1) == 0x4B0))
                    com.StartProgress();
            }
            else if (Visible == false && ShowIcon == false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
                notifyIcon.Icon = (Icon)resources.GetObject(icon[DateTime.Now.Second % 4]);
        }
        void OnItemClick(object sender, ToolStripItemClickedEventArgs e) => BeginInvoke(new Action(() =>
        {
            if (e.ClickedItem.Name.Equals(st))
            {
                SuspendLayout();

                if (strategy.Text.Equals(balance) && Balance != null)
                {
                    if (com is XingAPI.ConnectAPI xingAPI)
                    {
                        foreach (var ctor in xingAPI.querys)
                        {
                            ctor.Send += OnReceiveSecuritiesAPI;
                            ctor.QueryExcute();
                        }
                        foreach (var ctor in xingAPI.HoldingStocks)
                        {
                            Balance.SetConnectHoldingStock(ctor);
                            ctor.SendBalance += OnReceiveSecuritiesAPI;
                        }
                        if (Connect == int.MaxValue)
                            foreach (var convert in xingAPI.ConvertTheCodeToName)
                                convert.Send -= OnReceiveSecuritiesAPI;
                    }
                    else if (com is OpenAPI.ConnectAPI openAPI)
                    {
                        openAPI.OnConnectErrorMessage.Send += OnReceiveSecuritiesAPI;
                        openAPI.Send += OnReceiveSecuritiesAPI;
                        openAPI.InputValueRqData(true, opw00005).Send += OnReceiveSecuritiesAPI;

                        foreach (var ctor in openAPI.HoldingStocks)
                        {
                            Balance.SetConnectHoldingStock(ctor);
                            ctor.SendBalance += OnReceiveSecuritiesAPI;
                        }
                        if (Connect == int.MaxValue)
                        {
                            while (Stack.Count > 0)
                                openAPI.InputValueRqData(opt50001, Stack.Pop()).Send -= OnReceiveSecuritiesAPI;
                        }
                    }
                    Connect = int.MinValue;
                    Size = new Size(0x3B8, 0x63 + 0x28);
                    Balance.Show();
                }
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                WindowState = FormWindowState.Normal;
                ResumeLayout();

                if (com is XingAPI.ConnectAPI xing && xing.API == null || com is OpenAPI.ConnectAPI open && open.API == null)
                    StartProgress();
            }
            else
                Close();
        }));
        [Conditional("DEBUG")]
        void SendMessage(int code)
        {
            if (code > 200)
                Console.WriteLine(code);
        }
        Balance Balance
        {
            get; set;
        }
        Stack<string> Stack
        {
            get; set;
        }
        int Connect
        {
            get; set;
        }
        readonly GoblinBatClient client;
        readonly Random random;
        readonly Dictionary<string, Codes> infoCodes;
        readonly Color[] colors;
        readonly Privacies privacy;
        readonly ISecuritiesAPI com;
        readonly string[] icon;
    }
}