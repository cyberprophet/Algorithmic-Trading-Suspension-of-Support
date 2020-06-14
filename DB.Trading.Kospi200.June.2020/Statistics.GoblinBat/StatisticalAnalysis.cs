using System;
using System.Windows.Forms;

using ShareInvest.Catalog.XingAPI;
using ShareInvest.FindByName;

namespace ShareInvest.GoblinBatControls
{
    public partial class StatisticalAnalysis : UserControl
    {
        public StatisticalAnalysis() => InitializeComponent();
        public Specify[] Statistics(string code)
        {
            var commission = string.IsNullOrEmpty(emptyCommission.Text) == false && double.TryParse(emptyCommission.Text, out double ec) ? (ec / 100) : 3e-5;
            var temp = new Specify[5];

            for (int i = 0; i < temp.Length; i++)
                temp[i] = new Specify
                {
                    Assets = (ulong)numericAssets.Value,
                    Code = string.IsNullOrEmpty(emptyCode.Text) ? code : emptyCode.Text,
                    Commission = commission,
                    Strategy = strategy.Text,
                    RollOver = checkRollOver.Checked,
                    Time = i == 4 && uint.TryParse(emptyNum.Text.Replace(",", string.Empty), out uint time) ? time : (uint)string.Concat("numeric", i).FindByName<NumericUpDown>(this).Value,
                    Short = (int)string.Concat("numeric", i + 10).FindByName<NumericUpDown>(this).Value,
                    Long = (int)string.Concat("numeric", i + 20).FindByName<NumericUpDown>(this).Value
                };
            return temp;
        }
        public void OnEventConnect() => button.Click += ButtonClick;
        public void OnEventDisconnect() => button.Click -= ButtonClick;
        void ButtonClick(object sender, EventArgs e) => emptyCode.Text = "101Q6000";
    }
}