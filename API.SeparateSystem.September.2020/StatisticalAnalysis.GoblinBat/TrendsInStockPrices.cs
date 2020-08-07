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
        internal TrendsInStockPrices(string code, Holding holding)
        {
            InitializeComponent();
            price = chart.Series[priceChart];
            revenue = chart.Series[revenueChart];
            sShort = chart.Series[shortChart];
            sLong = chart.Series[longChart];
            holding.SendStocks += OnReceiveChart;
            this.holding = holding;
            this.code = code;
            CenterToScreen();
            Text = string.Empty;
        }
        void OnReceiveChart(object sender, SendHoldingStocks e) => BeginInvoke(new Action(() =>
        {
            if (DateTime.TryParseExact(e.Time, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
            {
                price.Points.AddXY(date, e.Current);
                revenue.Points.AddXY(date, e.Revenue);
                sShort.Points.AddXY(date, e.Base);
                sLong.Points.AddXY(date, e.Secondary);
            }
            if (string.IsNullOrEmpty(Text))
                Text = holding.FindStrategicsCode(code);
        }));
        void TrendsInStockPricesFormClosing(object sender, FormClosingEventArgs e) => Dispose();
        readonly string code;
        readonly Series price;
        readonly Series revenue;
        readonly Series sShort;
        readonly Series sLong;
        readonly Holding holding;
    }
}