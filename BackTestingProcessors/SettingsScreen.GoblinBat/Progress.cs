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
        private void TimerTick(object sender, EventArgs e)
        {
            progressBar.Value = ProgressBarValue;
        }
    }
}