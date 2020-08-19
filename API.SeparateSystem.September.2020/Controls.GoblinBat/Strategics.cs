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
        public Strategics(dynamic client)
        {
            InitializeComponent();
            buttonChart.Click += OnReceiveClickItem;
            buttonSave.Click += OnReceiveClickItem;
            buttonProgress.Click += OnReceiveClickItem;
            textCode.Leave += OnReceiveClickItem;
            Codes = new HashSet<Codes>();
            this.client = client;
        }
        public event EventHandler<SendHoldingStocks> SendSize;
        public void SetProgressRate(int rate) => progressBar.Value = rate + 1;
        public void SetProgressRate(Color color) => buttonProgress.ForeColor = color;
        public void SetProgressRate(bool state)
        {
            if (state)
            {
                buttonProgress.Text = start;
                progressBar.Value = 0;
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

                            comboStrategics.Items.AddRange(new object[] { ts });
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
                        var select = Codes.FirstOrDefault(o => o.Code.Equals(strategics.Substring(2, Length)));
                        tab.Controls.Add(new TabPage
                        {
                            BackColor = Color.Transparent,
                            BorderStyle = BorderStyle.FixedSingle,
                            ForeColor = Color.Black,
                            Location = new Point(4, 0x22),
                            Margin = new Padding(0),
                            Name = strategics.Substring(2, Length),
                            TabIndex = Index++,
                            Text = strategics.Substring(2, Length),
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
                var url = "https://youtu.be/CIfSIsozG_E";

                if (comboStrategics.SelectedItem is string str)
                    switch (str)
                    {
                        case tf:
                            url = "https://youtu.be/CIfSIsozG_E";
                            break;

                        case ts:
                            url = "https://youtu.be/CIfSIsozG_E";
                            break;
                    }
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
                        }
                    if (check && verify != null)
                    {
                        buttonSave.ForeColor = Color.Ivory;
                        SendSize?.Invoke(this, verify);
                    }
                    else
                        buttonChart.ForeColor = Color.Maroon;
                }
                else if (buttonSave.Name.Equals(bfn.Name) && bfn.ForeColor.Equals(Color.Ivory) && double.TryParse(str.Remove(str.Length - 1, 1), out double commission))
                {
                    var sb = new StringBuilder();

                    foreach (TabPage tabPage in tab.TabPages)
                    {
                        foreach (var con in tabPage.Controls)
                            switch (con)
                            {
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
                });
                switch (box.SelectedItem)
                {
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
        void TabSelecting(object sender, EventArgs e)
        {
            if (sender is TabControl && e is TabControlCancelEventArgs tc && tc.TabPage != null)
                SendSize?.Invoke(this, new SendHoldingStocks(tc.TabPage.Size));
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