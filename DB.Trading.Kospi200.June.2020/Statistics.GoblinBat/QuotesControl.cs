using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.FindByName;

namespace ShareInvest.GoblinBatControls
{
    public partial class QuotesControl : UserControl
    {
        public QuotesControl()
        {
            InitializeComponent();
            stateSell.ForeColor = Color.Navy;
            stateBuy.ForeColor = Color.Crimson;
        }
        public void OnReceiveState(object sender, State e)
        {
            BeginInvoke(new Action(() =>
            {
                stateReceive.Text = e.OnReceive ? (e.Max ?? "주문가능") : e.ScreenNumber;
                stateSell.Text = e.SellOrderCount;
                stateBuy.Text = e.BuyOrderCount;
                var position = e.Quantity.Contains("-");
                stateQuantity.Text = position ? e.Quantity.Substring(1) : e.Quantity;
                stateQuantity.ForeColor = position ? Color.DeepSkyBlue : Color.Maroon;
            }));
        }
        public void OnReceiveTrend(object sender, Trends e)
        {
            BeginInvoke(new Action(() =>
            {
                var count = 0;

                foreach (var kv in e.Trend.OrderByDescending(o => o.Key))
                {
                    var label = string.Concat("state", count++).FindByName<Label>(this);
                    var trend = kv.Value.Contains("-");
                    label.Text = trend ? kv.Value.Substring(1) : kv.Value;
                    label.ForeColor = trend ? Color.Navy : Color.Maroon;
                }
                var check = e.Volume.Contains("-");
                stateVolume.Text = check ? e.Volume.Substring(1) : e.Volume;
                stateVolume.ForeColor = check ? Color.DeepSkyBlue : Color.Maroon;
            }));
        }
        public void OnReceiveQuotes(object sender, EventHandler.XingAPI.Quotes e)
        {
            BeginInvoke(new Action(() =>
            {
                var time = DateTime.ParseExact(e.Time, "HHmmss", null).ToString("HH : mm : ss");

                if (quotes10.Text.Equals(time) == false)
                    quotes10.Text = time;

                for (int i = 0; i < e.Price.Length; i++)
                {
                    var temp = string.Concat("quotes", i).FindByName<Label>(this);
                    var param = e.Price[i].ToString("N2");
                    var temporary = string.Concat("order", i).FindByName<Label>(this);

                    if (temp.Text.Equals(param) == false)
                        temp.Text = param;

                    if (e.SellOrder != null && i < 5 && e.SellOrder.ContainsValue(e.Price[i]))
                    {
                        var number = int.Parse(e.SellOrder.First(o => o.Value == e.Price[i]).Key).ToString();

                        if (temporary.Text.Equals(number))
                            continue;

                        temporary.Text = number;
                    }
                    else if (e.BuyOrder != null && i > 4 && e.BuyOrder.ContainsValue(e.Price[i]))
                    {
                        var number = int.Parse(e.BuyOrder.First(o => o.Value == e.Price[i]).Key).ToString();

                        if (temporary.Text.Equals(number))
                            continue;

                        temporary.Text = number;
                    }
                    else
                        temporary.Text = string.Empty;
                }
                Application.DoEvents();
            }));
        }
        public void OnReceiveQuotes(object sender, EventHandler.OpenAPI.Quotes e)
        {
            BeginInvoke(new Action(() =>
            {
                var time = DateTime.ParseExact(e.Time, "HHmmss", null).ToString("HH : mm : ss");

                if (quotes10.Text.Equals(time) == false)
                    quotes10.Text = time;

                for (int i = 0; i < e.Price.Length; i++)
                {
                    var temp = string.Concat("quotes", i).FindByName<Label>(this);
                    var param = e.Price[i].ToString("N2");
                    var temporary = string.Concat("order", i).FindByName<Label>(this);

                    if (temp.Text.Equals(param) == false)
                        temp.Text = param;

                    if (e.SellOrder != null && i < 5 && e.SellOrder.ContainsValue(e.Price[i]))
                    {
                        var number = int.Parse(e.SellOrder.First(o => o.Value == e.Price[i]).Key).ToString();

                        if (temporary.Text.Equals(number))
                            continue;

                        temporary.Text = number;
                    }
                    else if (e.BuyOrder != null && i > 4 && e.BuyOrder.ContainsValue(e.Price[i]))
                    {
                        var number = int.Parse(e.BuyOrder.First(o => o.Value == e.Price[i]).Key).ToString();

                        if (temporary.Text.Equals(number))
                            continue;

                        temporary.Text = number;
                    }
                    else
                        temporary.Text = string.Empty;
                }
                Application.DoEvents();
            }));
        }
        public void OnReceiveOrderMsg(string message)
        {
            this.message.Text = message;
        }
    }
}