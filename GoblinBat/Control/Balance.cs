using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ShareInvest.AutoMessageBox;
using ShareInvest.EventHandler;

namespace ShareInvest.Control
{
    public partial class Balance : UserControl
    {
        public Balance()
        {
            InitializeComponent();

            gap = chart.Series["Revenue"];

            Futures.Get().SendBalance += Make;
        }
        public int Gap(string time, double gap)
        {
            if (time.Length == 6 && (time.Substring(4, 2).Equals("00") || time.Substring(4, 2).Equals("30")))
                this.gap.Points.AddXY(time, gap);

            return gap > 0 ? 1 : -1;
        }
        private void Make(object sender, Conclusion e)
        {
            if (e.Commission != 0 && e.Time != null)
                iar = BeginInvoke(new Action(() => Box.Show("Commission ￦" + e.Commission.ToString("N0"), e.Time.ToString("HH시 mm분 ss초"), 735)));

            else if (e.Commission == 0 && e.Time != null)
                iar = BeginInvoke(new Action(() => Box.Show(e.Total < 0 ? string.Concat("Total Loss ￦", e.Total.ToString("N0").Substring(1)) : string.Concat("Total Revenue ￦", e.Total.ToString("N0")), e.Time.ToString("HH시 mm분 ss초"), 735)));

            else
                return;

            EndInvoke(iar);
        }
        private readonly Series gap;
        private IAsyncResult iar;
    }
}