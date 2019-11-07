using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.EventHandler.x64;

namespace ShareInvest.Controls.x64
{
    public partial class Progress : UserControl
    {
        public Progress()
        {
            Bar = new ProgressBar
            {
                BackColor = Color.FromArgb(121, 133, 130),
                Cursor = Cursors.WaitCursor,
                Dock = DockStyle.Fill,
                Size = new Size(1915, 30)
            };
            Controls.Add(Bar);
            InitializeComponent();
        }
        public void Rate(object sender, Rate rate)
        {
            if (Bar.Maximum != rate.Result)
                Bar.Maximum = rate.Result;

            timer.Start();
        }
        public int ProgressBarValue
        {
            get; set;
        }
        private ProgressBar Bar
        {
            get; set;
        }
        private void TimerTick(object sender, EventArgs e)
        {
            Bar.Value = ProgressBarValue;
        }
    }
}