using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ShareInvest.GoblinBatControls
{
    public partial class ChartControl : UserControl
    {
        public ChartControl()
        {
            InitializeComponent();
            price = chart.Series[priceChart];
            revenue = chart.Series[revenueChart];
        }
        public Size SetChartValue(Dictionary<DateTime, string> param)
        {
            price.Points.Clear();
            revenue.Points.Clear();

            foreach (var kv in param)
            {
                var splits = kv.Value.Split(';');

                price.Points.AddXY(kv.Key, splits[1]);
                revenue.Points.AddXY(kv.Key, splits[0]);
            }
            return new Size(1350, 750);
        }
        public void OnReceiveChartValue(object sender, EventHandler.BackTesting.Statistics e)
        {
            price.Points.Clear();
            revenue.Points.Clear();

            foreach (var kv in e.Information)
            {
                var splits = kv.Value.Split(';');

                price.Points.AddXY(kv.Key, splits[1]);
                revenue.Points.AddXY(kv.Key, splits[0]);
            }
        }
        public Size SetChartValue() => new Size(1350, 750);
        readonly Series price;
        readonly Series revenue;
        const string priceChart = "Price";
        const string revenueChart = "Revenue";
    }
}