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

            api = Futures.Get();

            api.SendBalance += Make;
        }
        private void Make(object sender, Conclusion e)
        {
            if (e.commission != 0)
            {
                commission.Points.AddXY(e.time, e.commission);

                return;
            }
            if (e.commission == 0)
            {
                revenue.Points.AddXY(e.time, e.Total);

                return;
            }
        }
        private readonly Futures api;
        private readonly Series revenue;
        private readonly Series commission;
    }
}