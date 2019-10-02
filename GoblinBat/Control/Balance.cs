using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ShareInvest.EventHandler;

namespace ShareInvest.Control
{
    public partial class Balance : UserControl
    {
        public Balance()
        {
            InitializeComponent();

            revenue = chart.Series["Revenue"];
            commission = chart.Series["Commission"];

            Futures.Get().SendBalance += Make;
        }
        private void Make(object sender, Conclusion e)
        {
            if (e.Commission != 0 && e.Time != null)
            {
                commission.Points.AddXY(e.Time, e.Commission);

                return;
            }
            if (e.Commission == 0 && e.Time != null)
            {
                revenue.Points.AddXY(e.Time, e.Total);

                return;
            }
        }
        private readonly Series revenue;
        private readonly Series commission;
    }
}