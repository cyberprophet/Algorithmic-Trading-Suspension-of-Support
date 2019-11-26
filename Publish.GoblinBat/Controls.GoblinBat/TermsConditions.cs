using System;
using System.Drawing;
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
            tableLayoutPanel.BackColor = Color.Black;
            condition = new Terms();
            label.Text = condition.Conditions;
        }
        private void Agree_Click(object sender, EventArgs e)
        {
            if (checkBox.CheckState.Equals(CheckState.Checked))
                SendQuit?.Invoke(this, new ForceQuit(1));
        }
        private void Refuse_Click(object sender, EventArgs e)
        {
            Dispose();
            Environment.Exit(0);
        }
        private void CheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox.CheckState.Equals(CheckState.Unchecked))
                label.Text = condition.Rest;

            else if (checkBox.CheckState.Equals(CheckState.Indeterminate))
                label.Text = condition.Conditions;

            else
                label.Text = condition.Sentance;
        }
        private readonly Terms condition;
        public event EventHandler<ForceQuit> SendQuit;
    }
}