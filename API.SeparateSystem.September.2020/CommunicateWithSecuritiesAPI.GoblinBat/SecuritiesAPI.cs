using System.Drawing;
using System.Windows.Forms;

using ShareInvest.Catalog;

namespace ShareInvest
{
    partial class SecuritiesAPI : Form
    {
        internal SecuritiesAPI(ISecuritiesAPI com)
        {
            this.com = com;
            InitializeComponent();
            StartProgress();
        }
        void StartProgress()
        {
            var control = (Control)com;
            Controls.Add(control);
            control.Dock = DockStyle.Fill;
            control.Show();
            Size = new Size(375, 275);
            Opacity = 0.8135;
            BackColor = Color.FromArgb(121, 133, 130);
            CenterToScreen();
        }
        readonly ISecuritiesAPI com;
    }
}