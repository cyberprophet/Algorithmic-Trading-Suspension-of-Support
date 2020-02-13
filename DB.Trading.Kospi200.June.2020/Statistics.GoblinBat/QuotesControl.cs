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
        }
        public void OnReceiveState(object sender, State e)
        {
            BeginInvoke(new Action(() =>
            {
                stateReceive.Text = e.OnReceive ? "주문가능" : string.Empty;
                stateCount.Text = e.OrderCount;
                var position = e.Quantity.Contains("-");
                stateQuantity.Text = position ? e.Quantity.Substring(1) : e.Quantity;
                stateQuantity.ForeColor = position ? Color.DeepSkyBlue : Color.Maroon;
            }));
        }
        public void OnReceiveQuotes(object sender, Quotes e)
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

                    if (e.OrderNumber.ContainsValue(e.Price[i]))
                    {
                        var number = int.Parse(e.OrderNumber.First(o => o.Value == e.Price[i]).Key).ToString();

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