using System;
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

                    if (e.OrderNumber.TryGetValue(e.Price[i], out string value))
                    {
                        if (temporary.Text.Equals(value))
                            continue;

                        temporary.Text = value;
                    }
                    else
                        temporary.Text = string.Empty;
                }
                Application.DoEvents();
            }));
        }
    }
}