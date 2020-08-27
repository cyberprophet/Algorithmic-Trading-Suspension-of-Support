using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.FindByName;

namespace ShareInvest.Controls
{
    partial class ScenarioAccordingToTrend : UserControl
    {
        internal ScenarioAccordingToTrend(Codes codes, Tuple<List<ConvertConsensus>, List<ConvertConsensus>> tuple)
        {
            this.tuple = tuple;
            InitializeComponent();
            var last = tuple.Item2.Where(o => o.Date.EndsWith("(A)")).Max(o => o.Date).Substring(0, 5).Split('.');
            var first = tuple.Item1.Min(o => o.Date).Substring(0, 5).Split('.');

            if (int.TryParse(first[0], out int fYear) && int.TryParse(first[first.Length - 1], out int fMonth) && int.TryParse(last[0], out int lYear) && int.TryParse(last[last.Length - 1], out int lMonth))
            {
                calendar.MaxDate = new DateTime(lYear + 0x7CF, lMonth, DateTime.DaysInMonth(lYear + 0x7CF, lMonth));
                calendar.MinDate = new DateTime(fYear + 0x7D0, fMonth, DateTime.DaysInMonth(fYear + 0x7D0, fMonth));
            }
            labelName.Text = codes.Name;
            code = codes.Code;
        }
        internal (string, Tuple<List<ConvertConsensus>, List<ConvertConsensus>>) TransmuteStrategics(string code)
        {
            if (this.code.Equals(code))
            {
                var sb = new StringBuilder();

                for (int i = 0; i < strategics.Length; i++)
                {
                    var param = string.Concat(numeric, strategics[i]).FindByName<NumericUpDown>(this).Value;

                    if (i > 5)
                        sb.Append(string.Concat(check, strategics[i]).FindByName<CheckBox>(this).Checked ? 1 : 0).Append('.');

                    if (i > 4)
                        param *= 0x64;

                    sb.Append((int)param).Append('.');
                }
                return (string.Concat("ST", code, sb, calendar.SelectionEnd.ToString("yyMMdd")), tuple);
            }
            return (string.Empty, null);
        }
        internal bool TransmuteStrategics() => numericShort.Value < numericLong.Value && (checkNetIncome.Checked || checkOperatingProfit.Checked || checkSales.Checked);
        readonly Tuple<List<ConvertConsensus>, List<ConvertConsensus>> tuple;
        readonly string code;
    }
}