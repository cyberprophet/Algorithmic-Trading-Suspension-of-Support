using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Client;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest
{
    sealed partial class SecuritiesAPI : Form
    {
        internal SecuritiesAPI(GoblinBatClient client, Privacies privacy, ISecuritiesAPI<SendSecuritiesAPI> com)
        {
            this.com = com;
            this.privacy = privacy;
            this.client = client;
            random = new Random();
            stocks = new List<string>();
            futures = new List<string>();
            options = new List<string>();
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
                    int empty = 0;
                    var param = string.Empty;
                    Retention retention;
                    ISecuritiesAPI<SendSecuritiesAPI> securities = null;
                    ICharts<SendSecuritiesAPI> chart = null;

                    switch (e.Convey)
                    {
                        case Tuple<int, string, int, int, string> order:
                            if (com is OpenAPI.ConnectAPI openOrder)
                                openOrder.SendOrder(Info, order);

                            else if (com is XingAPI.ConnectAPI xingOrder)
                            {

                            }
                            return;

                        case string message:
                            Balance.OnReceiveMessage(message);
                            return;

                        case Tuple<string, string, int, dynamic, dynamic, long, double> balance:
                            SuspendLayout();
                            var strategics = string.Empty;

                            switch (com)
                            {
                                case XingAPI.ConnectAPI x when x.Strategics.Count > 0:
                                    securities = x;
                                    break;

                                case OpenAPI.ConnectAPI o when o.Strategics.Count > 0:
                                    securities = o;
                                    break;
                            }
                            if (securities != null && securities.Strategics.Count > 0 && securities.Strategics.Any(o => o.Code.Equals(balance.Item1)))
                                strategics = securities?.Strategics?.First(x => x.Code.Equals(balance.Item1)).GetType().Name;

                            Size = new Size(0x3CD, 0x63 + 0x28 + Balance.OnReceiveBalance(balance, strategics));
                            ResumeLayout();
                            return;

                        case long available:
                            Balance.OnReceiveDeposit(available);
                            return;

                        case Tuple<long, long> tuple:
                            Balance.OnReceiveDeposit(tuple);
                            return;

                        case Tuple<int, string> kw:

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
                            var index = 0;

                            foreach (var kv in dictionary)
                                if (infoCodes.TryGetValue(kv.Key, out Codes info) && double.TryParse(kv.Value.Item2, out double rate) && com is XingAPI.ConnectAPI xing)
                                {
                                    info.MarginRate = rate * 1e-2;
                                    info.Name = kv.Value.Item1;
                                    infoCodes[kv.Key] = info;
                                    xing.StartProgress(info);

                                    if (kv.Key.StartsWith("101") && kv.Key.Length == 8 && kv.Key.EndsWith("000") && double.TryParse(info.Price, out double price))
                                        futures = price;
                                }
                            foreach (var kv in infoCodes)
                                if (futures > double.MinValue && kv.Key.StartsWith("2") && double.TryParse(kv.Key.Substring(kv.Key.Length - 3), out double oPrice) && oPrice < futures + 0x14 && oPrice > futures - 0x14 && index++ < 0xF && infoCodes.TryGetValue(string.Concat("3", kv.Key.Substring(1)), out Codes codes))
                                {
                                    var option = com as XingAPI.ConnectAPI;
                                    option?.StartProgress(kv.Value);
                                    option?.StartProgress(codes);
                                    options.Add(kv.Key);
                                    options.Add(string.Concat("3", kv.Key.Substring(1)));
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
                                        MarginRate = gubun,
                                        Price = tuple.Item4[i]
                                    });
                                    SendMessage(statusCode);
                                    stocks.Add(tuple.Item1[i]);
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
                            if (tuple.Item1.Length == 6)
                                stocks.Add(tuple.Item1);

                            else
                                options.Add(tuple.Item1);

                            SendMessage(statusOptionsCode);
                            return;

                        case Tuple<byte, byte> tuple:
                            switch (tuple)
                            {
                                case Tuple<byte, byte> tp when tp.Item1 == 1 && tp.Item2 == 0x15 && com is XingAPI.ConnectAPI || com is OpenAPI.ConnectAPI && tp.Item1 == 3 && tp.Item2 == 9:
                                    if (WindowState.Equals(FormWindowState.Minimized))
                                        strip.Items.Find(st, false).First(o => o.Name.Equals(st)).PerformClick();

                                    return;

                                case Tuple<byte, byte> tp when tp.Item2 == 0x29 && tp.Item1 == 1 && com is XingAPI.ConnectAPI || com is OpenAPI.ConnectAPI && tp.Item1 == 8 && tp.Item2 == 0x58:
                                    retention = await SelectStocksCodeAsync();
                                    chart = (com as XingAPI.ConnectAPI)?.Stocks;
                                    param = opt10079;
                                    break;

                                case Tuple<byte, byte> tp when tp.Item2 == 41 && tp.Item1 == 5 && com is XingAPI.ConnectAPI:
                                    retention = await SelectOptionsCodeAsync();
                                    chart = (com as XingAPI.ConnectAPI)?.Options;
                                    break;

                                case Tuple<byte, byte> tp when tp.Item1 == 0x65 && tp.Item2 == 0xF && com is OpenAPI.ConnectAPI:
                                    retention = await client.GetContext(SelectFuturesCode);
                                    param = opt50028;
                                    break;

                                default:
                                    return;
                            }
                            if (WindowState.Equals(FormWindowState.Minimized) == false && Info.Account.Equals("S"))
                                WindowState = FormWindowState.Minimized;

                            break;

                        case Tuple<string, Stack<string>> charts:
                            switch (charts.Item1.Length)
                            {
                                case 6:
                                    if (com is XingAPI.ConnectAPI xs)
                                    {
                                        chart = xs?.Stocks;
                                        chart.Send -= OnReceiveSecuritiesAPI;
                                    }
                                    else if (com is OpenAPI.ConnectAPI os)
                                    {
                                        os.InputValueRqData(opt10079, charts.Item1).Send -= OnReceiveSecuritiesAPI;
                                        param = opt10079;
                                    }
                                    retention = await client.PostContext((await SelectStocksCodeAsync()).Code, new Catalog.Convert().ToStoreInStocks(charts.Item1, charts.Item2));

                                    break;

                                case int length when length == 8 && (charts.Item1.StartsWith("101") || charts.Item1.StartsWith("106")):
                                    (com as OpenAPI.ConnectAPI).InputValueRqData(opt50028, charts.Item1).Send -= OnReceiveSecuritiesAPI;
                                    param = opt50028;

                                    if (this.futures.Remove(charts.Item1))
                                        retention = await client.PostContext(SelectFuturesCode, new Catalog.Convert().ToStoreInFutures(charts.Item1, charts.Item2));

                                    break;

                                case int length when length == 8 && (charts.Item1.StartsWith("2") || charts.Item1.StartsWith("3")):
                                    switch (com)
                                    {
                                        case XingAPI.ConnectAPI xo:
                                            chart = xo?.Options;
                                            chart.Send -= OnReceiveSecuritiesAPI;
                                            break;

                                        case OpenAPI.ConnectAPI o:
                                            o.InputValueRqData(opt50066, charts.Item1).Send -= OnReceiveSecuritiesAPI;
                                            param = opt50066;
                                            break;
                                    }
                                    retention = await client.PostContext((await SelectOptionsCodeAsync()).Code, new Catalog.Convert().ToStoreInOptions(charts.Item1, charts.Item2));

                                    if (com is XingAPI.ConnectAPI && (string.IsNullOrEmpty(retention.Code) || retention.Code.Equals(noMatch)))
                                        return;

                                    else if (stocks.Count == 0 && this.futures.Count == 0 && options.Count == 0)
                                    {
                                        Dispose(WindowState);

                                        return;
                                    }
                                    break;
                            }
                            while (retention.Code == null && retention.LastDate == null)
                            {
                                switch (empty)
                                {
                                    case 0:
                                        if (com is OpenAPI.ConnectAPI)
                                            param = opt10079;

                                        else if (com is XingAPI.ConnectAPI xs)
                                            chart = xs?.Stocks;

                                        if (stocks.Count > 0)
                                            retention = await SelectStocksCodeAsync();

                                        break;

                                    case 1:
                                        if (com is OpenAPI.ConnectAPI && this.futures.Count > 0)
                                        {
                                            retention = await client.GetContext(SelectFuturesCode);
                                            param = opt50028;
                                        }
                                        break;

                                    case 2:
                                        if (com is OpenAPI.ConnectAPI)
                                            param = opt50066;

                                        else if (com is XingAPI.ConnectAPI xo)
                                            chart = xo?.Options;

                                        if (options.Count > 0)
                                            retention = await SelectOptionsCodeAsync();

                                        break;

                                    case 3:
                                        Dispose(WindowState);
                                        return;
                                }
                                if (stocks.Count == 0 && this.futures.Count == 0 && options.Count == 0)
                                    Dispose(WindowState);

                                else if (retention.LastDate == null)
                                    empty++;

                                else
                                    break;
                            }
                            break;
                    }
                    if (string.IsNullOrEmpty(retention.Code) == false && retention.Code.Equals(noMatch) == false)
                        switch (com)
                        {
                            case OpenAPI.ConnectAPI o when string.IsNullOrEmpty(param) == false:
                                o.InputValueRqData(string.Concat(instance, param), string.Concat(retention.Code, ";", retention.LastDate)).Send += OnReceiveSecuritiesAPI;
                                return;

                            case XingAPI.ConnectAPI _ when chart != null:
                                chart.Send += OnReceiveSecuritiesAPI;
                                chart?.QueryExcute(retention);
                                return;
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
                Info = com.SetPrivacy(com is OpenAPI.ConnectAPI ? new Privacies { AccountNumber = param[0] } : new Privacies
                {
                    AccountNumber = param[0],
                    AccountPassword = param[1]
                });
                Balance = new Balance(Info);
                Controls.Add(Balance);
                Balance.Dock = DockStyle.Fill;
                Text = Info.Nick;
                notifyIcon.Text = Info.Nick;
                Opacity = 0.79315;
                backgroundWorker.RunWorkerAsync();
                OnReceiveData(MessageBox.Show("This is a Temporary Code.", "Emergency", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2));
            }
        }
        async void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            ISecuritiesAPI<SendSecuritiesAPI> connect = null;
            var catalog = new Dictionary<string, IStrategics>();

            if (privacy.CodeStrategics is string cStrategics)
                foreach (var strategics in cStrategics?.Split(';'))
                {
                    var stParam = strategics?.Split('.');

                    if (Enum.TryParse(strategics.Substring(0, 2), out Strategics initial))
                        switch (initial)
                        {
                            case Strategics.TF:
                                if (int.TryParse(stParam[0].Substring(0xB), out int ds) & int.TryParse(stParam[1], out int dl) & int.TryParse(stParam[2], out int m) & int.TryParse(stParam[3], out int ms) & int.TryParse(stParam[4], out int ml) & int.TryParse(stParam[5], out int rs) & int.TryParse(stParam[6], out int rl) & int.TryParse(stParam[7], out int qs) & int.TryParse(stParam[8], out int ql))
                                    catalog[strategics.Substring(2, 8)] = new TrendFollowingBasicFutures
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

                            case Strategics.TS:
                                if (char.TryParse(stParam[stParam.Length - 1], out char setting) && char.TryParse(stParam[8], out char tTrend) && char.TryParse(stParam[7], out char longShort) && int.TryParse(stParam[6], out int quoteUnit) && int.TryParse(stParam[5], out int quantity) && double.TryParse(stParam[4].Insert(stParam[4].Length - 2, "."), out double additionalPurchase) && double.TryParse(stParam[3].Insert(stParam[3].Length - 2, "."), out double realizeProfit) && int.TryParse(stParam[2], out int trend) && int.TryParse(stParam[1], out int l) && int.TryParse(stParam[0].Substring(8), out int s))
                                    catalog[strategics.Substring(2, 6)] = new TrendsInStockPrices
                                    {
                                        Code = strategics.Substring(2, 6),
                                        Short = s,
                                        Long = l,
                                        Trend = trend,
                                        RealizeProfit = realizeProfit,
                                        AdditionalPurchase = additionalPurchase,
                                        Quantity = quantity,
                                        QuoteUnit = quoteUnit,
                                        LongShort = (LongShort)longShort,
                                        TrendType = (Trend)tTrend,
                                        Setting = (Setting)setting
                                    };
                                break;
                        }
                }
            switch (com)
            {
                case OpenAPI.ConnectAPI o:
                    Stack = new Stack<string>();
                    double basePrice = double.MinValue;

                    foreach (var code in o.InputValueRqData())
                        if ((code.Length == 8 && (code.StartsWith("2") || code.StartsWith("3")) && double.TryParse(code.Substring(code.Length - 3), out double oPrice) && (oPrice > basePrice + 0x41 || oPrice < basePrice - 0x41)) == false)
                        {
                            if (code.Length == 8 && code.StartsWith("106"))
                                futures.Add(code);

                            else if (code.Length == 8 && code.StartsWith("101") && double.TryParse((await client.GetContext(new Codes { Code = code })).Price, out double price))
                            {
                                basePrice = price;
                                futures.Add(code);
                            }
                            Stack.Push(code);
                            o.InputValueRqData(string.Concat(instance, code.Length == 8 ? opt50001 : optkwFID), code).Send += OnReceiveSecuritiesAPI;
                        }
                    o.ConnectChapterOperation.Send += OnReceiveSecuritiesAPI;
                    connect = o;
                    break;

                case XingAPI.ConnectAPI x:
                    foreach (var ctor in x.ConvertTheCodeToName)
                    {
                        ctor.Send += OnReceiveSecuritiesAPI;
                        ctor.QueryExcute();
                    }
                    if (catalog.Any(o => o.Key.Length == 8))
                        foreach (var conclusion in x.Conclusion)
                            conclusion.OnReceiveRealTime(string.Empty);

                    var alarm = x.JIF;
                    alarm.Send += OnReceiveSecuritiesAPI;
                    alarm.QueryExcute();
                    connect = x;
                    break;
            }
            foreach (var kv in catalog)
                if (connect.Strategics.Add(kv.Value) && connect?.SetStrategics(kv.Value) > 0)
                    switch (kv.Key.Length)
                    {
                        case int length when length == 8 && (kv.Key.StartsWith("101") || kv.Key.StartsWith("106")):
                            foreach (var real in (connect as XingAPI.ConnectAPI)?.Reals)
                                real.OnReceiveRealTime(kv.Key);

                            break;
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
                    int code = 0;
                    Invoke(new Action(async () => code = await client.DeleteContext<Privacies>(privacy)));

                    if (code > 0xC8)
                        SendMessage(OnReceiveErrorMessage(code));

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
                int day = 0, today = DateTime.DaysInMonth(now.Year, now.Month);

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
                var remain = new DateTime(now.Year, now.Day == today ? now.AddMonths(1).Month : now.Month, day, 9, 0, 0) - DateTime.Now;
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
                            while (Stack.Count > 0)
                            {
                                var pop = Stack.Pop();
                                openAPI.InputValueRqData(pop.Length == 8 ? opt50001 : optkwFID, pop).Send -= OnReceiveSecuritiesAPI;
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
        string SelectFuturesCode
        {
            get
            {
                var code = string.Empty;

                while (futures.Count > 0 && string.IsNullOrEmpty(code))
                    code = futures[random.Next(0, futures.Count)];

                return code;
            }
        }
        async Task<Retention> SelectOptionsCodeAsync()
        {
            if (options.Count > 0)
            {
                var retention = await client.GetContext(options[random.Next(0, options.Count)]);

                if (options.Remove(retention.Code))
                {
                    if (string.IsNullOrEmpty(retention.LastDate) == false && retention.LastDate.Substring(0, 6).Equals(DateTime.Now.ToString("yyMMdd")))
                        return await SelectOptionsCodeAsync();

                    else if (string.IsNullOrEmpty(retention.Code) == false && retention.LastDate == null)
                        return await SelectOptionsCodeAsync();

                    else
                        return retention;
                }
                else
                    return await SelectOptionsCodeAsync();
            }
            else
                return new Retention { Code = null, LastDate = null };
        }
        async Task<Retention> SelectStocksCodeAsync()
        {
            if (stocks.Count > 0)
            {
                var retention = await client.GetContext(stocks[random.Next(0, stocks.Count)]);

                if (stocks.Remove(retention.Code))
                {
                    if (string.IsNullOrEmpty(retention.LastDate) == false && retention.LastDate.Substring(0, 6).Equals(DateTime.Now.ToString("yyMMdd")))
                        return await SelectStocksCodeAsync();

                    else if ((await client.GetContext(new Codes { Code = retention.Code })).MaturityMarketCap.Contains("거래정지"))
                        return await SelectStocksCodeAsync();

                    else if (string.IsNullOrEmpty(retention.Code) == false && retention.LastDate == null)
                        return await SelectStocksCodeAsync();

                    else
                        return retention;
                }
                else
                    return await SelectStocksCodeAsync();
            }
            else
                return new Retention { Code = null, LastDate = null };
        }
        void Dispose(FormWindowState state)
        {
            if (state.Equals(FormWindowState.Minimized) == false)
                WindowState = FormWindowState.Minimized;

            switch (com)
            {
                case OpenAPI.ConnectAPI o:
                    o.ConnectChapterOperation.Send -= OnReceiveSecuritiesAPI;
                    break;

                case XingAPI.ConnectAPI x:
                    x.JIF.Send -= OnReceiveSecuritiesAPI;
                    break;
            }
            if (timer.Enabled)
                timer.Stop();

            strip.ItemClicked -= OnItemClick;
            Dispose();
        }
        [Conditional("DEBUG")]
        void SendMessage(object code)
        {
            if (code is int response && response > 200)
                Console.WriteLine(response);

            else if (code is string str)
                Console.WriteLine(str);
        }
        [Conditional("DEBUG")]
        void OnReceiveData(DialogResult result) => BeginInvoke(new Action(async () =>
        {
            if (result.Equals(DialogResult.OK))
                switch (com)
                {
                    case OpenAPI.ConnectAPI o:
                        var param = opt50028;
                        var retention = await client.GetContext(SelectFuturesCode);
                        o.InputValueRqData(string.Concat(instance, param), string.Concat(retention.Code, ";", retention.LastDate)).Send += OnReceiveSecuritiesAPI;
                        return;

                    case XingAPI.ConnectAPI x:
                        var chart = x?.Stocks;
                        chart.Send += OnReceiveSecuritiesAPI;
                        chart?.QueryExcute(await SelectStocksCodeAsync());
                        return;
                }
        }));
        Balance Balance
        {
            get; set;
        }
        Stack<string> Stack
        {
            get; set;
        }
        IAccountInformation Info
        {
            get; set;
        }
        int Connect
        {
            get; set;
        }
        readonly List<string> futures;
        readonly List<string> options;
        readonly List<string> stocks;
        readonly GoblinBatClient client;
        readonly Random random;
        readonly Dictionary<string, Codes> infoCodes;
        readonly Color[] colors;
        readonly Privacies privacy;
        readonly ISecuritiesAPI<SendSecuritiesAPI> com;
        readonly string[] icon;
    }
}