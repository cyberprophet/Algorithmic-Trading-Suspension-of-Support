using System;
using System.Drawing;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.FindByName;

namespace ShareInvest.Controls
{
    partial class TrendFollowingBasicFutures : UserControl
    {
        internal TrendFollowingBasicFutures(Codes codes)
        {
            marginRate = codes.MarginRate;
            price = codes.Price;
            code = codes.Code;
            InitializeComponent();
            numericQuantityShort.Leave += OutOfFocusNumericQuantity;
            numericQuantityLong.Leave += OutOfFocusNumericQuantity;
        }
        internal void TransmuteStrategics(bool check, string[] strategics)
        {
            checkRollover.Checked = check;
            var str = new string[] { "BaseShort", "BaseLong", "Minute", "MinuteShort", "MinuteLong", "ReactionShort", "ReactionLong", "QuantityShort", "QuantityLong" };

            for (int i = 0; i < strategics.Length; i++)
                if (int.TryParse(strategics[i], out int value))
                    string.Concat("numeric", str[i]).FindByName<NumericUpDown>(this).Value = value;
        }
        internal string TransmuteStrategics(string code) => string.Concat("TF", code, checkRollover.Checked ? 1 : 0, numericBaseShort.Value, '.', numericBaseLong.Value, '.', numericMinute.Value, '.', numericMinuteShort.Value, '.', numericMinuteLong.Value, '.', numericReactionShort.Value, '.', numericReactionLong.Value, '.', numericQuantityShort.Value, '.', numericQuantityLong.Value);
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

                if (double.TryParse(price, out double current))
                {
                    if (price.Contains("."))
                    {
                        if (code.Substring(2, 1).Equals("1"))
                            labelAvailable.Text = (transactionMutiplier * amount * current * marginRate).ToString("C0");
                    }
                    else
                        labelAvailable.Text = (amount * current * marginRate).ToString("C0");
                }
            }
        }
        const uint transactionMutiplier = 0x3D090;
        readonly double marginRate;
        readonly string price;
        readonly string code;
    }
}