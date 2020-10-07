using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Client;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Interface.XingAPI;
using ShareInvest.Message;

namespace ShareInvest
{
    sealed partial class SecuritiesAPI : Form
    {
        internal SecuritiesAPI(Consensus consensus, GoblinBatClient client, Privacies privacy, ISecuritiesAPI<SendSecuritiesAPI> com)
        {
            this.com = com;
            this.privacy = privacy;
            this.consensus = consensus;
            this.client = client;
            random = new Random();
            stocks = new List<string>();
            futures = new List<string>();
            options = new List<string>();
            InitializeComponent();
            icon = new string[] { mono, duo, tri, quad };
            colors = new Color[] { Color.Maroon, Color.Ivory, Color.DeepSkyBlue };
            GetTheCorrectAnswer = new int[privacy.Security.Length];
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
                        case Tuple<int, string, int, int, string> order when com is OpenAPI.ConnectAPI oOrder:
                            oOrder.SendOrder(Info, order);
                            return;

                        case Tuple<string, int, string, string, int, string, string> order when com is OpenAPI.ConnectAPI oOrderFO:
                            oOrderFO.SendOrder(Info, order);
                            return;

                        case Catalog.XingAPI.Order order when com is XingAPI.ConnectAPI xOrder:
                            xOrder.Orders[string.IsNullOrEmpty(order.OrgOrdNo) ? 0 : (string.IsNullOrEmpty(order.BnsTpCode) && string.IsNullOrEmpty(order.OrdPrc) && string.IsNullOrEmpty(order.FnoOrdprcPtnCode) ? 2 : 1)].QueryExcute(order);
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

                        case bool connect:
                            var query = (com as XingAPI.ConnectAPI)?.Querys[1];

                            if (connect)
                            {
                                query.Send += OnReceiveSecuritiesAPI;
                                query.QueryExcute();
                            }
                            else
                                query.Send -= OnReceiveSecuritiesAPI;

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

                        case Tuple<string, string, string, string, int> tuple:
                            var statusOptionsCode = await client.PutContext<Codes>(new Codes
                            {
                                Code = tuple.Item1,
                                Name = tuple.Item2,
                                MaturityMarketCap = tuple.Item3,
                                Price = tuple.Item4,
                                MarginRate = tuple.Item5
                            });
                            if (tuple.Item1.Length == 6)
                                stocks.Add(tuple.Item1);

                            else
                                options.Add(tuple.Item1);

                            SendMessage(statusOptionsCode);
                            return;

                        case Tuple<string, Stack<Catalog.OpenAPI.RevisedStockPrice>> tuple:
                            while (tuple.Item2 != null && tuple.Item2.Count > 0)
                            {
                                var info = tuple.Item2.Pop();

                                if (await client.PostContext(info) == 0xC8)
                                    SendMessage(string.Concat(info.Name, " ", info.Date, " ", info.Revise, " ", info.Price, " ", info.Rate));
                            }
                            var axAPI = com as OpenAPI.ConnectAPI;
                            axAPI.InputValueRqData(opt10081, tuple.Item1).Send -= OnReceiveSecuritiesAPI;

                            if (axAPI.Count < 0x3B7 && DateTime.Now.Minute < 0x31)
                            {
                                retention = await SelectDaysCodeAsync();
                                axAPI.InputValueRqData(string.Concat(instance, opt10081), string.Concat(retention.Code, ';', retention.LastDate)).Send += OnReceiveSecuritiesAPI;
                            }
                            else
                            {
                                Stocks.Clear();
                                Stocks = null;
                            }
                            return;

                        case short error:
                            switch (error)
                            {
                                case -0x6A:
                                    Dispose(WindowState);
                                    return;
                            }
                            return;

                        case Tuple<byte, byte> tuple:
                            switch (tuple)
                            {
                                case Tuple<byte, byte> tp when (tp.Item1 == 1 || tp.Item1 == 5 || tp.Item1 == 7) && tp.Item2 == 0x15 && com is XingAPI.ConnectAPI || com is OpenAPI.ConnectAPI && (tp.Item1 == 0 && tp.Item2 == 8 || tp.Item1 == 3 && tp.Item2 == 9):
                                    if (WindowState.Equals(FormWindowState.Minimized))
                                        strip.Items.Find(st, false).First(o => o.Name.Equals(st)).PerformClick();

                                    return;

                                case Tuple<byte, byte> tp when tp.Item2 == 0x29 && tp.Item1 == 1 && com is XingAPI.ConnectAPI || com is OpenAPI.ConnectAPI && tp.Item1 == 8 && tp.Item2 == 0x58:
                                    retention = await SelectStocksCodeAsync();
                                    chart = (com as XingAPI.ConnectAPI)?.Stocks;
                                    param = opt10079;
                                    break;

                                case Tuple<byte, byte> tp when tp.Item2 == 0x29 && tp.Item1 == 5 && com is XingAPI.ConnectAPI:
                                    retention = await SelectOptionsCodeAsync();
                                    chart = (com as XingAPI.ConnectAPI)?.Options;
                                    break;

                                case Tuple<byte, byte> tp when tp.Item1 == 0x65 && tp.Item2 == 0xF && com is OpenAPI.ConnectAPI:
                                    retention = await client.GetContext(SelectFuturesCode);
                                    param = opt50028;
                                    break;

                                case Tuple<byte, byte> tp when tp.Item2 == 0x19 && tp.Item1 == 7 && com is XingAPI.ConnectAPI || tp.Item1 == 0x64 && tp.Item2 > 0x10 && com is OpenAPI.ConnectAPI:
                                    Dispose(WindowState);
                                    return;

                                default:
                                    GetSettleTheFare();
                                    return;
                            }
                            if (WindowState.Equals(FormWindowState.Minimized) == false && ((tuple.Item1 == 0x65 && tuple.Item2 == 0xF || tuple.Item1 == 5 && tuple.Item2 == 0x29) && Info.Name.Equals("선물옵션") || Info.Name.Equals("위탁종합") && (tuple.Item1 == 8 && tuple.Item2 == 0x58 || tuple.Item1 == 1 && tuple.Item2 == 0x29)))
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
                Opacity = 0.79315;
                notifyIcon.Text = loading;
                backgroundWorker.RunWorkerAsync(Info.Nick);
                OnReceiveData(MessageBox.Show("This is a Temporary Code.", "Emergency", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2));
            }
        }
        async void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            ISecuritiesAPI<SendSecuritiesAPI> connect = null;
            var catalog = new Dictionary<string, IStrategics>();

