using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using ShareInvest.Catalog;

namespace ShareInvest.Controls
{
    partial class ScenarioAccordingToTrend : UserControl
    {
        internal ScenarioAccordingToTrend(Codes codes, Tuple<List<ConvertConsensus>, List<ConvertConsensus>> tuple)
        {
            this.tuple = tuple;
            InitializeComponent();
            labelName.Text = codes.Name;
            var last = tuple.Item2.Where(o => o.Date.EndsWith("(A)")).Max(o => o.Date).Substring(0, 5).Split('.');
            var first = tuple.Item1.Min(o => o.Date).Substring(0, 5).Split('.');

            if (int.TryParse(first[0], out int fYear) && int.TryParse(first[first.Length - 1], out int fMonth) && int.TryParse(last[0], out int lYear) && int.TryParse(last[last.Length - 1], out int lMonth))
            {
                calendar.MaxDate = new DateTime(lYear + 0x7D0, lMonth, DateTime.DaysInMonth(lYear + 0x7D0, lMonth));
                calendar.MinDate = new DateTime(fYear + 0x7D0, fMonth, DateTime.DaysInMonth(fYear + 0x7D0, fMonth));
            }
        }
        readonly Tuple<List<ConvertConsensus>, List<ConvertConsensus>> tuple;
    }
}