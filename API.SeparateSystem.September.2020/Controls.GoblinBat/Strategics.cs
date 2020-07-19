using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Client;
using ShareInvest.FindByName;

namespace ShareInvest.Controls
{
    public partial class Strategics : UserControl
    {
        Privacies Privacy
        {
            get; set;
        }
        public Strategics()
        {
            InitializeComponent();
            buttonChart.Click += OnReceiveClickItem;
            buttonSave.Click += OnReceiveClickItem;
            textCode.Leave += OnReceiveClickItem;
            Codes = new HashSet<Codes>();
        }
        public event EventHandler<Size> SendSize;
        public string SetPrivacy(Privacies privacy)
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
                        break;

                    case "S":
                        str = "위탁종합";
                        Length = 6;
                        break;
                }
                if (GoblinBatClient.GetContext(new Codes { }, Length) is List<Codes> list)
                {
                    string[] codes = new string[list.Count], names = new string[list.Count];
                    var source = new AutoCompleteStringCollection();

                    for (int i = 0; i < codes.Length; i++)
                        if (Codes.Add(list[i]))
                        {
                            codes[i] = list[i].Code;
                            names[i] = list[i].Name;
                        }
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
                        tab.Controls.Add(new TabPage
                        {
                            BackColor = Color.Transparent,
                            BorderStyle = BorderStyle.FixedSingle,
                            ForeColor = Color.Black,
                            Location = new Point(4, 0x22),
                            Margin = new Padding(0),
                            Name = strategics.Substring(2, 8),
                            TabIndex = Index++,
                            Text = strategics.Substring(2, 8),
                        });
                        if (Length == 8)
                            switch (strategics.Substring(0, 2))
                            {
                                case "TF":
                                    var select = Codes.FirstOrDefault(o => o.Code.Equals(strategics.Substring(2, 8)));

                                    if (select.MarginRate > 0 && string.IsNullOrEmpty(select.Price) == false)
                                    {
                                        var view = new TrendFollowingBasicFutures(select);
                                        tab.TabPages[tab.TabPages.Count - 1].Controls.Add(view);
                                        view.Dock = DockStyle.Fill;
                                        tab.TabPages[tab.TabPages.Count - 1].Size = new Size(0x223, 0x7C);
                                        view.TransmuteStrategics(strategics.Substring(10, 1).Equals("1"), strategics.Substring(11).Split('.'));
                                    }
                                    break;
                            }
                        else if (Length == 6)
                        {

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
        void OnReceiveClickItem(object sender, EventArgs e)
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
                    buttonSave.ForeColor = Color.Ivory;
                }
                else if (buttonSave.Name.Equals(bfn.Name) && bfn.ForeColor.Equals(Color.Maroon) == false && double.TryParse(str.Remove(str.Length - 1, 1), out double commission))
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
                            }
                        sb.Append(';');
                    }
                    if (0xC8 == GoblinBatClient.PutContext<Privacies>(new Privacies
                    {
                        Security = Privacy.Security,
                        SecuritiesAPI = Privacy.SecuritiesAPI,
                        SecurityAPI = Privacy.SecurityAPI,
                        Account = Privacy.Account,
                        CodeStrategics = sb.Remove(sb.Length - 1, 1).ToString(),
                        Commission = commission / 0x64
                    }))
                        bfn.ForeColor = Color.Maroon;
                }
            }
            else if (sender is ComboBox box && comboStrategics.Name.Equals(box.Name) && textCode.TextLength == Length && tab.TabPages.ContainsKey(textCode.Text) == false)
            {
                SuspendLayout();
                var color = Color.FromArgb(0x79, 0x85, 0x82);
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
                    case tf:
                        var select = Codes.FirstOrDefault(o => o.Code.Equals(textCode.Text));

                        if (select.MarginRate > 0 && string.IsNullOrEmpty(select.Price) == false)
                        {
                            var view = new TrendFollowingBasicFutures(select);
                            tab.TabPages[tab.TabPages.Count - 1].Controls.Add(view);
                            view.Dock = DockStyle.Fill;
                            tab.TabPages[tab.TabPages.Count - 1].Size = new Size(0x223, 0x7C);
                        }
                        break;
                }
                this.link.LinkVisited = false;
                var lasttabrect = tab.GetTabRect(tab.TabPages.Count - 1);
                tab.TabPages[tab.TabPages.Count - 1].BackColor = color;
                tab.SelectTab(tab.TabPages.Count - 1);
                tab.CreateGraphics().FillRectangle(new SolidBrush(color), new RectangleF(lasttabrect.X + lasttabrect.Width + tab.Left, tab.Top + lasttabrect.Y, tab.Width - (lasttabrect.X + lasttabrect.Width), lasttabrect.Height));
                ResumeLayout();
            }
        }
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
                SendSize?.Invoke(this, tc.TabPage.Size);
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
        readonly double[] commissionFutures = { 3e-5, 25e-6, 2e-5, 18e-6, 15e-6 };
    }
}