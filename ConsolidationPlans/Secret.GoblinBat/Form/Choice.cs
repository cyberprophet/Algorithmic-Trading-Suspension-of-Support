using System;
using System.Linq;
using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.FindByName;

namespace ShareInvest.Identify
{
    public partial class Choice : Form
    {
        public Choice()
        {
            InitializeComponent();
        }
        public string TempText
        {
            get; private set;
        }
        public int Count
        {
            get; private set;
        }
        internal void OnReceiveText(object sender, Memorize e)
        {
            string.Concat(rb, Count++).FindByName<RadioButton>(this).Text = e.SPrevNext;
        }
        private void Button_Click(object sender, EventArgs e)
        {
            TempText = tableLayoutPanel.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Text;
            Close();
        }
        private const string rb = "radioButton";
    }
}