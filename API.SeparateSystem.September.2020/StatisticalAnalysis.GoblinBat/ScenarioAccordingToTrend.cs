using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using ShareInvest.Analysis;
using ShareInvest.EventHandler;

namespace ShareInvest.Strategics
{
    partial class ScenarioAccordingToTrend : Form
    {
        internal ScenarioAccordingToTrend(Holding holding)
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
            if (e.Strategics is long max && DateTime.TryParseExact(e.Time, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
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
            else if (e.Strategics is Dictionary<DateTime, double> dictionary && DateTime.TryParseExact(e.Time, format.Substring(0, 6), CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime time) && MessageBox.Show(question, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2).Equals(DialogResult.Yes))
                foreach (var kv in dictionary.OrderBy(o => o.Key))
                    if (kv.Key.CompareTo(time) > 0)
                        trend.Points.AddXY(kv.Key, kv.Value);
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