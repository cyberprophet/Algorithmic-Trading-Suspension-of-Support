using System;
using System.Windows.Forms;

namespace ShareInvest
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
        }
        void GoblinBatResize(object sender, EventArgs e) => BeginInvoke(new Action(() =>
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {

            }
        }));
    }
}