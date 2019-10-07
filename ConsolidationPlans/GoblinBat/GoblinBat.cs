using System;
using System.Windows.Forms;
using ShareInvest.Control;

namespace ShareInvest
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, EventArgs e)
        {
            if (Kospi200F.Checked)
                new Kospi200();

            SetVisibleCore(false);
        }
    }
}