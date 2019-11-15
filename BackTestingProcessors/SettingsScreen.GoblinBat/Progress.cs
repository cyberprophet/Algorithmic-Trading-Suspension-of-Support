using System;
using System.Windows.Forms;

namespace ShareInvest.BackTesting.SettingsScreen
{
    public partial class Progress : UserControl
    {
        public Progress()
        {
            InitializeComponent();
        }
        public int ProgressBarValue
        {
            get; set;
        }
        public int Rate(int max)
        {
            progressBar.Maximum = max;
            timer.Interval = 15;
            timer.Start();

            return max / 110;
        }
        private void TimerTick(object sender, EventArgs e)
        {
            progressBar.Value = ProgressBarValue;
        }
    }
}