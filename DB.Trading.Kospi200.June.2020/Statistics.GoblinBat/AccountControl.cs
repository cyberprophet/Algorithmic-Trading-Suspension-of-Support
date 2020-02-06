using System;
using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.FindByName;

namespace ShareInvest.GoblinBatControls
{
    public partial class AccountControl : UserControl
    {
        public AccountControl()
        {
            InitializeComponent();
        }
        public void OnReceiveDeposit(object sender, Deposit e)
        {
            BeginInvoke(new Action(() =>
            {
                for (int i = 0; i < e.DetailDeposit.Length; i++)
                    if (e.DetailDeposit[i].Length > 0)
                        string.Concat("balance", i).FindByName<Label>(this).Text = long.Parse(e.DetailDeposit[i]).ToString("N0");
            }));
        }
    }
}