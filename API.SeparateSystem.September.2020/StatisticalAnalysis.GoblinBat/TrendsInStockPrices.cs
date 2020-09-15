using System;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using ShareInvest.Analysis;
using ShareInvest.EventHandler;

namespace ShareInvest.Strategics
{
    partial class TrendsInStockPrices : Form
    {
        internal TrendsInStockPrices(Holding holding)
        {
            InitializeComponent();
            price = chart.Series[priceChart];
            revenue = chart.Series[revenueChart];
            sShort = chart.Series[shortChart];
            sLong = chart.Series[longChart];
            trend = chart.Series[trendChart];
            profit = chart.Series[profitChart];
            holding.SendStocks += OnReceiveChart;
            this.holding = holding;
            CenterToScreen();
            code = holding.Code;
            Text = string.Empty;
            WindowState = FormWindowState.Maximized;
        }
        void OnReceiveChart(object sender, SendHoldingStocks e) => BeginInvoke(new Action(() =>
        {
            if (DateTime.TryParseExact(e.Time, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date) && e.Strategics is long max)
            {
                price.Points.AddXY(date, e.Current);
                revenue.Points.AddXY(date, e.Revenue);
                sShort.Points.AddXY(date, e.Base);
                sLong.Points.AddXY(date, e.Secondary);
                trend.Points.AddXY(date, e.Rate);
                profit.Points.AddXY(date, max);

                if (string.IsNullOrEmpty(Text))
                    Text = holding.FindStrategicsCode(code);

                else
                    Cursor = Cursors.Default;
            }
            else if (e.Strategics is DateTime time)
                trend.Points.AddXY(time, e.Rate);
        }));
        void TrendsInStockPricesFormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(exit, Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning).Equals(DialogResult.Cancel))
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                Application.DoEvents();

                return;
            }
            Dispose();
        }
        readonly string code;
        readonly Series price;
        readonly Series revenue;
        readonly Series sShort;
        readonly Series sLong;
        readonly Series trend;
        readonly Series profit;
        readonly Holding holding;
    }
}