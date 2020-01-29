using System;
using System.Windows.Forms;

namespace ShareInvest.BackTesting
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
            new BackTesting(35000000L);
            Environment.Exit(0);
        }
        private void GoblinBatResize(object sender, EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;
            }
        }
    }
}