using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareInvest.Controls
{
    public partial class TrendFollowingBasicFutures : UserControl
    {
        public TrendFollowingBasicFutures(double marginRate)
        {
            this.marginRate = marginRate;
            InitializeComponent();
            numericQuantityShort.Leave += OutOfFocusNumericQuantity;
            numericQuantityLong.Leave += OutOfFocusNumericQuantity;
        }
        void OutOfFocusNumericQuantity(object sender, EventArgs e)
        {
            if (sender is CheckBox check)
                switch (check.CheckState)
                {
                    case CheckState.Checked:
                        check.Text = "Use";
                        check.FlatAppearance.BorderColor = Color.Maroon;
                        break;

                    case CheckState.Unchecked:
                        check.Text = "RollOver";
                        check.FlatAppearance.BorderColor = Color.Navy;
                        break;
                }
            else if (sender is NumericUpDown)
            {
                var amount = (uint)(numericQuantityShort.Value > numericQuantityLong.Value ? numericQuantityShort.Value : numericQuantityLong.Value);
                labelAvailable.Text = (transactionMutiplier * amount * 298.75 * marginRate).ToString("C0");
            }
        }
        const uint transactionMutiplier = 0x3D090;
        readonly double marginRate;
    }
}