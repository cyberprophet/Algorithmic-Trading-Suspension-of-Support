using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.FindByName;
using ShareInvest.Interface;
using ShareInvest.RecallStatistics;
using ShareInvest.TimerMessageBox;

namespace ShareInvest.Conservation
{
    public partial class Yield : UserControl, IEnumerable
    {
        private void ButtonClick(object sender, EventArgs e)
        {
            if (sender is Button name)
            {
                BeginInvoke(new Action(() =>
                {
                    string temp = name.Name.Replace("button", "rate");
                    var convert = Transmogrify(temp);
                    name.Text = name.Text.Contains('%') ? convert.ToString("C0") : string.Concat("C", (convert / Assets).ToString("P0"), "  D", (convert / Turn[temp] / Assets).ToString("P3"));
                    name.ForeColor = convert > 0 ? Color.Maroon : Color.Navy;

                    if (name.ForeColor.Equals(Color.Navy))
                        name.Text = name.Text.Replace("-", string.Empty);
                }));
            }
        }
        private void ComboBoxSelectedValue(object sender, EventArgs e)
        {
            if (sender is ComboBox cb)
            {
                BeginInvoke(new Action(() =>
                {
                    long revenue = Transmogrify(cb.Name);
                    var name = cb.Name.Replace("rate", "button").FindByName<Button>(this);
                    bool check = revenue > 0 ? true : false;
                    name.Text = check ? revenue.ToString("C0") : Math.Abs(revenue).ToString("C0");
                    name.ForeColor = check ? Color.Maroon : Color.Navy;
                }));
                SendHermes?.Invoke(this, new Hermes(cb.SelectedItem.ToString().Split('.')));
            }
        }
        private long Transmogrify(string sender)
        {
            try
            {
                return Sort[sender].First(o => o.Key.Equals(sender.FindByName<ComboBox>(this).Text.Replace('.', '^'))).Value;
            }
            catch (Exception ex)
            {
                TimerBox.Show(string.Concat(ex.ToString(), "\n\nStatistics don't Exist.\n\nPlease Try Again."), "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information, 3519);
            }
            return 0;
        }
        private double Assets
        {
            get; set;
        }
        private string Temp
        {
            get; set;
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
                var title = name.Replace("rate", "label").FindByName<Label>(this);
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
                        button.Margin = new Padding(3, 1, 3, 7);
                        comboBox.SelectedValueChanged += ComboBoxSelectedValue;

                        if (kv.Value > 0)
                        {
                            button.Text = kv.Value.ToString("C0");
                            button.ForeColor = Color.Maroon;
                        }
                        else
                        {
                            button.Text = kv.Value.ToString("C0");
                            button.ForeColor = Color.Navy;
                        }
                    }
                    if (12500 > i++)
                        comboBox.Items.Add(kv.Key.Replace('^', '.'));
                }
                if (!make.FindByName.Equals("cumulative"))
                    Turn[name] = make.Turn - 1;

                trust.Font = new Font(trust.Font.Name, trust.Font.Size - 3.25F, FontStyle.Regular);
                trust.Text = string.Concat(check.ToString("N0"), " / ", make.DescendingSort.Count.ToString("N0"), "\n", (check / (double)make.DescendingSort.Count).ToString("P2"));
                trust.Cursor = Cursors.Default;
                title.Cursor = Cursors.Default;
                comboBox.ForeColor = Color.FromArgb(96, 48, 25);
                comboBox.Font = new Font("Komoda", 17.25F, FontStyle.Bold);
                comboBox.DropDownHeight = 132;
                comboBox.SelectedIndex = 0;
                comboBox.Margin = new Padding(4, 3, 3, 0);

                if (trust.Text.Contains("-"))
                {
                    trust.ForeColor = Color.Navy;
                    trust.Text = trust.Text.Replace("-", string.Empty);
                }
                else
                    trust.ForeColor = Color.Maroon;

            }
            foreach (string name in Enum.GetNames(typeof(IRecall.ButtonYield)))
            {
                var button = name.FindByName<Button>(this);

                if (button.Text.Contains("-"))
                    button.Text = button.Text.Replace("-", string.Empty);
            }
            ResumeLayout();
        }
        public void OnReceiveStrategy(object sender, Hermes e)
        {
            if (Sort["rateCumulative"].ContainsKey(e.Strategy))
            {
                string temp = e.Strategy.Replace('^', '.');

                foreach (string name in Enum.GetNames(typeof(IRecall.ComboBoxYield)))
                {
                    var article = name.FindByName<ComboBox>(this);

                    if (temp.Equals(Temp))
                        name.Replace("rate", "button").FindByName<Button>(this).PerformClick();

                    else if (article.Items.Contains(temp) == false)
                        article.Items.Add(temp);

                    BeginInvoke(new Action(() => article.SelectedItem = temp));
                }
                Temp = temp;
            }
            else
            {
                TimerBox.Show("Statistics don't Exist.\n\nPlease Try Again.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information, 3519);
                SendHermes?.Invoke(sender, new Hermes(false));
            }
        }
        public DialogResult SetStrategy(DialogResult result)
        {
            int min = int.MinValue, max;
            string find = string.Empty;

            foreach (string name in Enum.GetNames(typeof(IRecall.TrustYield)))
            {
                var temp = name.FindByName<Label>(this);

                if (temp.ForeColor.Equals(Color.Maroon))
                {
                    max = int.Parse(temp.Text.Split('\n')[1].Replace(".", string.Empty).Replace("%", string.Empty));

                    if (max > min && !IRecall.TrustYield.trustRecent.ToString().Equals(name))
                    {
                        min = max;
                        find = name;
                    }
                }
            }
            if (min > 0)
            {
                find.Replace("trust", "label").FindByName<Label>(this).ForeColor = Color.Khaki;
                find.Replace("trust", "rate").FindByName<ComboBox>(this).SelectedIndex = 0;

                return result;
            }
            TimerBox.Show("The Statistics are Very Unreliable.\n\nThus there is No Strategy to Recommend.\n\nPlan Strategy or Stop Trading.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, 3572);

            return DialogResult.Abort;
        }
        public Yield(string[] assets)
        {
            InitializeComponent();
            Sort = new Dictionary<string, Dictionary<string, long>>(1024);
            Turn = new Dictionary<string, int>();
            Assets = long.Parse(assets[1]);
            SuspendLayout();
        }
        public Dictionary<string, int> Turn
        {
            get; private set;
        }
        public Dictionary<string, Dictionary<string, long>> Sort
        {
            get; private set;
        }
        public event EventHandler<Hermes> SendHermes;
    }
}