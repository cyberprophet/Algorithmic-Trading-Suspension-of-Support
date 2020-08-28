using System;
using System.Globalization;
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
            CenterToScreen();
            Text = holding.Code;
            WindowState = FormWindowState.Maximized;
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
        }));
        void TrendFollowingFormClosing(object sender, FormClosingEventArgs e)
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
        readonly Series price;
        readonly Series revenue;
        readonly Series sShort;
        readonly Series sLong;
    }
}