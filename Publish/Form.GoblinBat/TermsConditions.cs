using System;
using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.TermsAndConditions;

namespace ShareInvest.Control
{
    public partial class TermsConditions : UserControl
    {
        public TermsConditions()
        {
            InitializeComponent();
            label.Text = new Terms().Conditions;
        }
        private void Agree_Click(object sender, EventArgs e)
        {
            SendQuit?.Invoke(this, new ForceQuit(1));
        }
        private void Refuse_Click(object sender, EventArgs e)
        {
            Dispose();
            Environment.Exit(0);
        }
        public event EventHandler<ForceQuit> SendQuit;
    }
}