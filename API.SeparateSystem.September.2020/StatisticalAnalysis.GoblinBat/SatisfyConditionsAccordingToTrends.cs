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

namespace ShareInvest.Strategics
{
    partial class SatisfyConditionsAccordingToTrends : Form
    {
        internal SatisfyConditionsAccordingToTrends(Privacies privacies, GoblinBatClient client)
        {
            InitializeComponent();
            this.privacies = privacies;
            this.client = client;
            data.ColumnCount = 9;
            data.BackgroundColor = Color.FromArgb(0x79, 0x85, 0x82);
            data.Columns[0].Name = "종목코드";
            data.Columns[1].Name = "종목명";
            data.Columns[data.Columns.Count - 1].Name = "AC";
            var now = DateTime.Now;

            for (int i = 2; i < data.ColumnCount - 1; i++)
            {
                var quarter = GetQuarter(now.AddMonths(i == 2 ? 1 : ((i - 2) * 3) + 1));
                data.Columns[i].Name = quarter;
                var split = quarter.Split('/');
                string.Concat("label", label[i - 2], "Quarter").FindByName<Label>(this).Text = string.Concat("'", split[0], "년 ", split[1].Replace("0", string.Empty), "월");
            }
            data.SortCompare += DataSortCompare;
            data.CellClick += DataCellClick;
            buttonSave.Click += ButtonClick;
            link.Click += ButtonClick;
        }
        internal SatisfyConditionsAccordingToTrends PerformClick()
        {
            timer.Start();

            return this;
        }
        string GetQuarter(DateTime dt)
        {
            switch (dt.Month)
            {
                case int month when month > 0 && month < 4:
                    return string.Concat(dt.Year - 0x7D0, "/03");

                case int month when month > 3 && month < 7:
                    return string.Concat(dt.Year - 0x7D0, "/06");

                case int month when month > 6 && month < 0xA:
                    return string.Concat(dt.Year - 0x7D0, "/09");

                case int month when month > 9 && month < 0xD:
                case 0:
                    return string.Concat(dt.Year - 0x7D0, "/12");
            }
            return null;
        }
        void DataCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (data.Columns.Count > 0)
            {
                data.SuspendLayout();
                data.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

                if (e.RowIndex > -1)
                {
                    var color = data.Rows[e.RowIndex].DefaultCellStyle.BackColor;
                    data.Rows[e.RowIndex].DefaultCellStyle.BackColor = color.Equals(Color.Maroon) ? Color.Silver : Color.Maroon;
                    data.CurrentCell = null;
                }
                data.ResumeLayout();
            }
        }
        void DataSortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index < data.Columns.Count - 1 && e.Column.Index > 1 && double.TryParse(e.CellValue1.ToString().Replace("%", string.Empty), out double x) && double.TryParse(e.CellValue2.ToString().Replace("%", string.Empty), out double y))
            {
                e.SortResult = x.CompareTo(y);
                e.Handled = true;
            }
            else if (e.Column.Index == data.Columns.Count - 1 && int.TryParse(e.CellValue2.ToString().Replace(",", string.Empty), out int ca) && int.TryParse(e.CellValue1.ToString().Replace(",", string.Empty), out int cb))
            {
                e.SortResult = cb.CompareTo(ca);
                e.Handled = true;
            }
        }
        void ButtonClick(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Name.Equals(this.button.Name))
                    BeginInvoke(new Action(async () =>
                    {
                        var strategics = new Dictionary<string, Tuple<int, double, double, double, double, double, double>>();
                        var stack = new Stack<Catalog.Request.Consensus>();

                        for (int i = 0; i < this.strategics.Length; i++)
                            foreach (var context in await client.GetContext(new Catalog.Request.Consensus { Strategics = string.Concat("TC.", this.strategics[i]) }))
                            {
                                if (strategics.TryGetValue(context.Code, out Tuple<int, double, double, double, double, double, double> tuple))
                                    strategics[context.Code] = new Tuple<int, double, double, double, double, double, double>(tuple.Item1 + 1, tuple.Item2 + context.FirstQuarter, tuple.Item3 + context.SecondQuarter, tuple.Item4 + context.ThirdQuarter, tuple.Item5 + context.Quarter, tuple.Item6 + context.TheNextYear, tuple.Item7 + context.TheYearAfterNext);

                                else
                                    strategics[context.Code] = new Tuple<int, double, double, double, double, double, double>(1, context.FirstQuarter, context.SecondQuarter, context.ThirdQuarter, context.Quarter, context.TheNextYear, context.TheYearAfterNext);
                            }
                        foreach (var kv in strategics.OrderByDescending(o => o.Key))
                            if (string.IsNullOrEmpty(kv.Key) == false && kv.Key.Length == 6)
                                stack.Push(new Catalog.Request.Consensus
                                {
                                    Code = kv.Key,
                                    FirstQuarter = kv.Value.Item2 / kv.Value.Item1,
                                    SecondQuarter = kv.Value.Item3 / kv.Value.Item1,
                                    ThirdQuarter = kv.Value.Item4 / kv.Value.Item1,
                                    Quarter = kv.Value.Item5 / kv.Value.Item1,
                                    TheNextYear = kv.Value.Item6 / kv.Value.Item1,
                                    TheYearAfterNext = kv.Value.Item7 / kv.Value.Item1,
                                });
                        if (stack.Count > 0 && await client.GetContextAsync() is Dictionary<string, int> rank && rank.Count > 0 && await client.GetContext(new Codes { }, 6) is List<Codes> codes && codes.Count > 0)
                            InitializeComponent(stack, codes, rank);
                    }));
                else if (button.Name.Equals(buttonSave.Name))
                {
                    if (buttonSave.ForeColor.Equals(Color.Ivory))
                    {
                        if (numericShort.Value < numericLong.Value)
                            BeginInvoke(new Action(async () =>
                            {
                                int index;
                                var numeric = "numeric";
                                StringBuilder ban = new StringBuilder(), setting = new StringBuilder(), value = new StringBuilder(), strategics = new StringBuilder("SC;");

                                for (index = 0; index < this.numeric.Length; index++)
                                    strategics.Append(string.Concat(numeric, this.numeric[index]).FindByName<NumericUpDown>(this).Value).Append(';');

                                foreach (DataGridViewRow row in data.Rows)
                                    if (row.DefaultCellStyle.BackColor.Equals(Color.Maroon))
                                        ban.Append(row.Cells[0].Value).Append(';');

                                for (index = 0; index < label.Length; index++)
                                {
                                    setting.Append(string.Concat(numeric, label[index]).FindByName<NumericUpDown>(this).Value).Append(';');

                                    if (index < label.Length - 1)
                                        value.Append(string.Concat(numeric, label[index], rate).FindByName<NumericUpDown>(this).Value).Append(';');
                                }
                                if (await client.PostContext(new Catalog.Request.SatisfyConditions
                                {
                                    Security = privacies.Security,
                                    Strategics = strategics.Remove(strategics.Length - 1, 1).ToString(),
                                    SettingValue = string.Concat(setting, value.Remove(value.Length - 1, 1)),
                                    Ban = ban.Length > 0 ? ban.Remove(ban.Length - 1, 1).ToString() : string.Empty,
                                    TempStorage = string.Empty
                                }) == 0xC8)
                                {
                                    buttonSave.ForeColor = Color.Maroon;
                                    buttonSave.Text = "Complete";
                                }
                            }));
                        else
                            buttonSave.Text = retry;
                    }
                    else
                    {
                        buttonSave.Text = retry;
                        buttonSave.Enabled = false;
                    }
                }
            }
            else if (sender is Timer)
            {
                BeginInvoke(new Action(async () =>
                {
                    if (await client.GetContext(new Catalog.Request.SatisfyConditions { Security = privacies.Security }) is Catalog.Request.SatisfyConditions condition)
                    {
                        Ban = string.IsNullOrEmpty(condition.Ban) ? null : condition.Ban.Split(';');
                        string[] strategics = condition.Strategics.Split(';'), sv = condition.SettingValue.Split(';');
                        var numeric = "numeric";
                        int index;

                        for (index = 0; index < this.numeric.Length; index++)
                            if (decimal.TryParse(strategics[index + 1], out decimal value))
                                string.Concat(numeric, this.numeric[index]).FindByName<NumericUpDown>(this).Value = value;

                        for (index = 0; index < label.Length; index++)
                            if (decimal.TryParse(sv[index], out decimal setting))
                            {
                                string.Concat(numeric, label[index]).FindByName<NumericUpDown>(this).Value = setting;

                                if (index < label.Length - 1 && decimal.TryParse(sv[index + label.Length], out decimal value))
                                    string.Concat(numeric, label[index], rate).FindByName<NumericUpDown>(this).Value = value;
                            }
                    }
                }));
                this.button.PerformClick();
                timer.Stop();
                timer.Dispose();
            }
            else if (sender is LinkLabel)
            {
                Process.Start(@"https://www.youtube.com/c/TenbaggercyberprophetsStock");
                link.LinkVisited = true;
            }
        }
        void InitializeComponent(Stack<Catalog.Request.Consensus> stack, List<Codes> codes, Dictionary<string, int> rank)
        {
            SuspendLayout();
            data.Rows.Clear();

            while (stack.Count > 0)
            {
                var pop = stack.Pop();

                if (JudgeWhetherMeetsTheConditions(pop))
                {
                    var index = data.Rows.Add(new string[] { pop.Code, codes.First(o => o.Code.Equals(pop.Code)).Name, Math.Abs(pop.FirstQuarter).ToString("P2"), Math.Abs(pop.SecondQuarter).ToString("P2"), Math.Abs(pop.ThirdQuarter).ToString("P2"), Math.Abs(pop.Quarter).ToString("P2"), Math.Abs(pop.TheNextYear).ToString("P2"), Math.Abs(pop.TheYearAfterNext).ToString("P2"), rank.TryGetValue(pop.Code, out int choice) ? choice.ToString("N0") : string.Empty });
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

                    if (Ban != null && Array.Exists(Ban, o => o.Equals(pop.Code)))
                        data.Rows[index].DefaultCellStyle.BackColor = Color.Maroon;
                }
            }
            data.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            data.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            data.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            data.ScrollBars = ScrollBars.Vertical;
            data.AutoResizeRows();
            data.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            ResumeLayout();
        }
        string[] Ban
        {
            get; set;
        }
        bool JudgeWhetherMeetsTheConditions(Catalog.Request.Consensus consensus) => consensus.FirstQuarter > (double)numericFirst.Value * 1e-2 && consensus.SecondQuarter > (double)numericSecond.Value * 1e-2 && consensus.ThirdQuarter > (double)numericThird.Value * 1e-2 && consensus.Quarter > (double)numericFourth.Value * 1e-2 && consensus.TheNextYear > (double)numericFifth.Value * 1e-2 && consensus.TheYearAfterNext > (double)numericSixth.Value * 1e-2 && consensus.FirstQuarter + (double)numericFirstRate.Value * 1e-2 < consensus.SecondQuarter && consensus.SecondQuarter + (double)numericSecondRate.Value * 1e-2 < consensus.ThirdQuarter && consensus.ThirdQuarter + (double)numericThirdRate.Value * 1e-2 < consensus.Quarter && consensus.Quarter + (double)numericFourthRate.Value * 1e-2 < consensus.TheNextYear && consensus.TheNextYear + (double)numericFifthRate.Value * 1e-2 < consensus.TheYearAfterNext;
        readonly Privacies privacies;
        readonly GoblinBatClient client;
    }
}