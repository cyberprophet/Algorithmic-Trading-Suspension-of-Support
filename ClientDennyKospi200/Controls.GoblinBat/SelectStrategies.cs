using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.EventHandler;
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
        public void OnReceiveClose(object sender, DialogClose e)
        {
            numericReaction.Value = e.Reaction;
            numericShortTick.Value = e.ShortTick;
            numericShortDay.Value = e.ShortDay;
            numericLongTick.Value = e.LongTick;
            numericLongDay.Value = e.LongDay;
            checkHedge.CheckState = (CheckState)e.Hedge;
        }
        public void OnReceiveClose(string[] param)
        {
            numericShortDay.Value = int.Parse(param[0]);
            numericShortTick.Value = int.Parse(param[1]);
            numericLongDay.Value = int.Parse(param[2]);
            numericLongTick.Value = int.Parse(param[3]);
            numericReaction.Value = int.Parse(param[4]);
            checkHedge.CheckState = (CheckState)int.Parse(param[5]);
        }
        private void CheckHedgeCheckStateChanged(object sender, EventArgs e)
        {
            checkHedge.Text = Enum.GetName(typeof(IStatistics.Hedge), (int)checkHedge.CheckState);
            checkHedge.ForeColor = Color.FromArgb(255 - (int)checkHedge.CheckState * 120, 255 - (int)checkHedge.CheckState * 60, 255 - (int)checkHedge.CheckState * 30);

            if (checkHedge.CheckState.Equals(CheckState.Unchecked))
                checkHedge.BackColor = Color.FromArgb(121, 133, 130);

            else
                checkHedge.BackColor = Color.SlateGray;
        }
        private void ButtonTradingClick(object sender, EventArgs e)
        {
            if (!buttonTrading.ForeColor.Equals(Color.DarkRed))
            {
                buttonTrading.ForeColor = Color.DarkRed;
                buttonTrading.Text = "Trading.";
                SendClose?.Invoke(this, new DialogClose(checkHedge.CheckState, sender, numericShortDay.Value, numericShortTick.Value, numericLongDay.Value, numericLongTick.Value, numericReaction.Value));
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
                SendClose?.Invoke(this, new DialogClose(checkHedge.CheckState, numericShortDay.Value, numericShortTick.Value, numericLongDay.Value, numericLongTick.Value, numericReaction.Value));
            }
        }
        public event EventHandler<DialogClose> SendClose;
    }
}