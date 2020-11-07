using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShareInvest
{
    sealed partial class SecuritiesAPI : Form
    {
        internal bool Repeat
        {
            get; private set;
        }
        internal SecuritiesAPI()
        {
            InitializeComponent();
        }
        void TimerTick(object sender, EventArgs e)
        {
            if (Connect == null)
            {
                Connect = new OpenAPI.ConnectAPI();
                var control = (Control)Connect;
                Controls.Add(control);
                control.Dock = DockStyle.Fill;
                control.Show();
                Debug.WriteLine(Connect.CommConnect());
            }
            else if (Count++ > 50)
            {
                Connect.Dispose();
                Connect = null;
                Dispose();
                Repeat = true;
            }
        }
        OpenAPI.ConnectAPI Connect
        {
            get; set;
        }
        int Count
        {
            get; set;
        }
    }
}