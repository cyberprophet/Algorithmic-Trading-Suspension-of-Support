using System;
using System.Windows.Forms;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.EventHandler.BackTesting;
using ShareInvest.FindByName;

namespace ShareInvest.GoblinBatControls
{
    public partial class StatisticalControl : UserControl
    {
        public StatisticalControl()
        {
            Commission = new string[commission.Length];
            MaginRate = new string[magin_rate.Length];

            for (int i = 0; i < commission.Length; i++)
            {
                var temp = commission[i].ToString("P4");

                if (temp.Substring(5, 1).Equals("0"))
                {
                    Commission[i] = commission[i].ToString("P3");

                    continue;
                }
                Commission[i] = temp;
            }
            for (int i = 0; i < magin_rate.Length; i++)
            {
                var temp = magin_rate[i].ToString("P2");

                if (temp.Split('.')[1].Substring(1, 1).Equals("0"))
                {
                    MaginRate[i] = magin_rate[i].ToString("P1");

                    continue;
                }
                MaginRate[i] = temp;
            }
            InitializeComponent();
            comboCode.Items.AddRange(code);
            comboStrategy.Items.AddRange(strategy);
            comboCommission.Items.AddRange(Commission);
            comboMarginRate.Items.AddRange(MaginRate);
            Specifies = Statistics();
        }
        public void OnEventConnect()
        {
            buttonStartProgress.Click += ButtonClick;
        }
        public void OnEventDisconnect()
        {
            buttonStartProgress.Click -= ButtonClick;
        }
        public Specify[] Specifies
        {
            get; private set;
        }
        Specify[] Statistics()
        {
            var temp = new Specify[10];

            for (int i = 0; i < temp.Length; i++)
                temp[i] = new Specify
                {
                    Assets = (ulong)numericAssets.Value,
                    Code = comboCode.SelectedIndex < 0 ? code[0] : comboCode.SelectedItem.ToString(),
                    Commission = comboCommission.SelectedIndex < 0 ? commission[0] : commission[Array.FindIndex(Commission, o => o.Equals(comboCommission.SelectedItem.ToString()))],
                    MarginRate = comboMarginRate.SelectedIndex < 0 ? magin_rate[0] : magin_rate[Array.FindIndex(MaginRate, o => o.Equals(comboMarginRate.SelectedItem.ToString()))],
                    Strategy = comboStrategy.SelectedIndex < 0 ? strategy[0] : comboStrategy.SelectedItem.ToString(),
                    RollOver = checkRollOver.Checked,
                    Time = i == 0 ? 1440 : (uint)string.Concat("numeric", i).FindByName<NumericUpDown>(this).Value,
                    Short = (int)string.Concat("numeric", i + 10).FindByName<NumericUpDown>(this).Value,
                    Long = (int)string.Concat("numeric", i + 20).FindByName<NumericUpDown>(this).Value
                };
            return temp;
        }
        void ButtonClick(object sender, EventArgs e)
        {
            SendStatistics?.Invoke(this, new Statistics(Statistics()));
        }
        string[] Commission
        {
            get; set;
        }
        string[] MaginRate
        {
            get; set;
        }
        readonly string[] code = { code_0 };
        readonly string[] strategy = { strategy_0 };
        readonly double[] commission = { commission_0 };
        readonly double[] magin_rate = { magin_rate_0 };
        const string code_0 = "101Q6000";
        const string strategy_0 = "Base";
        const double commission_0 = 3e-5;
        const double magin_rate_0 = 16.2e-2;
        public event EventHandler<Statistics> SendStatistics;
    }
}