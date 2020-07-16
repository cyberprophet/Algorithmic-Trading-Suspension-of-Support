using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using ShareInvest.Catalog;
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
            textCode.Enter += OnReceiveClickItem;
        }
        public string SetPrivacy(Privacies privacy)
        {
            Privacy = privacy;
            string str = "도깨비방망이";

            if (string.IsNullOrEmpty(privacy.Account) == false)
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
            return str;
        }
        void OnReceiveClickItem(object sender, EventArgs e)
        {
            if (sender is TextBox text)
            {

            }
            else if (sender is LinkLabel link)
            {
                var url = "https://youtu.be/CIfSIsozG_E";

                switch (comboStrategics.SelectedItem.ToString())
                {
                    case tf:
                        url = "https://youtu.be/CIfSIsozG_E";
                        break;
                }
                Process.Start(url);
                link.LinkVisited = true;
            }
            else if (sender is Button button)
            {
                var bfn = button.Name.FindByName<Button>(this);

                if (buttonChart.Name.Equals(bfn.Name))
                {

                }
                else if (buttonSave.Name.Equals(bfn.Name))
                {

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
                    Size = new Size(0x223, 0x7C),
                    TabIndex = Index++,
                    Text = textCode.Text,
                });
                switch (box.SelectedItem)
                {
                    case tf:
                        var view = new TrendFollowingBasicFutures(0.1335);
                        tab.TabPages[tab.TabPages.Count - 1].Controls.Add(view);
                        view.Dock = DockStyle.Fill;
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
        int Length
        {
            get; set;
        }
        int Index
        {
            get; set;
        }
        readonly double[] commissionFutures = { 3e-5, 25e-6, 2e-5, 18e-6, 15e-6 };
    }
}