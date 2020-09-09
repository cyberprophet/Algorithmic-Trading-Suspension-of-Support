using System.Collections.Generic;
using System.Windows.Forms;

namespace ShareInvest.Controls
{
    partial class TrendToCashflow : UserControl
    {
        internal TrendToCashflow(Catalog.Codes codes)
        {
            InitializeComponent();
            boxTrend.Text = codes.Name;
            this.codes = codes;
        }
        internal void TransmuteStrategics(string[] strategics)
        {

        }
        internal bool TransmuteStrategics()
        {
            return false;
        }
        internal string TransmuteStrategics(string code)
        {
            if (codes.Code.Equals(code))
            {

            }
            return null;
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
        readonly Catalog.Codes codes;
    }  
}