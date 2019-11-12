using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.EventHandler;
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
        }
        public void OnReceiveClose(string[] param)
        {
            numericShortDay.Value = int.Parse(param[0]);
            numericShortTick.Value = int.Parse(param[1]);
            numericLongDay.Value = int.Parse(param[2]);
            numericLongTick.Value = int.Parse(param[3]);
            numericReaction.Value = int.Parse(param[4]);
        }
        private void ButtonTrading_Click(object sender, EventArgs e)
        {
            if (!buttonTrading.ForeColor.Equals(Color.DarkRed))
            {
                buttonTrading.ForeColor = Color.DarkRed;
                buttonTrading.Text = "Trading.";
                SendClose?.Invoke(this, new DialogClose(sender, numericShortDay.Value, numericShortTick.Value, numericLongDay.Value, numericLongTick.Value, numericReaction.Value));
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
                SendClose?.Invoke(this, new DialogClose(numericShortDay.Value, numericShortTick.Value, numericLongDay.Value, numericLongTick.Value, numericReaction.Value));
            }
        }
        public event EventHandler<DialogClose> SendClose;
    }
}