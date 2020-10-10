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
            transactionMutiplier = GetTransactionMultiplier(codes.Code);
            price = codes.Price;
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
        internal bool TransmuteStrategics()
        {
            if (numericBaseShort.Value < numericBaseLong.Value && numericMinute.Value < 0x2D1 && numericMinuteShort.Value < numericMinuteLong.Value && numericReactionShort.Value < 0x65 && numericReactionLong.Value < 0x65)
                return true;

            else
                return false;
        }
        internal string TransmuteStrategics(string code) => string.Concat("TF", code, checkRollover.Checked ? 1 : 0, numericBaseShort.Value, '.', numericBaseLong.Value, '.', numericMinute.Value, '.', numericMinuteShort.Value, '.', numericMinuteLong.Value, '.', numericReactionShort.Value, '.', numericReactionLong.Value, '.', numericQuantityShort.Value, '.', numericQuantityLong.Value);
        uint GetTransactionMultiplier(string code)
        {
            if (code[1].Equals('0'))
                switch (code[2])
                {
                    case '1':
                        return 0x3D090;

                    case '5':
                        return 0xC350;

                    case '6':
                        return 0x2710;
                }
            return 0;
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

                if (price.Contains(".") && double.TryParse(price, out double current))
                    labelAvailable.Text = (transactionMutiplier * amount * current * marginRate).ToString("C0");

                else if (int.TryParse(this.price, out int price))
                    labelAvailable.Text = (amount * price * marginRate).ToString("C0");
            }
        }
        readonly uint transactionMutiplier;
        readonly double marginRate;
        readonly string price;
    }
}