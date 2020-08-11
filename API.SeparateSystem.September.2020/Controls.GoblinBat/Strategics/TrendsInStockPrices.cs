using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.FindByName;

namespace ShareInvest.Controls
{
    partial class TrendsInStockPrices : UserControl
    {
        internal TrendsInStockPrices(Codes codes)
        {
            code = codes.Code;
            name = codes.Name;
            InitializeComponent();
            label.Text = name;
        }
        internal void TransmuteStrategics(string[] strategics)
        {
            for (int i = 0; i < this.strategics.Length; i++)
                if (i < 7 && int.TryParse(strategics[i], out int value))
                {
                    decimal temp = decimal.MinValue;

                    if (i == 3 || i == 4)
                        temp = (decimal)value / 0x64;

                    ConvertToFindTheVariable(i, this.strategics[i]).FindByName<NumericUpDown>(this).Value = temp > decimal.MinValue ? temp : value;
                }
                else if (char.TryParse(strategics[i], out char param))
                    ConvertToFindTheVariable(i, Enum.GetName(typeof(StrategicsCode), param)).FindByName<RadioButton>(this).Checked = true;
        }
        internal bool TransmuteStrategics()
        {
            if (numericShort.Value < numericLong.Value && numericRealizeProfit.Value >= 1 && numericAdditionalPurchase.Value >= 1 && numericQuantity.Value > 0 && numericQuoteUnit.Value > 0)
            {
                var stack = new Stack<StrategicsCode>();

                for (int i = 7; i < strategics.Length; i++)
                    foreach (Control control in ConvertToFindTheVariable(i, strategics[i]).Replace(radio, group).FindByName<GroupBox>(this).Controls)
                        if (control is RadioButton radio && radio.Checked && Enum.TryParse(radio.Name.Substring(5), out StrategicsCode sc))
                            stack.Push(sc);

                return stack.Count == 3;
            }
            else
                return false;
        }
        internal string TransmuteStrategics(string code)
        {
            if (this.code.Equals(code))
            {
                var sb = new StringBuilder();

                for (int i = 0; i < strategics.Length; i++)
                    if (i < 7)
                    {
                        var param = ConvertToFindTheVariable(i, strategics[i]).FindByName<NumericUpDown>(this).Value;

                        if (i == 3 || i == 4)
                            param *= 0x64;

                        sb.Append((int)param).Append('.');
                    }
                    else
                        foreach (Control control in ConvertToFindTheVariable(i, strategics[i]).Replace(radio, group).FindByName<GroupBox>(this).Controls)
                            if (control is RadioButton radio && radio.Checked && Enum.TryParse(radio.Name.Substring(5), out StrategicsCode sc))
                                sb.Append((char)sc).Append('.');

                return string.Concat("TS", code, sb.Remove(sb.Length - 1, 1));
            }
            return string.Empty;
        }
        string ConvertToFindTheVariable(int index, string param) => string.Concat(index > 6 ? radio : numeric, param);
        readonly string code;
        readonly string name;
    }
}