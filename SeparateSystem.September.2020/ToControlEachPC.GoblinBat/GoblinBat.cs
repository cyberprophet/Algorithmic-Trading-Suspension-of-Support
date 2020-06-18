using System;
using System.Windows.Forms;

namespace ShareInvest
{
    partial class GoblinBat : Form
    {
        internal GoblinBat()
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