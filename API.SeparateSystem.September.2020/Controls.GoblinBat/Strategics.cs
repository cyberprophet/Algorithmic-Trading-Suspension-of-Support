using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Client;
using ShareInvest.EventHandler;
using ShareInvest.FindByName;
using ShareInvest.Interface;

namespace ShareInvest.Controls
{
    public partial class Strategics : UserControl
    {
        Privacies Privacy
        {
            get; set;
        }
        string GetQuarter(DateTime dt)
        {
            switch (dt.Month)
            {
                case int month when month > 0 && month < 4:
                    return string.Concat(dt.Year, ".03");

                case int month when month > 3 && month < 7:
                    return string.Concat(dt.Year, ".06");

                case int month when month > 6 && month < 0xA:
                    return string.Concat(dt.Year, ".09");

                case int month when month > 9 && month < 0xD:
                case 0:
                    return string.Concat(dt.Year, ".12");
            }
            return null;
        }
        bool CheckDayOfWeek(DateTime now)
        {
            switch (now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                case DayOfWeek.Saturday when now.Hour > 0xF:
                case DayOfWeek.Monday when now.Hour < 0x11:
                    return linkTerms != null;
            }
            return true;
        }
        void DataSortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index > 1 && double.TryParse(e.CellValue1.ToString().Replace("%", string.Empty), out double x) && double.TryParse(e.CellValue2.ToString().Replace("%", string.Empty), out double y))
            {
                e.SortResult = x.CompareTo(y);
                e.Handled = true;
            }
        }
        void WorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument is Tuple<List<Codes>, IEnumerable<Catalog.Request.Consensus>> tuple)
                foreach (var cs in tuple.Item2)
                {
                    var next = cs.TheNextYear * 0.85;
                    var trends = new int[] { 0xF0, 0x1E0, 0x2D0 };

                    if (cs.FirstQuarter < cs.SecondQuarter && cs.SecondQuarter < cs.ThirdQuarter && cs.ThirdQuarter < cs.Quarter && next > cs.FirstQuarter && next > cs.SecondQuarter && next > cs.ThirdQuarter && next > cs.Quarter)
                        foreach (var trend in trends)
                            SendSize?.Invoke(this, new SendHoldingStocks(tuple.Item1.First(o => o.Code.Equals(cs.Code)).Price, new Catalog.TrendsInStockPrices
                            {
                                Code = cs.Code,
                                Short = 5,
                                Long = 0x3C,
                                Trend = trend,
                                RealizeProfit = 325e-3,
                                AdditionalPurchase = 105e-3,
                                Quantity = 1,
                                QuoteUnit = 1,
                                LongShort = LongShort.Day,
                                TrendType = Trend.Day,
                                Setting = Setting.Both
                            }));
                }
        }
        void OnReceiveCellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
                BeginInvoke(new Action(async () => await disclosure.GetDisclosureInformation(data.Rows[e.RowIndex].Cells[0].Value.ToString(), data.Rows[e.RowIndex].Cells[1].Value.ToString())));
        }
        void InitializeComponent(Stack<Catalog.Request.Consensus> stack, int count, List<Codes> codes)
        {
            var now = DateTime.Now;
            data.CellContentDoubleClick -= OnReceiveCellContentDoubleClick;
            SuspendLayout();

            if (linkTerms != null)
            {
                linkTerms.Dispose();
                InitializeDataGridView();
                linkTerms = null;
            }
            if (data.Rows.Count == 0)
            {
                data.ColumnCount = 8;
                data.BackgroundColor = Color.FromArgb(0x79, 0x85, 0x82);
                data.Columns[0].Name = "종목코드";
                data.Columns[1].Name = "종목명";
            }
            else
                data.Rows.Clear();

            for (count = 2; count < data.ColumnCount; count++)
                data.Columns[count].Name = GetQuarter(now.AddMonths(count == 2 ? 1 : ((count - 2) * 3) + 1));

            while (stack.Count > 0)
            {
                var pop = stack.Pop();
                var index = data.Rows.Add(new string[] { pop.Code, codes.First(o => o.Code.Equals(pop.Code)).Name, Math.Abs(pop.FirstQuarter).ToString("P2"), Math.Abs(pop.SecondQuarter).ToString("P2"), Math.Abs(pop.ThirdQuarter).ToString("P2"), Math.Abs(pop.Quarter).ToString("P2"), Math.Abs(pop.TheNextYear).ToString("P2"), Math.Abs(pop.TheYearAfterNext).ToString("P2") });
                data.Rows[index].Cells[2].Style.ForeColor = pop.FirstQuarter > 0 ? Color.Maroon : Color.Navy;
                data.Rows[index].Cells[2].Style.SelectionForeColor = pop.FirstQuarter > 0 ? Color.FromArgb(0xB9062F) : Color.DeepSkyBlue;
                data.Rows[index].Cells[3].Style.ForeColor = pop.SecondQuarter > 0 ? Color.Maroon : Color.Navy;
                data.Rows[index].Cells[3].Style.SelectionForeColor = pop.SecondQuarter > 0 ? Color.FromArgb(0xB9062F) : Color.DeepSkyBlue;
                data.Rows[index].Cells[4].Style.ForeColor = pop.ThirdQuarter > 0 ? Color.Maroon : Color.Navy;
                data.Rows[index].Cells[4].Style.SelectionForeColor = pop.ThirdQuarter > 0 ? Color.FromArgb(0xB9062F) : Color.DeepSkyBlue;
                data.Rows[index].Cells[5].Style.ForeColor = pop.Quarter > 0 ? Color.Maroon : Color.Navy;
                data.Rows[index].Cells[5].Style.SelectionForeColor = pop.Quarter > 0 ? Color.FromArgb(0xB9062F) : Color.DeepSkyBlue;
                data.Rows[index].Cells[6].Style.ForeColor = pop.TheNextYear > 0 ? Color.Maroon : Color.Navy;
                data.Rows[index].Cells[6].Style.SelectionForeColor = pop.TheNextYear > 0 ? Color.FromArgb(0xB9062F) : Color.DeepSkyBlue;
                data.Rows[index].Cells[7].Style.ForeColor = pop.TheYearAfterNext > 0 ? Color.Maroon : Color.Navy;
                data.Rows[index].Cells[7].Style.SelectionForeColor = pop.TheYearAfterNext > 0 ? Color.FromArgb(0xB9062F) : Color.DeepSkyBlue;
            }
            data.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            data.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            data.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            data.ScrollBars = ScrollBars.Vertical;
            data.AutoResizeRows();
            data.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            ResumeLayout();
            data.CellContentDoubleClick += OnReceiveCellContentDoubleClick;
        }
        public Strategics(dynamic client, dynamic disclosure)
        {
            InitializeComponent();
            buttonChart.Click += OnReceiveClickItem;
            buttonSave.Click += OnReceiveClickItem;
            buttonProgress.Click += OnReceiveClickItem;
            textCode.Leave += OnReceiveClickItem;
            Codes = new HashSet<Codes>();
            data = new DataGridView();
            this.client = client;
            this.disclosure = disclosure;

            foreach (var str in button)
                string.Concat("button", str).FindByName<Button>(this).MouseLeave += ButtonPreviewKeyDown;
        }
        public event EventHandler<SendHoldingStocks> SendSize;
        public void SetDataGridView(Catalog.TrendsInStockPrices ts, uint price, double trend)
        {
            if (price < trend)
            {
                var color = Color.Gainsboro;

                switch (ts.Trend)
                {
                    case 0xF0:
                        color = Color.PapayaWhip;
                        break;

                    case 0x1E0:
                        color = Color.Moccasin;
                        break;

                    case 0x2D0:
                        color = Color.Wheat;
                        break;

                    default:
                        return;
                }
                data.Rows.Cast<DataGridViewRow>().First(o => o.Cells[0].Value.ToString().Equals(ts.Code)).DefaultCellStyle.BackColor = color;
            }
        }
        public void CheckForSurvival(Color color) => labelProgress.ForeColor = color;
        public void SetProgressRate(Catalog.Request.Consensus consensus) => BeginInvoke(new Action(async () =>
        {
            var stack = new Stack<Catalog.Request.Consensus>();
            var list = await client.GetContext(consensus);

            if (list != null && list.Count > 0)
            {
                foreach (var context in list.OrderByDescending(o => o.Code))
                    if (string.IsNullOrEmpty(context.Code) == false && context.Code.Length == 6)
                        stack.Push(context);

                if (stack.Count > 0)
                    InitializeComponent(stack, 0, await client.GetContext(new Codes { }, 6) as List<Codes>);
            }
        }));
        public void SetProgressRate(int rate)
        {
            if (rate > 0 && rate == progressBar.Value && rate % 0x19 == 0 && CheckDayOfWeek(DateTime.Now))
                BeginInvoke(new Action(async () =>
                {
                    var count = 0;
                    var code = string.Empty;
                    var codes = await client.GetContext(new Codes { }, 6) as List<Codes>;
                    var temp = new double[6];
                    var stack = new Stack<Catalog.Request.Consensus>();
                    var price = 0U;

                    foreach (var consensus in (await client.GetContext(new Catalog.Request.Consensus { })).OrderByDescending(o => o.Code))
                    {
                        if (string.IsNullOrEmpty(code) || code.Equals(consensus.Code) == false)
                        {
                            if (string.IsNullOrEmpty(code) == false)
                                stack.Push(new Catalog.Request.Consensus
                                {
                                    Code = code,
                                    FirstQuarter = (temp[0] / count - price) / price,
                                    SecondQuarter = (temp[1] / count - price) / price,
                                    ThirdQuarter = (temp[2] / count - price) / price,
                                    Quarter = (temp[3] / count - price) / price,
                                    TheNextYear = (temp[4] / count - price) / price,
                                    TheYearAfterNext = (temp[5] / count - price) / price
                                });
                            temp = new double[6];
                            code = consensus.Code;
                            count = 0;

                            if (uint.TryParse(codes.First(o => o.Code.Equals(code)).Price, out uint current))
                                price = current;
                        }
                        temp[0] += price * (1 + consensus.FirstQuarter);
                        temp[1] += price * (1 + consensus.SecondQuarter);
                        temp[2] += price * (1 + consensus.ThirdQuarter);
                        temp[3] += price * (1 + consensus.Quarter);
                        temp[4] += price * (1 + consensus.TheNextYear);
                        temp[5] += price * (1 + consensus.TheYearAfterNext);
                        count++;
                    }
                    if (Length == 8)
                    {
                        var cap = new Disclosure(Privacy.Security, 0x4B);
                        var page = 1;
                        var list = await cap.GetMarketCap(0x50, page++);

                        foreach (var param in codes)
                            if (Codes.Add(new Codes
                            {
                                Code = param.Code,
                                Name = param.Name,
                                Price = param.Price
                            }))
                            {

                            }
                    }
                    worker.RunWorkerAsync(new Tuple<List<Codes>, IEnumerable<Catalog.Request.Consensus>>(codes, stack.OrderByDescending(o => o.TheNextYear)));
                    InitializeComponent(stack, count, codes);
                }));
            progressBar.Value = rate + 1;
        }
        public void SetProgressRate(Color color) => buttonProgress.ForeColor = color;
        public void SetProgressRate(bool state)
        {
            if (state)
            {
                buttonProgress.Text = start;
                progressBar.Value = 0;
            }
            else if (data.Rows.Count > 0)
            {
                SuspendLayout();
                data.AutoResizeRows();
                data.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                ResumeLayout();
            }
        }
        public void SetProgressRate()
        {
            buttonProgress.Text = cancel;
            buttonProgress.ForeColor = Color.Maroon;
        }
        public async Task<string> SetPrivacy(Privacies privacy)
        {
            Privacy = privacy;
            string str = "도깨비방망이";

            if (string.IsNullOrEmpty(privacy.Account) == false)
            {
                switch (privacy.Account)
                {
                    case "F":
                        if (comboCommission.Items.Count == 0 && comboStrategics.Items.Count == 0)
                        {
                            foreach (var commission in commissionFutures)
                                comboCommission.Items.Add(commission.ToString("P4"));

                            comboStrategics.Items.AddRange(new object[] { tf });
                        }
                        str = "선물옵션";
                        Length = 8;
                        comboCommission.SelectedIndex = Array.FindIndex(commissionFutures, o => o == privacy.Commission);
                        break;

                    case "S":
                        if (comboCommission.Items.Count == 0 && comboStrategics.Items.Count == 0)
                        {
                            foreach (var commission in commissionStocks)
                                comboCommission.Items.Add(commission.ToString("P3"));

                            comboStrategics.Items.AddRange(new object[] { tc, st, ts });
                        }
                        str = "위탁종합";
                        Length = 6;
                        comboCommission.SelectedIndex = Array.FindIndex(commissionStocks, o => o == privacy.Commission);
                        break;
                }
                if (await client.GetContext(new Codes { }, Length) is List<Codes> list)
                {
                    var stack = new Stack<Codes>();

                    foreach (var item in list)
                        if (FilteringItems(item))
                            stack.Push(item);

                    string[] codes = new string[stack.Count], names = new string[stack.Count];

                    while (stack.Count > 0)
                    {
                        var temp = stack.Pop();

                        if (Codes.Add(temp))
                        {
                            codes[stack.Count] = temp.Code;
                            names[stack.Count] = temp.Name;
                        }
                    }
                    var source = new AutoCompleteStringCollection();
                    source.AddRange(codes);
                    source.AddRange(names);
                    textCode.AutoCompleteCustomSource = source;
                    textCode.AutoCompleteMode = AutoCompleteMode.Append;
                    textCode.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
                if (string.IsNullOrEmpty(privacy.CodeStrategics) == false)
                {
                    SuspendLayout();

                    foreach (var strategics in privacy.CodeStrategics.Split(';'))
                    {
                        var color = Color.FromArgb(0x79, 0x85, 0x82);
                        var code = strategics.Substring(strategics[2].Equals('|') ? 3 : 2, Length);
                        var select = Codes.FirstOrDefault(o => o.Code.Equals(code));
                        tab.Controls.Add(new TabPage
                        {
                            BackColor = Color.Transparent,
                            BorderStyle = BorderStyle.FixedSingle,
                            ForeColor = Color.Black,
                            Location = new Point(4, 0x22),
                            Margin = new Padding(0),
                            Name = code,
                            TabIndex = Index++,
                            Text = code,
                            ToolTipText = strategics.Substring(0, 2)
                        });
                        switch (strategics.Substring(0, 2))
                        {
                            case string tf when tf.Equals("TF") && Length == 8:
                                if (select.MarginRate > 0 && string.IsNullOrEmpty(select.Price) == false)
                                {
                                    var view = new TrendFollowingBasicFutures(select);
                                    tab.TabPages[tab.TabPages.Count - 1].Controls.Add(view);
                                    view.Dock = DockStyle.Fill;
                                    view.TransmuteStrategics(strategics.Substring(10, 1).Equals("1"), strategics.Substring(11).Split('.'));
                                    panel.RowStyles[0].Height = 0x83 + 0x23;
                                }
                                break;

                            case string ts when ts.Equals("TS") && Length == 6:
                                if (string.IsNullOrEmpty(select.MaturityMarketCap) == false && string.IsNullOrEmpty(select.Price) == false)
                                {
                                    var view = new TrendsInStockPrices(select);
                                    tab.TabPages[tab.TabPages.Count - 1].Controls.Add(view);
                                    view.Dock = DockStyle.Fill;
                                    view.TransmuteStrategics(strategics.Substring(8).Split('.'));
                                    panel.RowStyles[0].Height = 0xCD + 0x23;
                                }
                                break;

                            case string tc when tc.Equals("TC") && Length == 6:
                                if (string.IsNullOrEmpty(select.MaturityMarketCap) == false && string.IsNullOrEmpty(select.Price) == false)
                                {
                                    var view = new TrendToCashflow(select);
                                    tab.TabPages[tab.TabPages.Count - 1].Controls.Add(view);
                                    view.Dock = DockStyle.Fill;
                                    view.TransmuteStrategics(strategics.Substring(0xA).Split('|'));
                                    panel.RowStyles[0].Height = 0xEB + 0x23;
                                }
                                break;
                        }
                        var lasttabrect = tab.GetTabRect(tab.TabPages.Count - 1);
                        tab.TabPages[tab.TabPages.Count - 1].BackColor = color;
                        tab.SelectTab(tab.TabPages.Count - 1);
                        tab.CreateGraphics().FillRectangle(new SolidBrush(color), new RectangleF(lasttabrect.X + lasttabrect.Width + tab.Left, tab.Top + lasttabrect.Y, tab.Width - (lasttabrect.X + lasttabrect.Width), lasttabrect.Height));
                    }
                    ResumeLayout();
                }
            }
            return str;
        }
        bool FilteringItems(Codes codes)
        {
            switch (codes.Code.Length)
            {
                case 6:
                    var market = codes.MaturityMarketCap;

                    return market.StartsWith("증거금") && market.Contains("거래정지") == false;

                case 8:
                    return codes.MaturityMarketCap.Length == 6 && string.Compare(DateTime.Now.ToString(format), codes.MaturityMarketCap) <= 0;
            }
            return false;
        }
        void OnReceiveClickItem(object sender, EventArgs e) => BeginInvoke(new Action(async () =>
        {
            if (sender is TextBox)
            {
                var empty = Codes.FirstOrDefault(o => o.Name.Equals(textCode.Text) || o.Code.Equals(textCode.Text) || o.Name.ToUpper().Equals(textCode.Text));
                textCode.Text = string.IsNullOrEmpty(empty.Code) ? string.Empty : empty.Code;
            }
            else if (sender is LinkLabel link)
            {
                var url = @"https://www.youtube.com/c/TenbaggercyberprophetsStock";

                if (link.Name.Equals(this.link.Name) && comboStrategics.SelectedItem is string str)
                    switch (str)
                    {
                        case tc:
                            url = @"https://www.youtube.com/c/TenbaggercyberprophetsStock";
                            break;

                        case tf:
                            url = @"https://www.youtube.com/c/TenbaggercyberprophetsStock";
                            break;

                        case ts:
                            url = @"https://www.youtube.com/c/TenbaggercyberprophetsStock";
                            break;

                        case st:
                            url = @"https://www.youtube.com/c/TenbaggercyberprophetsStock";
                            break;
                    }
                else if (link.Name.Equals(linkTerms.Name))
                    url = @"https://sharecompany.tistory.com/46";

                Process.Start(url);
                link.LinkVisited = true;
            }
            else if (sender is Button button && textCode.Text.Length > 0 && comboStrategics.SelectedItem != null && comboCommission.SelectedItem != null)
            {
                var bfn = button.Name.FindByName<Button>(this);
                var str = comboCommission.SelectedItem.ToString();

                if (buttonChart.Name.Equals(bfn.Name))
                {
                    var check = buttonChart.ForeColor.Equals(Color.Ivory);
                    var tabPage = Controls.Find(tab.SelectedTab.Name, true).First();
                    string[] stParam;
                    SendHoldingStocks verify = null;

                    foreach (var con in tabPage.Controls)
                        switch (con)
                        {
                            case TrendFollowingBasicFutures tf:
                                check = tf.TransmuteStrategics();
                                stParam = tf.TransmuteStrategics(tabPage.Text).Split('.');

                                if (check && int.TryParse(stParam[0].Substring(0xB), out int ds) & int.TryParse(stParam[1], out int dl) & int.TryParse(stParam[2], out int m) & int.TryParse(stParam[3], out int ms) & int.TryParse(stParam[4], out int ml) & int.TryParse(stParam[5], out int rs) & int.TryParse(stParam[6], out int rl) & int.TryParse(stParam[7], out int qs) & int.TryParse(stParam[8], out int ql))
                                    verify = new SendHoldingStocks(new Catalog.TrendFollowingBasicFutures
                                    {
                                        Code = stParam[0].Substring(2, 8),
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
                                    });
                                break;

                            case TrendsInStockPrices ts:
                                check = ts.TransmuteStrategics();
                                stParam = ts.TransmuteStrategics(tabPage.Text).Split('.');

                                if (check && char.TryParse(stParam[stParam.Length - 1], out char setting) && char.TryParse(stParam[8], out char tTrend) && char.TryParse(stParam[7], out char longShort) && int.TryParse(stParam[6], out int quoteUnit) && int.TryParse(stParam[5], out int quantity) && double.TryParse(stParam[4].Insert(stParam[4].Length - 2, "."), out double additionalPurchase) && double.TryParse(stParam[3].Insert(stParam[3].Length - 2, "."), out double realizeProfit) && int.TryParse(stParam[2], out int trend) && int.TryParse(stParam[1], out int l) && int.TryParse(stParam[0].Substring(8), out int s))
                                    verify = new SendHoldingStocks(new Catalog.TrendsInStockPrices
                                    {
                                        Code = stParam[0].Substring(2, 6),
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
                                    });
                                break;

                            case TrendToCashflow tc:
                                check = tc.TransmuteStrategics();
                                stParam = tc.TransmuteStrategics(tabPage.Text)?.Split('|');

                                if (check && stParam.Length == 0xD && double.TryParse(stParam[0xC], out double cpAddition) && double.TryParse(stParam[0xB], out double cpRevenue) && int.TryParse(stParam[0xA], out int ctQuantity) && int.TryParse(stParam[9], out int cInterval) && double.TryParse(stParam[8], out double cAddition) && double.TryParse(stParam[7], out double crRevenue) && int.TryParse(stParam[6], out int crQuantity) && int.TryParse(stParam[5], out int cUnit) && int.TryParse(stParam[4], out int cTrend) && int.TryParse(stParam[3], out int cLong) && int.TryParse(stParam[2], out int cShort))
                                    verify = new SendHoldingStocks(new Catalog.TrendToCashflow
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
                                        PositionAddition = cpAddition * 1e-2,
                                        AnalysisType = tc.CheckValues
                                    });
                                break;

                            case ScenarioAccordingToTrend st:
                                check = st.TransmuteStrategics();
                                var item = st.TransmuteStrategics(tabPage.Text);
                                stParam = item.Item1.Split('.');

                                if (check && int.TryParse(stParam[11], out int stNet) && int.TryParse(stParam[9], out int stOperating) && int.TryParse(stParam[7], out int stSales) && int.TryParse(stParam[5], out int stRange) && int.TryParse(stParam[4], out int stInterval) && int.TryParse(stParam[3], out int stQuantity) && int.TryParse(stParam[2], out int stTrend) && int.TryParse(stParam[1], out int stLong) && int.TryParse(stParam[0].Substring(8), out int stShort))
                                    verify = new SendHoldingStocks(item.Item2, new Catalog.ScenarioAccordingToTrend
                                    {
                                        Code = stParam[0].Substring(2, 6),
                                        Short = stShort,
                                        Long = stLong,
                                        Trend = stTrend,
                                        Quantity = stQuantity,
                                        IntervalInSeconds = stInterval,
                                        ErrorRange = stRange * 0.01,
                                        CheckSales = stParam[6].Equals("1"),
                                        Sales = stSales * 0.01,
                                        CheckOperatingProfit = stParam[8].Equals("1"),
                                        OperatingProfit = stOperating * 0.01,
                                        CheckNetIncome = stParam[10].Equals("1"),
                                        NetIncome = stNet * 0.01,
                                        Calendar = stParam[12]
                                    });
                                break;
                        }
                    if (check && verify != null)
                    {
                        buttonSave.ForeColor = Color.Ivory;
                        buttonChart.ForeColor = Color.Ivory;
                        SendSize?.Invoke(this, verify);
                    }
                    else
                        buttonChart.ForeColor = Color.Maroon;
                }
                else if (buttonSave.Name.Equals(bfn.Name) && buttonProgress.ForeColor.Equals(Color.Ivory) && bfn.ForeColor.Equals(Color.Ivory) && double.TryParse(str.Remove(str.Length - 1, 1), out double commission))
                {
                    var sb = new StringBuilder();

                    foreach (TabPage tabPage in tab.TabPages)
                    {
                        foreach (var con in tabPage.Controls)
                            switch (con)
                            {
                                case ScenarioAccordingToTrend _:
                                    continue;

                                case TrendToCashflow tc:
                                    sb.Append(tc.TransmuteStrategics(tabPage.Text));
                                    break;

                                case TrendFollowingBasicFutures tf:
                                    sb.Append(tf.TransmuteStrategics(tabPage.Text));
                                    break;

                                case TrendsInStockPrices ts:
                                    sb.Append(ts.TransmuteStrategics(tabPage.Text));
                                    break;
                            }
                        sb.Append(';');
                    }
                    if (0 < await client.PutContext(new Privacies
                    {
                        Security = Privacy.Security,
                        SecuritiesAPI = Privacy.SecuritiesAPI,
                        SecurityAPI = Privacy.SecurityAPI,
                        Account = Privacy.Account,
                        CodeStrategics = sb.Remove(sb.Length - 1, 1).ToString(),
                        Commission = commission / 0x64,
                        Coin = Privacy.Coin + GoblinBatClient.Coin
                    }))
                        bfn.ForeColor = Color.Maroon;
                }
                else if (buttonProgress.Name.Equals(bfn.Name))
                {
                    switch (buttonProgress.Text)
                    {
                        case start when buttonProgress.ForeColor.Equals(Color.Ivory):
                            progressBar.Value = 0;
                            buttonProgress.Text = cancel;
                            break;

                        case cancel when buttonProgress.ForeColor.Equals(Color.Gold):
                            progressBar.Value = progressBar.Maximum;
                            buttonProgress.Text = start;
                            break;
                    }
                    if (buttonProgress.ForeColor.Equals(Color.Maroon) == false)
                    {
                        buttonProgress.ForeColor = Color.Maroon;
                        SendSize?.Invoke(this, new SendHoldingStocks(progressBar.Value, Privacy));
                    }
                }
            }
            else if (sender is ComboBox box && buttonSave.ForeColor.Equals(Color.Ivory) && comboStrategics.Name.Equals(box.Name) && textCode.TextLength == Length && tab.TabPages.ContainsKey(textCode.Text) == false)
            {
                IEnumerable<ConvertConsensus> consensus = null;

                if (box.SelectedItem.Equals(st))
                {
                    var code = string.Empty;

                    switch (Length)
                    {
                        case 6:
                            code = textCode.Text;
                            break;

                        case 8:
                            break;

                        default:
                            return;
                    }
                    consensus = await client.GetContext(new ConvertConsensus { Code = code });

                    if (consensus == null || consensus.Any(o => o.Date.EndsWith("(E)")) == false)
                    {
                        textCode.Text = string.Empty;
                        box.SelectedIndex = -1;

                        return;
                    }
                }
                SuspendLayout();
                var color = Color.FromArgb(0x79, 0x85, 0x82);
                var select = Codes.FirstOrDefault(o => o.Code.Equals(textCode.Text));
                tab.Controls.Add(new TabPage
                {
                    BackColor = Color.Transparent,
                    BorderStyle = BorderStyle.FixedSingle,
                    ForeColor = Color.Black,
                    Location = new Point(4, 0x22),
                    Margin = new Padding(0),
                    Name = textCode.Text,
                    TabIndex = Index++,
                    Text = textCode.Text,
                    ToolTipText = box.SelectedItem as string
                });
                switch (box.SelectedItem)
                {
                    case tc:
                        if (string.IsNullOrEmpty(select.MaturityMarketCap) == false && string.IsNullOrEmpty(select.Price) == false)
                        {
                            var view = new TrendToCashflow(select);
                            tab.TabPages[tab.TabPages.Count - 1].Controls.Add(view);
                            view.Dock = DockStyle.Fill;
                            panel.RowStyles[0].Height = 0x145 + 0x23;
                        }
                        break;

                    case ts:
                        if (string.IsNullOrEmpty(select.MaturityMarketCap) == false && string.IsNullOrEmpty(select.Price) == false)
                        {
                            var view = new TrendsInStockPrices(select);
                            tab.TabPages[tab.TabPages.Count - 1].Controls.Add(view);
                            view.Dock = DockStyle.Fill;
                            panel.RowStyles[0].Height = 0xCD + 0x23;
                        }
                        break;

                    case tf:
                        if (select.MarginRate > 0 && string.IsNullOrEmpty(select.Price) == false)
                        {
                            var view = new TrendFollowingBasicFutures(select);
                            tab.TabPages[tab.TabPages.Count - 1].Controls.Add(view);
                            view.Dock = DockStyle.Fill;
                            panel.RowStyles[0].Height = 0x83 + 0x23;
                        }
                        break;

                    case st:
                        if (string.IsNullOrEmpty(select.MaturityMarketCap) == false && string.IsNullOrEmpty(select.Price) == false)
                        {
                            var view = new ScenarioAccordingToTrend(select, new ConvertConsensus().PresumeToConsensus(consensus));
                            tab.TabPages[tab.TabPages.Count - 1].Controls.Add(view);
                            view.Dock = DockStyle.Fill;
                            panel.RowStyles[0].Height = 0xEB + 0x23;
                        }
                        break;
                }
                this.link.LinkVisited = false;
                var lasttabrect = tab.GetTabRect(tab.TabPages.Count - 1);
                tab.TabPages[tab.TabPages.Count - 1].BackColor = color;
                tab.SelectTab(tab.TabPages.Count - 1);
                tab.CreateGraphics().FillRectangle(new SolidBrush(color), new RectangleF(lasttabrect.X + lasttabrect.Width + tab.Left, tab.Top + lasttabrect.Y, tab.Width - (lasttabrect.X + lasttabrect.Width), lasttabrect.Height));
                buttonSave.ForeColor = Color.Maroon;
                ResumeLayout();
            }
        }));
        void RemoveThePage(object sender, KeyEventArgs e)
        {
            if (e.KeyData.Equals(Keys.Delete) && sender is TabControl tab)
            {
                SuspendLayout();
                var page = tab.TabPages[tab.SelectedIndex];

                foreach (Control control in page.Controls)
                    control.Dispose();

                page.Controls.Clear();
                page.Dispose();
                tab.Controls.Remove(page);
                ResumeLayout();
            }
        }
        void StrategicsMouseHover(object sender, EventArgs e)
        {
            switch (sender)
            {
                case ComboBox box when box.Equals(comboStrategics):
                    var sb = new StringBuilder();
                    var count = 0;

                    foreach (var text in comboStrategics.Text.ToCharArray())
                    {
                        if (count++ > 0 && char.IsUpper(text))
                            sb.Append(' ');

                        sb.Append(text);
                    }
                    new ToolTip
                    {
                        IsBalloon = true,
                        ShowAlways = true,
                        AutoPopDelay = 0x1CB7,
                        InitialDelay = 0x3A7,
                        ReshowDelay = 0,
                        UseAnimation = true,
                        UseFading = true
                    }.SetToolTip(comboStrategics, sb.ToString());
                    break;

                case TabControl tab:
                    var tip = tab.SelectedTab.ToolTipText;

                    if (tip.Length > 2)
                    {
                        var str = new StringBuilder();
                        var index = 0;

                        foreach (var text in comboStrategics.Text.ToCharArray())
                        {
                            if (index++ > 0 && char.IsUpper(text))
                                str.Append(' ');

                            str.Append(text);
                        }
                        tip = str.ToString();
                    }
                    new ToolTip
                    {
                        IsBalloon = true,
                        ShowAlways = true,
                        AutoPopDelay = tip.Length > 2 ? 0x6D9 : 0x31B,
                        InitialDelay = 0x249F,
                        ReshowDelay = 0x1CE3,
                        UseAnimation = true,
                        UseFading = true
                    }.SetToolTip(tab, tip);
                    break;

                case Button button:
                    switch (Array.FindIndex(this.button, o => o.Equals(button?.Name.Substring(6))))
                    {
                        case 0:
                            buttonProgress.Enabled = buttonProgress.ForeColor.Equals(Color.Maroon) == false;
                            break;

                        case 1:
                            buttonChart.Enabled = buttonProgress.ForeColor.Equals(Color.Gold);
                            break;

                        case 2:
                            buttonSave.Enabled = buttonSave.ForeColor.Equals(Color.Ivory) && buttonProgress.ForeColor.Equals(Color.Ivory);
                            break;
                    }
                    break;
            }
        }
        void TabSelecting(object sender, EventArgs e)
        {
            if (sender is TabControl && e is TabControlCancelEventArgs tc && tc.TabPage != null)
            {
                SuspendLayout();
                SendSize?.Invoke(this, new SendHoldingStocks(tc.TabPage.Size));
                panel.SuspendLayout();
                int height = 0x23;

                switch (tc.TabPage.ToolTipText)
                {
                    case tf:
                    case "TF":
                        height += 0x83;
                        break;

                    case ts:
                    case "TS":
                        height += 0xCD;
                        break;

                    case st:
                    case "ST":
                        height += 0xEB;
                        break;

                    case Strategics.tc:
                    case "TC":
                        height += 0x145;
                        break;
                }
                if (data.Columns.Count > 0)
                {
                    data.SuspendLayout();
                    data.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                    data.ResumeLayout();
                }
                panel.RowStyles[0].Height = height;
                panel.ResumeLayout();
                ResumeLayout();
            }
        }
        void ButtonPreviewKeyDown(object sender, EventArgs e)
        {
            var index = Array.FindIndex(button, o => o.Equals((sender as Button)?.Name.Substring(6)));

            if (e is PreviewKeyDownEventArgs && index > -1)
                switch (index)
                {
                    case 0:
                        buttonProgress.Enabled = buttonProgress.ForeColor.Equals(Color.Maroon) == false;
                        break;

                    case 1:
                        buttonChart.Enabled = buttonProgress.ForeColor.Equals(Color.Gold);
                        break;

                    case 2:
                        buttonSave.Enabled = buttonSave.ForeColor.Equals(Color.Ivory) && buttonProgress.ForeColor.Equals(Color.Ivory);
                        break;
                }
            else
                switch (index)
                {
                    case 0:
                        buttonProgress.Enabled = true;
                        break;

                    case 1:
                        buttonChart.Enabled = true;
                        break;

                    case 2:
                        buttonSave.Enabled = true;
                        break;
                }
        }
        int Length
        {
            get; set;
        }
        int Index
        {
            get; set;
        }
        HashSet<Codes> Codes
        {
            get;
        }
        readonly Disclosure disclosure;
        readonly DataGridView data;
        readonly GoblinBatClient client;
        readonly double[] commissionFutures = { 3e-5, 25e-6, 2e-5, 24e-6, 21e-6, 18e-6, 15e-6, 1e-5 };
        readonly double[] commissionStocks = { 1.5e-4, 1.4e-4, 1.3e-4, 1.2e-4, 1.1e-4 };
    }
    enum StrategicsCode
    {
        Day = 'D',
        DayTrend = 'd',
        Minute = 'M',
        MinuteTrend = 'm',
        Short = 'S',
        Long = 'L',
        Both = 'B',
        Reservation = 'R'
    }
}