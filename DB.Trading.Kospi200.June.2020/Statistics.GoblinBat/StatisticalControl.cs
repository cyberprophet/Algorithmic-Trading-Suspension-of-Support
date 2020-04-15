using System;
using System.Drawing;
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
                    Time = i == 0 ? 1440 : (uint)string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value,
                    Short = (int)string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value,
                    Long = (int)string.Concat(numeric, i + 20).FindByName<NumericUpDown>(this).Value
                };
            return temp;
        }
        void ButtonClick(object sender, EventArgs e)
        {
            var button = (Button)sender;

            switch (button.Name)
            {
                case start:
                    int value = int.MaxValue;

                    for (int i = 0; i < 10; i++)
                    {
                        var time = i > 0 ? string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value : 1440;

                        if (value > time && string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value < string.Concat(numeric, i + 20).FindByName<NumericUpDown>(this).Value)
                        {
                            value = (int)time;

                            continue;
                        }
                        else if (MessageBox.Show(message, warning, MessageBoxButtons.OK, MessageBoxIcon.Error).Equals(DialogResult.OK))
                            return;
                    }
                    SendStatistics?.Invoke(this, new Statistics(Statistics()));
                    buttonStorage.Text = setting;
                    buttonStorage.ForeColor = Color.Gold;
                    return;

                case storage:
                    if (comboCode.SelectedIndex < 0 || comboCommission.SelectedIndex < 0 || comboStrategy.SelectedIndex < 0)
                    {
                        if (MessageBox.Show(notApplicable, warning, MessageBoxButtons.OK, MessageBoxIcon.Warning).Equals(DialogResult.OK))
                            return;
                    }
                    if (button.ForeColor.Equals(Color.Crimson) == false)
                        SendStatistics?.Invoke(this, new Statistics(new Catalog.Setting
                        {
                            Assets = (ulong)numericAssets.Value,
                            Commission = commission[Array.FindIndex(Commission, o => o.Equals(comboCommission.SelectedItem.ToString()))],
                            Strategy = strategy[Array.FindIndex(strategy, o => o.Equals(comboStrategy.SelectedItem.ToString()))],
                            Code = code[Array.FindIndex(code, o => o.Equals(comboCode.SelectedItem.ToString()))],
                            RollOver = checkRollOver.CheckState
                        }));
                    break;
            }
            button.Text = complete;
            button.ForeColor = Color.Crimson;
        }
        public void OnEventConnect()
        {
            buttonStartProgress.Click += ButtonClick;
            buttonStorage.Click += ButtonClick;
        }
        public void OnEventDisconnect()
        {
            buttonStartProgress.Click -= ButtonClick;
            buttonStorage.Click -= ButtonClick;
        }
        public Specify[] Specifies
        {
            get; private set;
        }
        void CheckRollOverCheckStateChanged(object sender, EventArgs e)
        {
            var button = (CheckBox)sender;

            switch (button.CheckState)
            {
                case CheckState.Checked:
                    button.Text = over;
                    button.ForeColor = Color.Ivory;
                    break;

                case CheckState.Unchecked:
                    button.Text = notUsed;
                    button.ForeColor = Color.Maroon;
                    break;

                case CheckState.Indeterminate:
                    button.Text = auto;
                    button.ForeColor = Color.Navy;
                    button.BackColor = Color.DimGray;
                    return;
            }
            button.BackColor = Color.Transparent;
        }
        string[] Commission
        {
            get; set;
        }
        string[] MaginRate
        {
            get; set;
        }
        readonly string[] code = { code0 };
        readonly string[] strategy = { strategy0 };
        readonly double[] commission = { commission0 };
        readonly double[] magin_rate = { magin_rate0 };
        const string numeric = "numeric";
        const string storage = "buttonStorage";
        const string start = "buttonStartProgress";
        const string auto = "Auto";
        const string notUsed = "NotUsed";
        const string over = "RollOver";
        const string message = "단기 값이 장기 값보다 클 수 없습니다.\n\n확인하시고 다시 설정해주세요.";
        const string notApplicable = "설정 값을 다시 확인하세요.";
        const string warning = "Warning";
        const string code0 = "101Q6000";
        const string strategy0 = "Base";
        const string setting = "Setting";
        const string complete = "Complete";
        const double commission0 = 3e-5;
        const double magin_rate0 = 16.2e-2;
        public event EventHandler<Statistics> SendStatistics;
    }
}