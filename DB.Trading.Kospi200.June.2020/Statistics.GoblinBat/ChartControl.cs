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
            _short = chart.Series[shortChart];
            _long = chart.Series[longChart];
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
            _short.Points.Clear();
            _long.Points.Clear();

            if (e.Information != null)
                foreach (var kv in e.Information)
                {
                    var splits = kv.Value.Split(';');

                    price.Points.AddXY(kv.Key, splits[1]);
                    revenue.Points.AddXY(kv.Key, splits[0]);
                    _short.Points.AddXY(kv.Key, splits[2]);
                    _long.Points.AddXY(kv.Key, splits[3]);
                }
        }
        public Size SetChartValue() => new Size(1350, 750);
        readonly Series price;
        readonly Series revenue;
        readonly Series _short;
        readonly Series _long;
    }
}