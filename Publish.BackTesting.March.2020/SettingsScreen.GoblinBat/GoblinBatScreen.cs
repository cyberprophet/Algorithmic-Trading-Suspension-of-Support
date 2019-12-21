using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.AutoSetting;
using ShareInvest.BackTesting.Analysis;
using ShareInvest.Communication;
using ShareInvest.Information;
using ShareInvest.Log.Message;
using ShareInvest.MassProcessingTechnology;
using ShareInvest.RemainingDate;
using ShareInvest.RetrieveOptions;
using ShareInvest.SettingValue;

namespace ShareInvest.BackTesting.SettingsScreen
{
    public partial class GoblinBatScreen : UserControl
    {
        public GoblinBatScreen(int count, IAsset asset)
        {
            this.count = count;
            this.asset = asset;
            InitializeComponent();
            BackColor = Color.FromArgb(121, 133, 130);
            numericCapital.Value = asset.Assets;
            Estimate = new List<string>(32);
        }
        public void SetProgress(Progress pro)
        {
            this.pro = pro;
            SuspendLayout();
            BeginInvoke(new Action(() =>
            {
                ran = new Random();
                SetLabelUsed();
                SetNumeric(new RecallSettings().GetSettingValue());
            }));
            timer.Start();
            Application.DoEvents();
            ResumeLayout();
        }
        private void SetLabelUsed()
        {
            foreach (var temp in Enum.GetValues(typeof(IFindbyName.LabelUsed)))
            {
                uint value = uint.Parse(asset.Temp[(int)temp + 2]);
                temp.ToString().FindByName<Label>(this).Text = value.ToString("N0");

                if ((int)temp != 0 && (int)temp != 2)
                {
                    CheckBox check = Enum.GetName(typeof(IFindbyName.CheckBoxUsed), (int)temp).FindByName<CheckBox>(this);
                    check.Checked = true;
                    check.CheckStateChanged += CheckBoxClick;
                    check.CheckState = value > 0 ? CheckState.Checked : CheckState.Unchecked;
                }
            }
        }
        private void CheckBoxClick(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            bool day = cb.Name.Equals("checkBoxShortDay") || cb.Name.Equals("checkBoxLongDay"), check = cb.Name.Equals("checkBoxBase") || cb.Name.Equals("checkBoxSigma") || cb.Name.Equals("checkBoxPercent") || cb.Name.Equals("checkBoxMax");
            int index = (int)Enum.Parse(typeof(IFindbyName.CheckBoxUsed), cb.Name);
            NumericUpDown down = Enum.GetName(typeof(IFindbyName.Numeric), 3 * index).FindByName<NumericUpDown>(this);
            NumericUpDown up = Enum.GetName(typeof(IFindbyName.Numeric), 3 * index + 2).FindByName<NumericUpDown>(this);
            down.Minimum = 0;
            up.Minimum = 0;

            if (cb.CheckState.Equals(CheckState.Checked))
            {
                cb.ForeColor = Color.Gold;
                down.Value = ran.Next((int)down.Minimum, (int)down.Maximum);
                up.Value = ran.Next((int)up.Minimum, (int)up.Maximum);

                if (check)
                {
                    checkBoxBase.CheckState = CheckState.Checked;
                    checkBoxSigma.CheckState = CheckState.Checked;
                    checkBoxPercent.CheckState = CheckState.Checked;
                    checkBoxMax.CheckState = CheckState.Checked;
                }
                if (day)
                {
                    checkBoxShortDay.CheckState = CheckState.Checked;
                    checkBoxLongDay.CheckState = CheckState.Checked;
                }
                return;
            }
            down.Value = 0;
            up.Value = 0;
            cb.ForeColor = Color.Maroon;

            if (check)
            {
                checkBoxBase.CheckState = CheckState.Unchecked;
                checkBoxSigma.CheckState = CheckState.Unchecked;
                checkBoxPercent.CheckState = CheckState.Unchecked;
                checkBoxMax.CheckState = CheckState.Unchecked;
            }
            if (day)
            {
                checkBoxShortDay.CheckState = CheckState.Unchecked;
                checkBoxLongDay.CheckState = CheckState.Unchecked;
            }
        }
        private void SetNumeric(string[] param)
        {
            foreach (var name in Enum.GetValues(typeof(IFindbyName.Numeric)))
            {
                NumericUpDown temp = name.ToString().FindByName<NumericUpDown>(this);
                temp.Minimum = 0;
                temp.ThousandsSeparator = true;
                temp.Maximum = name switch
                {
                    IFindbyName.Numeric.numericPST => 200,
                    IFindbyName.Numeric.numericIST => 50,
                    IFindbyName.Numeric.numericDST => 500,
                    IFindbyName.Numeric.numericPSD => 15,
                    IFindbyName.Numeric.numericISD => 10,
                    IFindbyName.Numeric.numericDSD => 50,
                    IFindbyName.Numeric.numericPLT => 1500,
                    IFindbyName.Numeric.numericILT => 1500,
                    IFindbyName.Numeric.numericDLT => 3000,
                    IFindbyName.Numeric.numericPLD => 500,
                    IFindbyName.Numeric.numericILD => 500,
                    IFindbyName.Numeric.numericDLD => 5000,
                    IFindbyName.Numeric.numericPR => 100,
                    IFindbyName.Numeric.numericIR => 100,
                    IFindbyName.Numeric.numericDR => 200,
                    IFindbyName.Numeric.numericPH => 5,
                    IFindbyName.Numeric.numericIH => 5,
                    IFindbyName.Numeric.numericDH => 5,
                    IFindbyName.Numeric.numericPB => 1000,
                    IFindbyName.Numeric.numericIB => 2500,
                    IFindbyName.Numeric.numericDB => 5000,
                    IFindbyName.Numeric.numericPS => 20,
                    IFindbyName.Numeric.numericIS => 30,
                    IFindbyName.Numeric.numericDS => 50,
                    IFindbyName.Numeric.numericPP => 100,
                    IFindbyName.Numeric.numericIP => 200,
                    IFindbyName.Numeric.numericDP => 300,
                    IFindbyName.Numeric.numericPM => 100,
                    IFindbyName.Numeric.numericIM => 100,
                    IFindbyName.Numeric.numericDM => 200,
                    IFindbyName.Numeric.numericPQ => 50,
                    IFindbyName.Numeric.numericIQ => 50,
                    IFindbyName.Numeric.numericDQ => 100,
                    IFindbyName.Numeric.numericPT => 50,
                    IFindbyName.Numeric.numericIT => 50,
                    IFindbyName.Numeric.numericDT => 100,
                    _ => throw new Exception()
                };
                temp.Value = int.Parse(param[(int)name]);
                temp.Increment = 1;
            }
        }
        private StrategySetting GetNumericValue(Array array)
        {
            int[] temp = new int[array.Length];

            foreach (var name in array)
                temp[(int)name] = (int)name.ToString().FindByName<NumericUpDown>(this).Value;

            return new StrategySetting
            {
                ShortTick = SetValue(temp[0], temp[1], temp[2]),
                ShortDay = SetValue(temp[3], temp[4], temp[5]),
                LongTick = SetValue(temp[6], temp[7], temp[8]),
                LongDay = SetValue(temp[9], temp[10], temp[11]),
                Reaction = SetValue(temp[12], temp[13], temp[14]),
                Hedge = SetValue(temp[15], temp[16], temp[17]),
                Base = SetValue(temp[18], temp[19], temp[20]),
                Sigma = SetValue(temp[21], temp[22], temp[23]),
                Percent = SetValue(temp[24], temp[25], temp[26]),
                Max = SetValue(temp[27], temp[28], temp[29]),
                Quantity = SetValue(temp[30], temp[31], temp[32]),
                Time = SetValue(temp[33], temp[34], temp[35]),
                Capital = (long)numericCapital.Value
            };
        }
        private int SetOptimize(IAsset asset, AutomaticallySetting auto, int repeat)
        {
            try
            {
                set = new StrategySetting
                {
                    ShortTick = auto.SetVariableAutomatic(IAsset.Variable.ShortTick, asset.ShortTickPeriod, ran.Next(1, 5)),
                    LongTick = auto.SetVariableAutomatic(IAsset.Variable.LongTick, asset.LongTickPeriod, ran.Next(1, 5)),
                    ShortDay = auto.SetVariableAutomatic(IAsset.Variable.ShortDay, asset.ShortDayPeriod, ran.Next(1, 4)),
                    LongDay = auto.SetVariableAutomatic(IAsset.Variable.LongDay, asset.LongDayPeriod, ran.Next(1, 4)),
                    Reaction = auto.SetVariableAutomatic(IAsset.Variable.Reaction, asset.Reaction, ran.Next(1, 5)),
                    Hedge = auto.SetVariableAutomatic(IAsset.Variable.Hedge, asset.Hedge, ran.Next(0, 2)),
                    Base = auto.SetVariableAutomatic(IAsset.Variable.Base, asset.Base, ran.Next(0, 2)),
                    Sigma = auto.SetVariableAutomatic(IAsset.Variable.Sigma, asset.Sigma, ran.Next(0, 2)),
                    Percent = auto.SetVariableAutomatic(IAsset.Variable.Percent, asset.Percent, ran.Next(0, 2)),
                    Max = auto.SetVariableAutomatic(IAsset.Variable.Max, asset.Max, ran.Next(0, 2)),
                    Quantity = auto.SetVariableAutomatic(IAsset.Variable.Quantity, asset.Quantity, ran.Next(0, 2)),
                    Time = auto.SetVariableAutomatic(IAsset.Variable.Time, asset.Time, ran.Next(0, 3)),
                    Capital = asset.Assets
                };
                Estimate = set.EstimatedTime(new List<string>(64), CalculateTheRemainingTime() * count);

                if (repeat % 2500 == 0 && TimerBox.Show("Run the Program Again and Set it Manually.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, 5157).Equals(DialogResult.OK))
                {
                    Application.ExitThread();
                    Application.Exit();
                }
                else if (repeat % 500 == 0 && TimerBox.Show("Do You Want to Use the Existing Settings?", "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 7519).Equals(DialogResult.OK))
                {
                    checkBox.Checked = true;
                    button.ForeColor = Color.Gold;
                    button.PerformClick();

                    return int.MinValue;
                }
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Exception", ex.ToString());

                if (TimerBox.Show("Run the Program Again and Set it Manually.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, 5157).Equals(DialogResult.OK))
                {
                    Application.ExitThread();
                    Application.Exit();
                }
            }
            return Estimate.Count;
        }
        private void StartBackTesting(IStrategySetting set)
        {
            button.Text = string.Concat("Estimated Back Testing Time is ", pro.Rate(Estimate.Count, count).ToString("N0"), " Minutes.");
            button.ForeColor = Color.Maroon;
            checkBox.ForeColor = Color.DarkRed;
            checkBox.Text = "BackTesting";
            new Transmit(asset.Account, set.Capital);
            string path = Path.Combine(Application.StartupPath, @"..\Log\", DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"));
            InterLink = false;
            List<Specify> list = new List<Specify>(131072);

            foreach (string str in Estimate)
            {
                string[] temp = str.Split('^');
                list.Add(new Specify
                {
                    ShortTickPeriod = int.Parse(temp[0]),
                    ShortDayPeriod = int.Parse(temp[1]),
                    LongTickPeriod = int.Parse(temp[2]),
                    LongDayPeriod = int.Parse(temp[3]),
                    Reaction = int.Parse(temp[4]),
                    Hedge = int.Parse(temp[5]),
                    Base = int.Parse(temp[6]),
                    Sigma = int.Parse(temp[7]),
                    Percent = int.Parse(temp[8]),
                    Max = int.Parse(temp[9]),
                    Quantity = int.Parse(temp[10]),
                    Time = int.Parse(temp[11]),
                    Repository = options.Repository,
                    BasicAssets = set.Capital,
                    PathLog = path,
                    Strategy = str
                });
            }
            EstimateCount = Estimate.Count;
            Estimate = null;
            GC.Collect();
            Count = Process.GetCurrentProcess().Threads.Count;
            new Task(() =>
            {
                Remaining remaining = new Remaining();
                Parallel.ForEach(list, new ParallelOptions
                {
                    MaxDegreeOfParallelism = (int)(Environment.ProcessorCount * 1.5)
                },
                new Action<Specify>((analysis) =>
                {
                    new Analysize(remaining, analysis);
                    pro.ProgressBarValue++;
                }));
                list = null;
                button.ForeColor = Color.Yellow;
                SetMarketTick(GC.GetTotalMemory(true));
            }).Start();
        }
        private void SetMarketTick(long wait)
        {
            InterLink = true;
            pro.Maximum = SetMaximum();
            pro.Retry();

            for (long i = 0; i < wait; i++)
            {
                if (wait * 0.75 < i && Count > Process.GetCurrentProcess().Threads.Count)
                    break;

                if (pro.ProgressBarValue > pro.Maximum)
                    continue;

                pro.ProgressBarValue = (int)i / 1000;
            }
            GC.Collect();
            new BulkProcessing(string.Concat(Path.Combine(Application.StartupPath, @"..\Statistics\"), DateTime.Now.Ticks, ".csv"));

            if (TimerBox.Show(string.Concat("Do You Want to Continue with Trading??\n\nIf You don't Want to Proceed,\nPress 'No'.\n\nAfter ", pro.Maximum / 60000, " Minutes the Program is Terminated."), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, (uint)pro.Maximum).Equals((DialogResult)6))
                Process.Start("shutdown.exe", "-r");

            Application.ExitThread();
            Application.Exit();
        }
        private bool CheckNullValue(Array param)
        {
            foreach (var interval in param)
                if (interval.ToString().FindByName<NumericUpDown>(this).Value == 0 && (int)interval % 3 == 1)
                    return false;

            return true;
        }
        private void CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!button.ForeColor.Equals(Color.Maroon) && CheckNullValue(Enum.GetValues(typeof(IFindbyName.Numeric))))
            {
                if (button.ForeColor.Equals(Color.Ivory) && CheckCurrent)
                {
                    set = GetNumericValue(Enum.GetValues(typeof(IFindbyName.Numeric)));
                    checkBox.Text = "Loading. . .";
                    Application.DoEvents();
                    Estimate = set.EstimatedTime(new List<string>(32), CalculateTheRemainingTime() * count);
                    button.Text = string.Concat("Estimated Back Testing Time is ", pro.Rate(Estimate.Count, count).ToString("N0"), " Minutes.");
                    checkBox.Text = "Reset";
                    checkBox.ForeColor = Color.Yellow;
                    buttonSave.ForeColor = Color.Khaki;
                    button.ForeColor = Color.Gold;

                    return;
                }
                button.Text = string.Concat("Click to Recommend ", CalculateTheRemainingTime().ToString("N0"), " Minutes and Save Existing Data.");
                button.ForeColor = Color.Ivory;
                checkBox.ForeColor = Color.Ivory;
                checkBox.Text = "Process";
            }
            else if (CheckCurrent == false)
            {
                if (EstimateCount < 1)
                {
                    checkBox.Text = "Set It Again";
                    checkBox.ForeColor = Color.Crimson;

                    return;
                }
                checkBox.Text = string.Concat(pro.ProgressBarValue.ToString("N0"), " / ", EstimateCount.ToString("N0"));
                checkBox.Font = new Font(checkBox.Font.Name, checkBox.Font.Name.Equals("Consolas") ? 8.25F : 10.25F, FontStyle.Regular);

                if (TimerBox.Show(string.Concat("Currently\n", GC.GetTotalMemory(false).ToString("N0"), "bytes of Memory\nare in Use.\n\nDo You want to Clean Up the Accumulated Memory?"), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, 1325).Equals(DialogResult.OK))
                    GC.Collect();
            }
            else if (CheckCurrent)
            {
                checkBox.Text = string.Concat("Parallel ", Process.GetCurrentProcess().Threads.Count - Count > 0 ? (Process.GetCurrentProcess().Threads.Count - Count).ToString("N0") : "End");
                checkBox.Font = new Font(checkBox.Font.Name, checkBox.Font.Name.Equals("Consolas") ? 13.25F : 15.75F, FontStyle.Regular);
            }
        }
        private void ButtonClick(object sender, EventArgs e)
        {
            if (CheckCurrent && button.ForeColor.Equals(Color.Gold))
                StartBackTesting(set);

            else if (InterLink == false && button.ForeColor.Equals(Color.Ivory) && TimerBox.Show("Do You Want to Store Only Existing Data\nWithout Back Testing?\n\nIf Not Selected,\nIt will be Saved after 30 Seconds and the Program will Exit.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 32735).Equals((DialogResult)6))
                SetMarketTick(0);

            else if (button.ForeColor.Equals(Color.Maroon))
                button.Text = string.Concat(((EstimateCount - pro.ProgressBarValue) / count).ToString("N0"), " Minutes left to Complete.");
        }
        private int[] SetValue(int sp, int interval, int destination)
        {
            bool check = sp > destination;
            int[] value = new int[(check ? sp - destination : destination - sp) / interval + 1];

            for (int i = 0; i < value.Length; i++)
                value[i] = sp + interval * i;

            if (check)
                TimerBox.Show("The Start Value is Greater than the End Value.\n\nThe Error is Very likely to Occur in the Operation.\n\nSet It Again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, 7351);

            return value;
        }
        private void TimerTick(object sender, EventArgs e)
        {
            timer.Stop();
            checkBox.Text = "Loading. . .";
            Application.DoEvents();
            BeginInvoke(new Action(() =>
            {
                options = new Options();
            }));
            if (TimerBox.Show("Start Back Testing.\n\nClick 'No' to Do this Manually.\n\nIf Not Selected,\nIt will Automatically Proceed after 20 Seconds.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 25617).Equals((DialogResult)7))
            {
                checkBox.Text = "Manual";

                return;
            }
            int setting, repeat = 0;
            ran = new Random();
            checkBox.Font = new Font(checkBox.Font.Name, 8.25F, FontStyle.Regular);
            AutomaticallySetting auto = new AutomaticallySetting();

            do
            {
                setting = SetOptimize(asset, auto, ++repeat);
                checkBox.Text = string.Concat("No.", repeat.ToString("N0"), " Co.", (setting / count).ToString("N0"));
                Application.DoEvents();
            }
            while (CalculateTheRemainingTime(setting));

            checkBox.Font = new Font(checkBox.Font.Name, checkBox.Font.Name.Equals("Consolas") ? 13.25F : 15.75F, FontStyle.Regular);
            checkBox.Text = "BackTesting";
            GC.Collect();

            if (setting > int.MinValue)
                StartBackTesting(set);

            timer.Dispose();
        }
        private bool CalculateTheRemainingTime(int setting)
        {
            if (setting == int.MinValue)
                return false;

            int i = 1;

            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    i = 3;
                    break;

                case DayOfWeek.Saturday:
                    i = 2;
                    break;
            };
            return setting / count > (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 8, 45, 59) - DateTime.Now).TotalMinutes || setting / count < (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 7, 31, 59) - DateTime.Now).TotalMinutes;
        }
        private int CalculateTheRemainingTime()
        {
            int i = 1;

            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    i = 3;
                    break;

                case DayOfWeek.Saturday:
                    i = 2;
                    break;
            };
            return (int)(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 8, 45, 59) - DateTime.Now).TotalMinutes;
        }
        private void ButtonSaveClick(object sender, EventArgs e)
        {
            if (buttonSave.ForeColor.Equals(Color.Maroon))
                return;

            buttonSave.ForeColor = Color.Maroon;
            StringBuilder sb = new StringBuilder();

            foreach (string name in Enum.GetNames(typeof(IFindbyName.Numeric)))
                sb.Append(name.FindByName<NumericUpDown>(this).Value).Append(',');

            new SaveSetting().SetSettingValue(sb);
        }
        private int SetMaximum()
        {
            string[] temp;
            int date = 0;

            try
            {
                foreach (string val in Directory.GetDirectories(Path.Combine(Application.StartupPath, @"..\Log\")))
                {
                    temp = val.Split('\\');
                    int recent = int.Parse(temp[temp.Length - 1]);

                    if (recent > date)
                        date = recent;
                }
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Exception", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return Directory.GetFiles(string.Concat(Path.Combine(Application.StartupPath, @"..\Log\"), date), "*.csv", SearchOption.TopDirectoryOnly).Length;
        }
        private bool CheckCurrent
        {
            get
            {
                return checkBox.Checked;
            }
        }
        private bool InterLink
        {
            get; set;
        }
        private int Count
        {
            get; set;
        }
        private int EstimateCount
        {
            get; set;
        }
        private List<string> Estimate
        {
            get; set;
        }
        private Random ran;
        private Progress pro;
        private IStrategySetting set;
        private IOptions options;
        private readonly IAsset asset;
        private readonly int count;
    }
}