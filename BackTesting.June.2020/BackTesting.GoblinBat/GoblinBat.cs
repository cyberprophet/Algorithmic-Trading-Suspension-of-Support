using System.Windows.Forms;
using System;

namespace ShareInvest.BackTesting
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
            new BackTesting(35000000);
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