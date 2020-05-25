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
            ran = new Random(Guid.NewGuid().GetHashCode());
            InitializeComponent();
            stateSell.ForeColor = Color.Navy;
            stateBuy.ForeColor = Color.Crimson;
        }
        public void OnReceiveState(object sender, State e) => BeginInvoke(new Action(() =>
        {
            if (e.Max != null)
            {
                var classfication = e.Max.Contains("-");
                stateReceive.Text = e.OnReceive ? (classfication ? e.Max.Substring(1) : e.Max) : e.ScreenNumber;
                stateReceive.ForeColor = e.OnReceive ? (classfication ? Color.Navy : Color.DarkRed) : Color.Ivory;
            }
            else
                stateReceive.Text = e.OnReceive ? "주문가능" : e.ScreenNumber;

            stateSell.Text = e.SellOrderCount;
            stateBuy.Text = e.BuyOrderCount;
            var position = e.Quantity.Contains("-");
            stateQuantity.Text = position ? e.Quantity.Substring(1) : e.Quantity;
            stateQuantity.ForeColor = position ? Color.DeepSkyBlue : Color.Maroon;
        }));
        public void OnReceiveTrend(object sender, Trends e) => BeginInvoke(new Action(() =>
        {
            var count = 0;

            if (string.IsNullOrEmpty(e.Volume) == false)
                if (e.Volume.Contains("."))
                {
                    stateVolume.Text = e.Volume;
                    stateVolume.ForeColor = e.OnAir ? Color.Ivory : Color.Gold;
                }
                else
                {
                    var check = e.Volume.Contains("-");
                    stateVolume.Text = check ? e.Volume.Substring(1) : e.Volume;
                    stateVolume.ForeColor = check ? Color.DeepSkyBlue : Color.Maroon;
                }
            foreach (var kv in e.Trend.OrderBy(o => ran.Next(e.Trend.Count)))
            {
                var label = string.Concat("state", count).FindByName<Label>(this);
                var trend = kv.Value.Contains("-");
                label.Text = trend ? kv.Value.Substring(1) : kv.Value;
                label.ForeColor = trend ? Color.Navy : Color.Maroon;

                if (count++ == 4)
                    break;
            }
        }));
        public void OnReceiveQuotes(object sender, EventHandler.XingAPI.Quotes e) => BeginInvoke(new Action(() =>
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
        public void OnReceiveQuotes(object sender, EventHandler.OpenAPI.Quotes e) => BeginInvoke(new Action(() =>
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
        public void OnReceiveOrderMsg(string message)
        {
            int first = message.IndexOf('.'), last = message.LastIndexOf('.');

            if (first == last)
            {
                this.message.Text = message.Trim();

                return;
            }
            this.message.Text = string.Concat(message.Substring(0, first + 1).Trim(), "\n", message.Substring(first + 1).Trim());
        }
        readonly Random ran;
    }
}