            if (privacy.CodeStrategics is string cStrategics)
                foreach (var strategics in cStrategics?.Split(';'))
                {
                    var stParam = strategics?.Split(strategics[2].Equals('|') ? '|' : '.');
                    var code = string.Empty;

                    if (Enum.TryParse(strategics.Substring(0, 2), out Strategics initial))
                        switch (initial)
                        {
                            case Strategics.TV:
                                if (stParam.Length == 0x11 && int.TryParse(stParam[2], out int vShort) && int.TryParse(stParam[3], out int vLong) && int.TryParse(stParam[4], out int vTrend) && int.TryParse(stParam[5], out int su) && int.TryParse(stParam[6], out int sq) && double.TryParse(stParam[7], out double vSubtraction) && int.TryParse(stParam[8], out int au) && int.TryParse(stParam[9], out int aq) && double.TryParse(stParam[0xA], out double vAddition) && int.TryParse(stParam[0xB], out int si) && int.TryParse(stParam[0xC], out int tsq) && double.TryParse(stParam[0xD], out double sp) && int.TryParse(stParam[0xE], out int ai) && int.TryParse(stParam[0xF], out int taq) && double.TryParse(stParam[0x10], out double ap))
                                {
                                    code = stParam[1];
                                    catalog[code] = new TrendsInValuation
                                    {
                                        Code = stParam[1],
                                        Short = vShort,
                                        Long = vLong,
                                        Trend = vTrend,
                                        SubtractionalUnit = su,
                                        ReservationSubtractionalQuantity = sq,
                                        Subtraction = vSubtraction * 1e-2,
                                        AdditionalUnit = au,
                                        ReservationAddtionalQuantity = aq,
                                        Addition = vAddition * 1e-2,
                                        SubtractionalInterval = si,
                                        TradingSubtractionalQuantity = tsq,
                                        SubtractionalPosition = sp * 1e-2,
                                        AddtionalInterval = ai,
                                        TradingAddtionalQuantity = taq,
                                        AdditionalPosition = ap * 1e-2
                                    };
                                    Balance.ToolTipDictionary[code] = string.Concat("Short_", stParam[2], "\r\nLong_", stParam[3], "\r\nTrend_", stParam[4], "\r\n☞매도예약\r\n호가단위_", stParam[5], "틱\r\n예약수량_", stParam[6], "주\r\n수익실현_", stParam[7], "%\r\n☞매수예약\r\n호가단위_", stParam[8], "틱\r\n예약수량_", stParam[9], "주\r\n추가매수_", stParam[0xA], "%\r\n☞매도\r\n매매간격_", stParam[0xB], "초\r\n매매수량_", stParam[0xC], "주\r\n수익실현_", stParam[0xD], "%\r\n☞매수\r\n매매간격_", stParam[0xE], "초\r\n매매수량_", stParam[0xF], "주\r\n추가매수_", stParam[0x10], "%");
                                }
                                break;

                            case Strategics.TC:
                                if (stParam.Length == 0xD && double.TryParse(stParam[0xC], out double cpAddition) && double.TryParse(stParam[0xB], out double cpRevenue) && int.TryParse(stParam[0xA], out int ctQuantity) && int.TryParse(stParam[9], out int cInterval) && double.TryParse(stParam[8], out double cAddition) && double.TryParse(stParam[7], out double crRevenue) && int.TryParse(stParam[6], out int crQuantity) && int.TryParse(stParam[5], out int cUnit) && int.TryParse(stParam[4], out int cTrend) && int.TryParse(stParam[3], out int cLong) && int.TryParse(stParam[2], out int cShort))
                                {
                                    code = stParam[1];
                                    catalog[code] = new TrendToCashflow
                                    {
                                        Code = stParam[1],
                                        Short = cShort,
                                        Long = cLong,
                                        Trend = cTrend,
                                        Unit = cUnit,
                                        ReservationQuantity = crQuantity,
                                        ReservationRevenue = crRevenue * 1e-2,
                                        Addition = cAddition * 1e-2,
                                        Interval = cInterval,
                                        TradingQuantity = ctQuantity,
                                        PositionRevenue = cpRevenue * 1e-2,
                                        PositionAddition = cpAddition * 1e-2
                                    };
                                    Balance.ToolTipDictionary[code] = string.Concat("Short_", stParam[2], "\r\nLong_", stParam[3], "\r\nTrend_", stParam[4], "\r\n호가단위_", stParam[5], "틱\r\n예약수량_", stParam[6], "주\r\n수익실현_", stParam[7], "%\r\n추가매수_", stParam[8], "%\r\n매매간격_", stParam[9], "초\r\n매매수량_", stParam[10], "주\r\n수익실현_", stParam[11], "%\r\n추가매수_", stParam[12], "%");
                                }
                                break;

                            case Strategics.TF:
                                if (int.TryParse(stParam[0].Substring(0xB), out int ds) & int.TryParse(stParam[1], out int dl) & int.TryParse(stParam[2], out int m) & int.TryParse(stParam[3], out int ms) & int.TryParse(stParam[4], out int ml) & int.TryParse(stParam[5], out int rs) & int.TryParse(stParam[6], out int rl) & int.TryParse(stParam[7], out int qs) & int.TryParse(stParam[8], out int ql))
                                {
                                    code = strategics.Substring(2, 8);
                                    catalog[strategics.Substring(2, 8)] = new TrendFollowingBasicFutures
                                    {
                                        Code = code,
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
                                    Balance.ToolTipDictionary[code] = string.Concat("RollOver_", stParam[0].Substring(0xA, 1).Equals("1"), "\r\nDayShort_", ds, "\r\nDayLong_", dl, "\r\nMinute_", m, "\r\nMinuteShort_", ms, "\r\nMinuteLong_", ml, "\r\nShortReaction_", rs, "\r\nLongReaction_", rl, "\r\nShortQuantity_", qs, "\r\nLongQuantity_", ql);
                                }
                                break;

                            case Strategics.TS:
                                if (char.TryParse(stParam[stParam.Length - 1], out char setting) && char.TryParse(stParam[8], out char tTrend) && char.TryParse(stParam[7], out char longShort) && int.TryParse(stParam[6], out int quoteUnit) && int.TryParse(stParam[5], out int quantity) && double.TryParse(stParam[4].Insert(stParam[4].Length - 2, "."), out double additionalPurchase) && double.TryParse(stParam[3].Insert(stParam[3].Length - 2, "."), out double realizeProfit) && int.TryParse(stParam[2], out int trend) && int.TryParse(stParam[1], out int l) && int.TryParse(stParam[0].Substring(8), out int s))
                                {
                                    code = strategics.Substring(2, 6);
                                    catalog[strategics.Substring(2, 6)] = new TrendsInStockPrices
                                    {
                                        Code = code,
                                        Short = s,
                                        Long = l,
                                        Trend = trend,
                                        RealizeProfit = realizeProfit * 0.01,
                                        AdditionalPurchase = additionalPurchase * 0.01,
                                        Quantity = quantity,
                                        QuoteUnit = quoteUnit,
                                        LongShort = (LongShort)longShort,
                                        TrendType = (Trend)tTrend,
                                        Setting = (Setting)setting
                                    };
                                    Balance.ToolTipDictionary[code] = string.Concat("Short_", s, "\r\nLong_", l, "\r\nTrend_", trend, "\r\nRealize_", (realizeProfit * 0.01).ToString("P2"), "\r\nAddition_", (additionalPurchase * 0.01).ToString("P2"), "\r\nQuantity_", quantity, "\r\nQuoteUnit_", quoteUnit, "\r\nLongShort_", Enum.GetName(typeof(LongShort), longShort), "\r\nTrendType_", Enum.GetName(typeof(Trend), tTrend), "\r\nSetting_", Enum.GetName(typeof(Setting), setting));
                                }
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

                    foreach (var ctor in x.Orders)
                        ctor.Send += OnReceiveSecuritiesAPI;

                    var alarm = x.JIF;
                    alarm.Send += OnReceiveSecuritiesAPI;
                    alarm.QueryExcute();
                    connect = x;
                    break;
            }
            foreach (var kv in catalog.OrderByDescending(o => o.Key))
                if (connect.Strategics.Add(kv.Value) && connect?.SetStrategics(kv.Value) > 0)
                    switch (kv.Key.Length)
                    {
                        case int length when length == 8 && (kv.Key.StartsWith("101") || kv.Key.StartsWith("106")):
                            if (connect is XingAPI.ConnectAPI xing)
                                foreach (var real in xing.Reals)
                                    real.OnReceiveRealTime(kv.Key);

                            break;
                    }
            if (com is OpenAPI.ConnectAPI api && DateTime.Now.Hour == 8 && TimerBox.Show(info, si, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, string.IsNullOrEmpty(privacy.CodeStrategics) ? 0x1FAC7U : (uint)(catalog.Count * 0x4BAF)).Equals(DialogResult.OK))
            {
                Stocks = new Stack<string>(GetRevisedStockPrices(stocks));
                var retention = await SelectDaysCodeAsync();
                (api?.InputValueRqData(string.Concat(instance, opt10081), string.Concat(retention.Code, ';', retention.LastDate))).Send += OnReceiveSecuritiesAPI;
            }
            Connect = int.MaxValue;
            UserName = e.Argument as string;
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

                        if (Info.Name.Equals(fo))
                        {
                            openAPI.InputValueRqData(false, opw20010).Send -= OnReceiveSecuritiesAPI;
                            openAPI.InputValueRqData(false, opw20007).Send -= OnReceiveSecuritiesAPI;
                        }
                        else
                            openAPI.InputValueRqData(false, opw00005).Send -= OnReceiveSecuritiesAPI;

                        foreach (var ctor in openAPI.HoldingStocks)
                        {
                            Balance.SetDisconnectHoldingStock(ctor);
                            ctor.SendBalance -= OnReceiveSecuritiesAPI;
                            ctor.WaitOrder = false;
                        }
                    }
                    else if (com is XingAPI.ConnectAPI xing)
                    {
                        foreach (var ctor in xing.HoldingStocks)
                        {
                            Balance.SetDisconnectHoldingStock(ctor);
                            ctor.SendBalance -= OnReceiveSecuritiesAPI;
                            ctor.WaitOrder = false;
                        }
                        foreach (var ctor in xing.Querys)
                            ctor.Send -= OnReceiveSecuritiesAPI;
                    }
                }
                ResumeLayout();
            }
        }
        void GoblinBatFormClosing(object sender, FormClosingEventArgs e)
        {
            GetSettleTheFare();

            switch (MessageBox.Show(rExit, notifyIcon.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3))
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    WindowState = FormWindowState.Minimized;
                    Application.DoEvents();

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
            {
                for (int i = 0; i < GetTheCorrectAnswer.Length; i++)
                {
                    var temp = 1 + random.Next(Connect, 0x4B0) * (i + 1);
                    GetTheCorrectAnswer[i] = temp < 0x4B1 ? temp : 0x4B0 - i;
                }
                WindowState = FormWindowState.Minimized;
            }
            else if (Controls.Contains((Control)com) == false && WindowState.Equals(FormWindowState.Minimized))
                strip.Items.Find(st, false).First(o => o.Name.Equals(st)).PerformClick();

            else if (Visible && ShowIcon && notifyIcon.Visible == false && FormBorderStyle.Equals(FormBorderStyle.None) && WindowState.Equals(FormWindowState.Normal) && (com is XingAPI.ConnectAPI || com is OpenAPI.ConnectAPI))
            {
                var now = DateTime.Now;

                switch (now.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        if (now.Hour < 3 && now.Minute > 15)
                        {
                            foreach (var process in Process.GetProcessesByName(program, Dns.GetHostName()))
                                process.Kill();

                            Process.Start(shut, "-r");
                            Dispose(WindowState);

                            return;
                        }
                        now = now.AddDays(1);
                        break;

                    case DayOfWeek.Saturday:
                        now = now.AddDays(2);
                        break;

                    case DayOfWeek weeks when weeks.Equals(DayOfWeek.Friday) && now.Hour > 8:
                        now = now.AddDays(3);
                        break;

                    default:
                        now = now.Hour > 8 || Array.Exists(holidays, o => o.Equals(now.ToString(dFormat))) ? now.AddDays(1) : now;
                        break;
                }
                var remain = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0) - DateTime.Now;
                com.SetForeColor(colors[DateTime.Now.Second % 3], GetRemainingTime(remain));

                if (remain.TotalMinutes < 0x1F && com.Start == false && DateTime.Now.Hour == 8 && DateTime.Now.Minute > 0x1E && (Connect > 0x4B0 || Array.Exists(GetTheCorrectAnswer, o => o == random.Next(Connect++, 0x4B2))))
                    com.StartProgress();
            }
            else if (Visible == false && ShowIcon == false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
            {
                notifyIcon.Icon = (Icon)resources.GetObject(icon[DateTime.Now.Second % 4]);

                if (string.IsNullOrEmpty(UserName) == false)
                    notifyIcon.Text = UserName;
            }
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
                        foreach (var ctor in xingAPI.Querys)
                        {
                            ctor.Send += OnReceiveSecuritiesAPI;
                            ctor.QueryExcute();
                        }
                        foreach (var ctor in xingAPI.HoldingStocks)
                        {
                            Balance.SetConnectHoldingStock(ctor);
                            ctor.SendBalance += OnReceiveSecuritiesAPI;
                            ctor.WaitOrder = true;
                        }
                        if (Connect == int.MaxValue)
                            foreach (var convert in xingAPI.ConvertTheCodeToName)
                                convert.Send -= OnReceiveSecuritiesAPI;
                    }
                    else if (com is OpenAPI.ConnectAPI openAPI)
                    {
                        openAPI.OnConnectErrorMessage.Send += OnReceiveSecuritiesAPI;
                        openAPI.Send += OnReceiveSecuritiesAPI;

                        if (Info.Name.Equals(fo))
                        {
                            openAPI.InputValueRqData(true, opw20010).Send += OnReceiveSecuritiesAPI;
                            openAPI.InputValueRqData(true, opw20007).Send += OnReceiveSecuritiesAPI;
                        }
                        else
                            openAPI.InputValueRqData(true, opw00005).Send += OnReceiveSecuritiesAPI;

                        foreach (var ctor in openAPI.HoldingStocks)
                        {
                            Balance.SetConnectHoldingStock(ctor);
                            ctor.SendBalance += OnReceiveSecuritiesAPI;
                            ctor.WaitOrder = true;
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
        IEnumerable<string> GetRevisedStockPrices(List<string> list)
        {
            int index, start, end;
            var stocks = new Stack<string>();

            switch (DateTime.Now.Second % 5)
            {
                case 0:
                    start = 0;
                    end = 0x3E8;
                    break;

                case 1:
                    start = 0x2EE;
                    end = 0x6D6;
                    break;

                case 2:
                    start = 0x5DC;
                    end = 0x9C4;
                    break;

                case 3:
                    start = 0;
                    end = list.Count;
                    break;

                default:
                    start = list.Count - 0x3E9;
                    end = list.Count;
                    break;
            }
            for (index = start; index < end; index++)
                stocks.Push(list[index]);

            return stocks.OrderBy(o => Guid.NewGuid());
        }
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

                    else if (com is XingAPI.ConnectAPI && string.IsNullOrEmpty(retention.Code) == false && string.IsNullOrEmpty(retention.LastDate))
                        return await SelectStocksCodeAsync();

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
                var now = DateTime.Now;

                if (stocks.Remove(retention.Code))
                {
                    if (string.IsNullOrEmpty(retention.LastDate) == false && retention.LastDate.Substring(0, 6).Equals(now.ToString("yyMMdd")))
                        return await SelectStocksCodeAsync();

                    else if ((await client.GetContext(new Codes { Code = retention.Code })).MaturityMarketCap.Contains("거래정지"))
                        return await SelectStocksCodeAsync();

                    else if (string.IsNullOrEmpty(retention.Code) == false && retention.LastDate == null)
                        return await SelectStocksCodeAsync();

                    else if (com is XingAPI.ConnectAPI && string.IsNullOrEmpty(retention.Code) == false && string.IsNullOrEmpty(retention.LastDate))
                        return await SelectStocksCodeAsync();

                    else
                    {
                        try
                        {
                            if (consensus.GrantAccess)
                            {
                                Queue<ConvertConsensus> queue;
                                Queue<Catalog.Request.FinancialStatement> context = null;

                                for (int i = 0; i < retention.Code.Length / 3; i++)
                                {
                                    queue = await consensus.GetContextAsync(i, retention.Code);
                                    int status = int.MinValue;

                                    if (queue != null && queue.Count > 0)
                                    {
                                        status = await client.PostContext(queue);

                                        if (i == 0)
                                            context = new Summary(privacy.Security).GetContext(retention.Code);

                                        if (i == 1 && context != null)
                                            status = await client.PostContext(context);
                                    }
                                    SendMessage(status);
                                }
                            }
                            else if (await client.GetContext(new Catalog.Request.IncorporatedStocks { Market = 'P' }) is int next && random.Next(0, await client.PostContext(new IncorporatedStocks(privacy.Security).OnReceiveSequentially(next))) == 0)
                                await new Advertise(privacy.Security).StartAdvertisingInTheDataCollectionSection(now);
                        }
                        catch (Exception ex)
                        {
                            SendMessage(ex.StackTrace);
                            GC.Collect();
                        }
                        if (string.IsNullOrEmpty(UserName) == false && notifyIcon.Text.Equals(UserName))
                        {
                            notifyIcon.Text = collecting;
                            UserName = string.Empty;
                        }
                        return retention;
                    }
                }
                else
                    return await SelectStocksCodeAsync();
            }
            else
                return new Retention { Code = null, LastDate = null };
        }
        async Task<Retention> SelectDaysCodeAsync()
        {
            if (Stocks.Count > 0)
            {
                var code = Stocks.Pop();
                SendMessage(string.Concat(Stocks.Count + "/" + stocks.Count + "\tCode_" + code));
                var date = DateTime.Now;

                if (string.IsNullOrEmpty(code) == false)
                    return new Retention
                    {
                        Code = code,
                        LastDate = (date.Hour < 6 ? date.AddDays(-1) : date).ToString("yyyyMMdd")
                    };
                else
                    return await SelectDaysCodeAsync();
            }
            else
                return new Retention();
        }
        void Dispose(FormWindowState state)
        {
            GetSettleTheFare();

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
            if (code is int response && response > 0xC8)
                Console.WriteLine(response);

            else if (code is string str)
                Console.WriteLine(str);
        }
        [Conditional("DEBUG")]
        void OnReceiveData(DialogResult result) => BeginInvoke(new Action(async () =>
        {
            if (result.Equals(DialogResult.Yes))
                switch (com)
                {
                    case OpenAPI.ConnectAPI o:
                        var retention = await client.GetContext(SelectFuturesCode);
                        o.InputValueRqData(string.Concat(instance, opt50028), string.Concat(retention.Code, ";", retention.LastDate)).Send += OnReceiveSecuritiesAPI;
                        return;

                    case XingAPI.ConnectAPI x:
                        var chart = x?.Stocks;
                        chart.Send += OnReceiveSecuritiesAPI;
                        chart?.QueryExcute(await SelectStocksCodeAsync());
                        return;
                }
            else if (result.Equals(DialogResult.Cancel))
            {
                var retention = await SelectDaysCodeAsync();
                ((com as OpenAPI.ConnectAPI)?.InputValueRqData(string.Concat(instance, opt10081), string.Concat(retention.Code, ';', retention.LastDate))).Send += OnReceiveSecuritiesAPI;
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
        Stack<string> Stocks
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
        string UserName
        {
            get; set;
        }
        readonly List<string> futures;
        readonly List<string> options;
        readonly List<string> stocks;
        readonly GoblinBatClient client;
        readonly Random random;
        readonly Consensus consensus;
        readonly Dictionary<string, Codes> infoCodes;
        readonly Color[] colors;
        readonly Privacies privacy;
        readonly ISecuritiesAPI<SendSecuritiesAPI> com;
        readonly string[] icon;
    }
}