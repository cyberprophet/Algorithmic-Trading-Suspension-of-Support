using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.Const;
using ShareInvest.EventHandler;
using ShareInvest.FindByName;
using ShareInvest.Interface;
using ShareInvest.TimerMessageBox;

namespace ShareInvest.Controls
{
    public partial class SelectStrategies : UserControl
    {
        public SelectStrategies()
        {
            InitializeComponent();
            timer.Interval = 2753;
            timer.Start();
        }
        public void OnReceiveColor(ColorFactory cf)
        {
            labelShortTick.ForeColor = cf.Recent;
            labelShortDay.ForeColor = cf.Weekly;
            labelLongTick.ForeColor = cf.Biweekly;
            labelLongDay.ForeColor = cf.Monthly;
            labelReaction.ForeColor = cf.For3Months;
            labelHedge.ForeColor = cf.Cumulative;
            labelShortTick.Text = string.Concat(labelShortTick.Text, cf.StrRecent.Contains("-") ? cf.StrRecent.Replace("-", "") : cf.StrRecent);
            labelShortDay.Text = string.Concat(labelShortDay.Text, cf.StrWeekly.Contains("-") ? cf.StrWeekly.Replace("-", "") : cf.StrWeekly);
            labelLongTick.Text = string.Concat(labelLongTick.Text, cf.StrBiweekly.Contains("-") ? cf.StrBiweekly.Replace("-", "") : cf.StrBiweekly);
            labelLongDay.Text = string.Concat(labelLongDay.Text, cf.StrMonthly.Contains("-") ? cf.StrMonthly.Replace("-", "") : cf.StrMonthly);
            labelReaction.Text = string.Concat(labelReaction.Text, cf.StrFor3Months.Contains("-") ? cf.StrFor3Months.Replace("-", "") : cf.StrFor3Months);
            labelHedge.Text = string.Concat(labelHedge.Text, cf.StrCumulative.Contains("-") ? cf.StrCumulative.Replace("-", "") : cf.StrCumulative);
        }
        public void OnReceiveClose(object sender, DialogClose e)
        {
            for (int i = 0; i < e.Param.Length; i++)
                string.Concat("numeric", Enum.GetName(typeof(IStatistics.Numeric), i)).FindByName<NumericUpDown>(this).Value = int.Parse(e.Param[i]);
        }
        public void OnReceiveClose(string[] param)
        {
            for (int i = 0; i < param.Length; i++)
                string.Concat("numeric", Enum.GetName(typeof(IStatistics.Numeric), i)).FindByName<NumericUpDown>(this).Value = int.Parse(param[i]);
        }
        private void ButtonTradingClick(object sender, EventArgs e)
        {
            if (!buttonTrading.ForeColor.Equals(Color.DarkRed))
            {
                buttonTrading.ForeColor = Color.DarkRed;
                buttonTrading.Text = "Trading.";
                SendClose?.Invoke(this, new DialogClose(numericHedge.Value, sender, numericShortDay.Value, numericShortTick.Value, numericLongDay.Value, numericLongTick.Value, numericReaction.Value));
            }
        }
        private void TimerTick(object sender, EventArgs e)
        {
            timer.Interval = 347365;

            if (!buttonTrading.ForeColor.Equals(Color.DarkRed) && TimerBox.Show("Click 'No' to Edit the Automatically generated Strategy.\n\nIf No Selection is made for 20 Seconds,\nTrading Starts with an Automatically Generated Strategy.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 21753).Equals((DialogResult)6))
            {
                buttonTrading.ForeColor = Color.DarkRed;
                buttonTrading.Text = "Trading.";
                timer.Stop();
                timer.Dispose();
                SendClose?.Invoke(this, new DialogClose(numericHedge.Value, numericShortDay.Value, numericShortTick.Value, numericLongDay.Value, numericLongTick.Value, numericReaction.Value));
            }
        }
        public event EventHandler<DialogClose> SendClose;
    }
}