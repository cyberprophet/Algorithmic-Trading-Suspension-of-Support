using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ShareInvest.FindByName;

namespace ShareInvest.Controls
{
    partial class TrendToCashflow : UserControl
    {
        internal TrendToCashflow(Catalog.Codes codes)
        {
            InitializeComponent();
            boxTrend.Text = codes.Name;
            code = codes.Code;
        }
        internal void TransmuteStrategics(string[] strategics)
        {

        }
        internal bool TransmuteStrategics() => numericShort.Value < numericLong.Value;
        internal string TransmuteStrategics(string code)
        {
            if (this.code.Equals(code))
            {
                var sb = new StringBuilder("TC|").Append(code);

                foreach (var str in strategics)
                    sb.Append('|').Append(string.Concat(numeric, str).FindByName<NumericUpDown>(this).Value);

                return sb.ToString();
            }
            return string.Empty;
        }
        internal IEnumerable<RadioButton> RadioButtons
        {
            get
            {
                foreach (var radio in panel.Controls)
                    if (radio is RadioButton button)
                        yield return button;
            }
        }
        readonly string code;
    }
}