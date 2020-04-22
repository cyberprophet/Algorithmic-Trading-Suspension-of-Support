using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.EventHandler.BackTesting;
using ShareInvest.FindByName;

namespace ShareInvest.GoblinBatControls
{
    public partial class StatisticalControl : UserControl
    {
        public StatisticalControl(string code, string[] strategy, double[] rate, double[] commission)
        {
            ran = new Random();
            this.code = code;
            this.strategy = strategy;
            this.rate = rate;
            this.commission = commission;
            Commission = new string[commission.Length];
            MaginRate = new string[rate.Length];

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
            for (int i = 0; i < rate.Length; i++)
            {
                var temp = rate[i].ToString("P2");

                if (temp.Split('.')[1].Substring(1, 1).Equals("0"))
                {
                    MaginRate[i] = rate[i].ToString("P1");

                    continue;
                }
                MaginRate[i] = temp;
            }
            InitializeComponent();
            comboCode.Items.Add(code);
            comboStrategy.Items.AddRange(strategy);
            comboCommission.Items.AddRange(Commission);
            comboMarginRate.Items.AddRange(MaginRate);
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
        public Specify[] Statistics(Specify[] specifies)
        {
            var temp = new Dictionary<uint, int[]>();
            int i = 0;

            if (specifies.Any(o => o.Assets == 0))
            {
                var basic = new Specify[10];
                var ro = new bool[] { true, false };

                for (i = 0; i < basic.Length; i++)
                    basic[i] = new Specify
                    {
                        Assets = (ulong)numericAssets.Value,
                        Code = code,
                        Commission = commission[0],
                        MarginRate = rate[0],
                        Strategy = strategy[ran.Next(0, strategy.Length - 1)],
                        RollOver = ro[ran.Next(0, ro.Length)],
                        Time = i == 0 ? 1440 : (uint)string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value,
                        Short = (int)string.Concat(numeric, 10 + i).FindByName<NumericUpDown>(this).Value,
                        Long = (int)string.Concat(numeric, 20 + i++).FindByName<NumericUpDown>(this).Value
                    };
                return basic;
            }
            foreach (var specify in specifies)
            {
                numericAssets.Value = specify.Assets;
                var commission = specify.Commission.ToString("P4");
                commission = commission.Substring(5, 1).Equals("0") ? specify.Commission.ToString("P3") : commission;
                var margin = specify.MarginRate.ToString("P2");
                margin = margin.Split('.')[1].Substring(1, 1).Equals("0") ? specify.MarginRate.ToString("P1") : margin;
                checkRollOver.CheckState = specify.RollOver ? CheckState.Checked : CheckState.Unchecked;
                temp[specify.Time] = new int[]
                {
                    specify.Short,
                    specify.Long
                };
                if (comboCode.Items.Contains(specify.Code) == false)
                    comboCode.Items.Add(specify.Code);

                if (comboCommission.Items.Contains(commission) == false)
                    comboCommission.Items.Add(commission);

                if (comboMarginRate.Items.Contains(margin) == false)
                    comboMarginRate.Items.Add(margin);

                if (comboStrategy.Items.Contains(specify.Strategy) == false)
                    comboStrategy.Items.Add(specify.Strategy);

                comboCommission.SelectedItem = commission;
                comboMarginRate.SelectedItem = margin;
                comboCode.SelectedItem = specify.Code;
                comboStrategy.SelectedItem = specify.Strategy;
            }
            foreach (var kv in temp.OrderByDescending(o => o.Key))
            {
                if (i > 0)
                    string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value = kv.Key;

                string.Concat(numeric, 10 + i).FindByName<NumericUpDown>(this).Value = kv.Value[0];
                string.Concat(numeric, 20 + i++).FindByName<NumericUpDown>(this).Value = kv.Value[1];
            }
            return specifies;
        }
        Catalog.DataBase.ImitationGame Statistics()
        {
            int i = 0;
            var ro = new bool[] { true, false };

            return new Catalog.DataBase.ImitationGame
            {
                Assets = (long)numericAssets.Value,
                Code = comboCode.SelectedIndex < 0 ? code : comboCode.SelectedItem.ToString(),
                Commission = comboCommission.SelectedIndex < 0 ? commission[0] : commission[Array.FindIndex(Commission, o => o.Equals(comboCommission.SelectedItem.ToString()))],
                MarginRate = comboMarginRate.SelectedIndex < 0 ? rate[0] : rate[Array.FindIndex(MaginRate, o => o.Equals(comboMarginRate.SelectedItem.ToString()))],
                Strategy = comboStrategy.SelectedIndex < 0 && comboStrategy.SelectedItem.ToString().Equals(auto) ? strategy[ran.Next(0, strategy.Length - 1)] : comboStrategy.SelectedItem.ToString(),
                RollOver = checkRollOver.CheckState.Equals(CheckState.Indeterminate) ? ro[ran.Next(0, ro.Length)] : checkRollOver.Checked,
                BaseTime = 1440,
                BaseShort = (int)string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value,
                BaseLong = (int)string.Concat(numeric, i++ + 20).FindByName<NumericUpDown>(this).Value,
                NonaTime = (int)string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value,
                NonaShort = (int)string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value,
                NonaLong = (int)string.Concat(numeric, i++ + 20).FindByName<NumericUpDown>(this).Value,
                OctaTime = (int)string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value,
                OctaShort = (int)string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value,
                OctaLong = (int)string.Concat(numeric, i++ + 20).FindByName<NumericUpDown>(this).Value,
                HeptaTime = (int)string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value,
                HeptaShort = (int)string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value,
                HeptaLong = (int)string.Concat(numeric, i++ + 20).FindByName<NumericUpDown>(this).Value,
                HexaTime = (int)string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value,
                HexaShort = (int)string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value,
                HexaLong = (int)string.Concat(numeric, i++ + 20).FindByName<NumericUpDown>(this).Value,
                PentaTime = (int)string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value,
                PentaShort = (int)string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value,
                PentaLong = (int)string.Concat(numeric, i++ + 20).FindByName<NumericUpDown>(this).Value,
                QuadTime = (int)string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value,
                QuadShort = (int)string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value,
                QuadLong = (int)string.Concat(numeric, i++ + 20).FindByName<NumericUpDown>(this).Value,
                TriTime = (int)string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value,
                TriShort = (int)string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value,
                TriLong = (int)string.Concat(numeric, i++ + 20).FindByName<NumericUpDown>(this).Value,
                DuoTime = (int)string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value,
                DuoShort = (int)string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value,
                DuoLong = (int)string.Concat(numeric, i++ + 20).FindByName<NumericUpDown>(this).Value,
                MonoTime = (int)string.Concat(numeric, i).FindByName<NumericUpDown>(this).Value,
                MonoShort = (int)string.Concat(numeric, i + 10).FindByName<NumericUpDown>(this).Value,
                MonoLong = (int)string.Concat(numeric, i + 20).FindByName<NumericUpDown>(this).Value
            };
        }
        void ButtonClick(object sender, EventArgs e) => BeginInvoke(new Action(() =>
        {
            SuspendLayout();
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
                    Cursor = Cursors.WaitCursor;
                    Application.DoEvents();
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
                            Assets = (long)numericAssets.Value,
                            Commission = commission[Array.FindIndex(Commission, o => o.Equals(comboCommission.SelectedItem.ToString()))],
                            Strategy = strategy[Array.FindIndex(strategy, o => o.Equals(comboStrategy.SelectedItem.ToString()))],
                            Code = code,
                            RollOver = checkRollOver.CheckState
                        }));
                    break;
            }
            button.Text = complete;
            button.ForeColor = Color.Crimson;
            ResumeLayout();
        }));
        void CheckRollOverCheckStateChanged(object sender, EventArgs e) => BeginInvoke(new Action(() =>
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
        }));
        string[] Commission
        {
            get; set;
        }
        string[] MaginRate
        {
            get; set;
        }
        readonly Random ran;
        readonly string code;
        readonly double[] rate;
        readonly double[] commission;
        readonly string[] strategy;
        const string numeric = "numeric";
        const string storage = "buttonStorage";
        const string start = "buttonStartProgress";
        const string auto = "Auto";
        const string notUsed = "NotUsed";
        const string over = "RollOver";
        const string message = "단기 값이 장기 값보다 클 수 없습니다.\n\n확인하시고 다시 설정해주세요.";
        const string notApplicable = "설정 값을 다시 확인하세요.";
        const string warning = "Warning";
        const string setting = "Setting";
        const string complete = "Complete";
        public event EventHandler<Statistics> SendStatistics;
    }
}