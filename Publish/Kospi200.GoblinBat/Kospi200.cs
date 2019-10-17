using System;
using System.Windows.Forms;
using ShareInvest.Control;
using ShareInvest.SelectableMessageBox;

namespace ShareInvest.Kospi200
{
    public partial class Kospi200 : Form
    {
        public Kospi200()
        {
            ChooseResult(Choose.Show("Please Select the Button You Want to Proceed. . .", "ShareInvest GoblinBat Kospi200 TradingSystem", "Trading", "BackTest", "Exit"));
            InitializeComponent();
        }
        private void ChooseResult(DialogResult result)
        {
            if (result.Equals(DialogResult.Yes))
            {
            }
            else if (result.Equals(DialogResult.No))
                Pro = new Progress();

            else
            {
                Pro.Dispose();
                Dispose();
                Environment.Exit(0);
            }
        }
        private Progress Pro
        {
            get; set;
        }
    }
}