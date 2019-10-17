using System;
using System.Windows.Forms;
using ShareInvest.SelectableMessageBox;

namespace ShareInvest.Publish
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
            ChooseResult(Choose.Show("Please Select the Button You Want to Proceed. . .", "ShareInvest GoblinBat Kosdaq150 TradingSystem", "Trading", "BackTest", "Exit"));
        }
        private void ChooseResult(DialogResult result)
        {
            if (result == DialogResult.Yes)
            {
            }
            else if (result == DialogResult.No)
            {

            }
            else
            {
                Dispose();
                Environment.Exit(0);
            }
        }
    }
}