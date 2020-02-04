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
                SuspendLayout();
                quotes10.Text = DateTime.ParseExact(e.Time, "HHmmss", null).ToString("HH : mm : ss");

                for (int i = 0; i < e.Price.Length; i++)
                {
                    if (e.OrderNumber.TryGetValue(e.Price[i], out string value))
                        string.Concat("order", i).FindByName<Label>(this).Text = value;

                    string.Concat("quotes", i).FindByName<Label>(this).Text = e.Price[i].ToString();
                }
                Application.DoEvents();
                ResumeLayout(true);
            }));
        }
    }
}