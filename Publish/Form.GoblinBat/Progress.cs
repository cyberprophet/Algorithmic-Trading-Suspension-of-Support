using System;
using System.Windows.Forms;
using ShareInvest.EventHandler;

namespace ShareInvest.Control
{
    public partial class Progress : UserControl
    {
        public Progress()
        {
            InitializeComponent();
        }
        public void Rate(object sender, ProgressRate pr)
        {
            if (progressBar.Maximum != pr.Result)
                progressBar.Maximum = pr.Result;

            timer.Start();
        }
        public int ProgressBarValue
        {
            get; set;
        }
        private void TimerTick(object sender, EventArgs e)
        {
            progressBar.Value = ProgressBarValue;
        }
    }
}