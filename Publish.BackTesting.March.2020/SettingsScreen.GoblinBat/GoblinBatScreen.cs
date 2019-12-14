using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        }
        public void SetProgress(Progress pro)
        {
            this.pro = pro;
            SuspendLayout();
            IAsyncResult result = BeginInvoke(new Action(() =>
            {
                ran = new Random();
                SetLabelUsed();
                SetNumeric(new RecallSettings().GetSettingValue());
            }));
            timer.Start();

            do
            {
                Application.DoEvents();
            }
            while (result.IsCompleted == false);

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
            int index = (int)Enum.Parse(typeof(IFindbyName.CheckBoxUsed), cb.Name);
            NumericUpDown down = Enum.GetName(typeof(IFindbyName.Numeric), 3 * index).FindByName<NumericUpDown>(this);
            NumericUpDown up = Enum.GetName(typeof(IFindbyName.Numeric), 3 * index + 2).FindByName<NumericUpDown>(this);
            down.Minimum = 0;
            up.Minimum = 0;

            if (cb.CheckState.Equals(CheckState.Checked))
            {
                cb.ForeColor = Color.Gold;
                down.Value = ran.Next(1, 3);
                up.Value = ran.Next(3, 6);

                return;
            }
            down.Value = 0;
            up.Value = 0;
            cb.ForeColor = Color.Maroon;
        }
        private void SetNumeric(string[] param)
        {
            foreach (var name in Enum.GetValues(typeof(IFindbyName.Numeric)))
            {
                NumericUpDown temp = name.ToString().FindByName<NumericUpDown>(this);
                temp.Minimum = 0;
                temp.Maximum = uint.MaxValue;
                temp.Value = uint.Parse(param[(int)name]);
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
        private int SetOptimize(IAsset asset, int repeat)
        {
            try
            {
                set = new StrategySetting
                {
                    ShortTick = SetValue(ran.Next(asset.ShortTickPeriod - 20, asset.ShortTickPeriod), ran.Next(5, 21), ran.Next(asset.ShortTickPeriod, asset.ShortTickPeriod + 20)),
                    LongTick = SetValue(ran.Next(asset.LongTickPeriod - 50, asset.LongTickPeriod), ran.Next(15, 51), ran.Next(asset.LongTickPeriod, asset.LongTickPeriod + 50)),
                    ShortDay = SetValue(2, ran.Next(1, 4), ran.Next(asset.ShortDayPeriod, asset.ShortDayPeriod + 6)),
                    LongDay = SetValue(ran.Next(asset.LongDayPeriod - 5, asset.LongDayPeriod), ran.Next(5, 11), ran.Next(asset.LongDayPeriod, asset.LongDayPeriod + 5)),
                    Reaction = SetValue(ran.Next(asset.Reaction - 15, asset.Reaction), ran.Next(1, 6), ran.Next(asset.Reaction, asset.Reaction + 15)),
                    Hedge = SetValue(0, ran.Next(1, 4), ran.Next(0, 6)),
                    Base = SetValue(ran.Next(asset.Base - 50, asset.Base), ran.Next(15, 51), ran.Next(asset.Base, asset.Base + 50)),
                    Sigma = SetValue(ran.Next(asset.Sigma - 5, asset.Sigma), ran.Next(1, 4), ran.Next(asset.Sigma, asset.Sigma + 6)),
                    Percent = SetValue(ran.Next(asset.Percent - 10, asset.Percent), ran.Next(1, 6), ran.Next(asset.Percent, asset.Percent + 11)),
                    Max = SetValue(ran.Next(asset.Max - 10, asset.Max), ran.Next(5, 11), ran.Next(asset.Max, 101)),
                    Quantity = SetValue(ran.Next(1), ran.Next(1, 6), ran.Next(1, 11)),
                    Time = SetValue(ran.Next(1), ran.Next(1, 6), ran.Next(1, 11)),
                    Capital = asset.Assets
                };
                return set.EstimatedTime();
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
            finally
            {
                if (repeat % 5000 == 0 && TimerBox.Show("Run the Program Again and Set it Manually.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, 5157).Equals(DialogResult.OK))
                {
                    Application.ExitThread();
                    Application.Exit();
                }
                if (repeat % 1000 == 0)
                    ran = new Random();
            }
            return 0;
        }
        private void StartBackTesting(IStrategySetting set)
        {
            Max = set.EstimatedTime();
            button.Text = string.Concat("Estimated Back Testing Time is ", pro.Rate(Max, count).ToString("N0"), " Minutes.");
            button.ForeColor = Color.Maroon;
            checkBox.ForeColor = Color.DarkRed;
            checkBox.Text = "BackTesting";
            new Transmit(asset.Account, set.Capital);
            string path = string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\Log\", DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"), @"\");
            InterLink = false;
            List<Specify> list = new List<Specify>(131072);

            foreach (int time in set.Time)
                foreach (int quantity in set.Quantity)
                    foreach (int max in set.Max)
                        foreach (int percent in set.Percent)
                            foreach (int sigma in set.Sigma)
                                foreach (int baseBand in set.Base)
                                    foreach (int hedge in set.Hedge)
                                        foreach (int reaction in set.Reaction)
                                            foreach (int sTick in set.ShortTick)
                                                foreach (int sDay in set.ShortDay)
                                                    foreach (int lTick in set.LongTick)
                                                        foreach (int lDay in set.LongDay)
                                                            if (sTick < lTick && sDay < lDay)
                                                                list.Add(new Specify
                                                                {
                                                                    Time = time,
                                                                    Quantity = quantity,
                                                                    Max = max,
                                                                    Percent = percent,
                                                                    Sigma = sigma,
                                                                    Base = baseBand,
                                                                    Repository = options.Repository,
                                                                    ShortTickPeriod = sTick,
                                                                    ShortDayPeriod = sDay,
                                                                    LongTickPeriod = lTick,
                                                                    LongDayPeriod = lDay,
                                                                    Reaction = reaction,
                                                                    Hedge = hedge,
                                                                    BasicAssets = set.Capital,
                                                                    PathLog = path,
                                                                    Strategy = string.Concat(sTick.ToString(), '^', sDay.ToString(), '^', lTick.ToString(), '^', lDay.ToString(), '^', reaction.ToString(), '^', hedge.ToString(), '^', baseBand.ToString(), '^', sigma.ToString(), '^', percent.ToString(), '^', max.ToString(), '^', quantity.ToString(), '^', time.ToString())
                                                                });
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
                list.Clear();
                button.ForeColor = Color.Ivory;
                SetMarketTick(GC.GetTotalMemory(true));
            }).Start();
        }
        private void SetMarketTick(long wait)
        {
            InterLink = true;
            GC.Collect();

            for (long i = 0; i < wait; i++)
                if (Count > Process.GetCurrentProcess().Threads.Count)
                    break;

            pro.Maximum = SetMaximum();
            pro.Retry();
            GC.Collect();
            new BulkProcessing(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\Statistics\", DateTime.Now.Ticks, ".csv"));
            GC.Collect();

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
                    IAsyncResult result = BeginInvoke(new Action(() =>
                    {
                        set = GetNumericValue(Enum.GetValues(typeof(IFindbyName.Numeric)));
                        checkBox.Text = "Loading. . .";
                        Application.DoEvents();
                        button.Text = string.Concat("Estimated Back Testing Time is ", pro.Rate(set.EstimatedTime(), count).ToString("N0"), " Minutes.");
                        checkBox.Text = "Reset";
                        checkBox.ForeColor = Color.Yellow;
                        buttonSave.ForeColor = Color.Khaki;
                        button.ForeColor = Color.Gold;
                    }));
                    do
                    {
                        Application.DoEvents();
                    }
                    while (result.IsCompleted == false);

                    return;
                }
                button.Text = string.Concat("Click to Recommend ", DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday) ? 2880 + 870 : 870, " Minutes and Save Existing Data.");
                button.ForeColor = Color.Ivory;
                checkBox.ForeColor = Color.Ivory;
                checkBox.Text = "Process";
            }
            else if (CheckCurrent == false)
            {
                if (Max == 0)
                {
                    checkBox.Text = "Set It Again";
                    checkBox.ForeColor = Color.Crimson;

                    return;
                }
                checkBox.Text = string.Concat(pro.ProgressBarValue.ToString("N0"), " / ", Max.ToString("N0"));
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
                button.Text = string.Concat(((Max - pro.ProgressBarValue) / count).ToString("N0"), " Minutes left to Complete.");
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

            do
            {
                setting = SetOptimize(asset, ++repeat);
                checkBox.Text = string.Concat("No.", repeat.ToString("N0"), " Co.", (setting / count).ToString("N0"));
                Application.DoEvents();
            }
            while (setting < count * (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday) ? 2880 + 915 : 915) || setting > count * (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday) ? 2880 + 965 : 965));

            checkBox.Font = new Font(checkBox.Font.Name, checkBox.Font.Name.Equals("Consolas") ? 13.25F : 15.75F, FontStyle.Regular);
            GC.Collect();
            StartBackTesting(set);
            timer.Dispose();
        }
        private void TimerStorageTick(object sender, EventArgs e)
        {
            if (InterLink && pro.Maximum - 20 > pro.ProgressBarValue)
                pro.ProgressBarValue += 15;

            else if (InterLink && pro.Maximum > pro.ProgressBarValue)
                pro.ProgressBarValue++;

            Application.DoEvents();
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
                foreach (string val in Directory.GetDirectories(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\Log\")))
                {
                    temp = val.Split('\\');
                    int recent = int.Parse(temp[temp.Length - 1]);

                    if (recent > date)
                        date = recent;
                }
                timerStorage.Interval = 15;
                timerStorage.Start();
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Exception", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
            return Directory.GetFiles(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\Log\", date), "*.csv", SearchOption.TopDirectoryOnly).Length;
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
        private int Max
        {
            get; set;
        }
        private int Count
        {
            get; set;
        }
        private Random ran;
        private Progress pro;
        private IStrategySetting set;
        private IOptions options;
        private readonly int count;
        private readonly IAsset asset;
    }
}