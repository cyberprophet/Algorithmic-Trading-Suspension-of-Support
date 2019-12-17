using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ShareInvest.FindByName;
using ShareInvest.Interface;
using ShareInvest.RecallStatistics;

namespace ShareInvest.Conservation
{
    public partial class Yield : UserControl, IEnumerable
    {
        private void ButtonClick(object sender, EventArgs e)
        {
            Button name = (Button)sender;
            string temp = name.Name.Replace("button", "rate");
            var convert = Sort[temp].FirstOrDefault(o => o.Key.Equals(temp.FindByName<ComboBox>(this).Text.Replace('.', '^'))).Value;
            name.Text = name.Text.Contains('%') ? convert.ToString("C0") : string.Concat("C", (convert / Assets).ToString("P2"), " D", (convert / Turn[temp] / Assets).ToString("P3"));
            name.ForeColor = convert > 0 ? Color.Maroon : Color.Navy;

            if (name.ForeColor.Equals(Color.Navy))
                name.Text = name.Text.Replace("-", string.Empty);
        }
        private double Assets
        {
            get; set;
        }
        public Dictionary<string, int> Turn
        {
            get; private set;
        }
        public Dictionary<string, Dictionary<string, long>> Sort
        {
            get; private set;
        }
        public Yield(string[] assets)
        {
            InitializeComponent();
            Sort = new Dictionary<string, Dictionary<string, long>>(1024);
            Turn = new Dictionary<string, int>();
            Assets = long.Parse(assets[1]);
            SuspendLayout();
        }
        public IEnumerator GetEnumerator()
        {
            foreach (var temp in new Recall())
            {
                if (temp.GetType().Equals(typeof(string)))
                {
                    string[] date = temp.ToString().Split('^');
                    labelCumulative.Text = string.Concat(labelCumulative.Text, "[", date[0], "]");
                    labelRecent.Text = string.Concat(labelRecent.Text.Replace(" ", string.Empty), "[", date[1], "]");

                    break;
                }
                if (temp.GetType().Equals(typeof(int)))
                {
                    Turn["rateCumulative"] = (int)temp;

                    continue;
                }
                var make = (IMakeUp)temp;
                var name = Array.Find(Enum.GetNames(typeof(IRecall.ComboBoxYield)), o => o.Contains(make.FindByName.Substring(1)));
                var comboBox = name.FindByName<ComboBox>(this);
                var trust = name.Replace("rate", "trust").FindByName<Label>(this);
                int i = 0, check = 0;
                comboBox.Sorted = false;
                Sort[name] = make.DescendingSort;

                foreach (KeyValuePair<string, long> kv in Sort[name].OrderByDescending(o => o.Value))
                {
                    check += kv.Value > 0 ? 1 : -1;

                    yield return kv.Key.Split('^');

                    if (i < 1)
                    {
                        var button = name.Replace("rate", "button").FindByName<Button>(this);
                        button.Click += ButtonClick;

                        if (kv.Value > 0)
                        {
                            button.Text = kv.Value.ToString("C0");
                            button.ForeColor = Color.Maroon;
                        }
                        else
                        {
                            button.Text = kv.Value.ToString("C0").Replace("-", string.Empty);
                            button.ForeColor = Color.Navy;
                        }
                    }
                    if (12500 > i++)
                        comboBox.Items.Add(kv.Key.Replace('^', '.'));
                }
                if (!make.FindByName.Equals("cumulative"))
                    Turn[name] = make.Turn - 1;

                trust.Font = new Font(Font.Name, Font.Size - 4.25F, FontStyle.Regular);
                trust.Text = string.Concat(check.ToString("N0"), " / ", make.DescendingSort.Count.ToString("N0"), "\n", (check / (double)make.DescendingSort.Count).ToString("P2"));
                trust.Cursor = Cursors.Default;
                comboBox.ForeColor = Color.FromArgb(96, 48, 25);
                comboBox.Font = new Font("Komoda", 14.35F, FontStyle.Bold);
                comboBox.DropDownHeight = 126;
                comboBox.SelectedIndex = 0;

                if (trust.Text.Contains("-"))
                {
                    trust.ForeColor = Color.Navy;
                    trust.Text = trust.Text.Replace("-", string.Empty);
                }
                else
                    trust.ForeColor = Color.Maroon;
            }
            ResumeLayout();
        }
    }
}