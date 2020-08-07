using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using ShareInvest.Analysis;
using ShareInvest.EventHandler;

namespace ShareInvest.Strategics
{
    partial class TrendFollowingBasicFutures : Form
    {
        internal TrendFollowingBasicFutures(Holding holding)
        {
            InitializeComponent();
            price = chart.Series[priceChart];
            revenue = chart.Series[revenueChart];
            sShort = chart.Series[shortChart];
            sLong = chart.Series[longChart];
            holding.SendStocks += OnReceiveChart;
        }
        void OnReceiveChart(object sender, SendHoldingStocks e)
        {

        }
        readonly Series price;
        readonly Series revenue;
        readonly Series sShort;
        readonly Series sLong;
    }
}