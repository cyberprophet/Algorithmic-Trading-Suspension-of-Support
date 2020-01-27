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
        public int Maximum
        {
            get; set;
        }
        public int Rate(int max, int count)
        {
            progressBar.Maximum = max;
            timer.Interval = 395;
            timer.Start();

            return max / count;
        }
        public void Retry()
        {
            timer.Stop();
            Swap = true;
            ProgressBarValue = 0;
            timer.Interval = 315;
            timer.Start();
        }
        private void TimerTick(object sender, EventArgs e)
        {
            if (Swap)
            {
                Swap = false;
                progressBar.Maximum = Maximum;
            }
            progressBar.Value = ProgressBarValue;
            Application.DoEvents();
        }
        private bool Swap
        {
            get; set;
        }
    }
}