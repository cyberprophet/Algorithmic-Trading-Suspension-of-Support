using System;
using System.Text;
using System.Windows.Forms;

using ShareInvest.FindByName;

namespace ShareInvest.Controls
{
    partial class TrendsInValuation : UserControl
    {
        string UseMnemonic(string name) => name.Insert(Array.FindIndex(name.ToCharArray(), o => o.Equals('&')), @"&");
        internal TrendsInValuation(Catalog.Codes codes)
        {
            InitializeComponent();
            random = new Random();
            boxTrend.Text = codes.Name.Contains("&") ? UseMnemonic(codes.Name) : codes.Name;
            code = codes.Code;

            foreach (var check in panel.Controls)
                if (check is CheckBox button)
                    button.Checked = random.Next(0, 4) == 0;
        }
        internal void TransmuteStrategics(string[] strategics)
        {
            for (int i = 0; i < strategics.Length; i++)
                if (decimal.TryParse(strategics[i], out decimal value))
                    string.Concat(numeric, this.strategics[i]).FindByName<NumericUpDown>(this).Value = value;
        }
        internal bool TransmuteStrategics()
        {
            var pass = false;

            foreach (var check in panel.Controls)
                if (check is CheckBox button && button.Checked)
                    pass = true;

            return numericShort.Value < numericLong.Value && pass;
        }
        internal string TransmuteStrategics(string code)
        {
            if (this.code.Equals(code))
            {
                var sb = new StringBuilder("TV|").Append(code);

                foreach (var str in strategics)
                    sb.Append('|').Append(string.Concat(numeric, str).FindByName<NumericUpDown>(this).Value);

                return sb.ToString();
            }
            return string.Empty;
        }
        internal string CheckValues
        {
            get
            {
                var str = new char[4];

                foreach (var check in panel.Controls)
                    if (check is CheckBox button)
                    {
                        var value = button.Checked.ToString()[0];

                        switch (Enum.ToObject(typeof(CheckBoxButtons), button?.Name[5]))
                        {
                            case CheckBoxButtons.매출:
                                str[0] = value;
                                break;

                            case CheckBoxButtons.영업이익:
                                str[1] = value;
                                break;

                            case CheckBoxButtons.순이익:
                                str[2] = value;
                                break;

                            case CheckBoxButtons.현금흐름:
                                str[3] = value;
                                break;
                        }
                    }
                return string.Concat(str[0], str[1], str[2], str[3]);
            }
        }
        readonly string code;
        readonly Random random;
    }
}