using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.BackTesting.Analysis;
using ShareInvest.Communication;
using ShareInvest.Market;

namespace ShareInvest.BackTesting.SettingsScreen
{
    public partial class GoblinBatScreen : UserControl
    {
        public GoblinBatScreen()
        {
            InitializeComponent();
        }
        public void SetProgress(Progress pro)
        {
            this.pro = pro;
            timer.Start();
        }
        private void StartBackTesting(IStrategySetting set)
        {
            Max = set.EstimatedTime();
            button.Text = string.Concat("Estimated Back Testing Time is ", pro.Rate(Max).ToString("N0"), " Minutes.");
            button.ForeColor = Color.Maroon;
            checkBox.ForeColor = Color.DarkRed;
            checkBox.Text = "BackTesting";
            string path = string.Concat(Path.Combine(Environment.CurrentDirectory, @"..\"), @"\Log\", DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"), @"\");

            foreach (int hedge in Enum.GetValues(typeof(IStrategySetting.Hedge)))
                foreach (int reaction in set.Reaction)
                    foreach (int sTick in set.ShortTick)
                        foreach (int sDay in set.ShortDay)
                            foreach (int lTick in set.LongTick)
                                foreach (int lDay in set.LongDay)
                                {
                                    if (sTick >= lTick || sDay >= lDay)
                                    {
                                        Application.DoEvents();

                                        continue;
                                    }
                                    new Task(() =>
                                    {
                                        new Analysize(new Specify
                                        {
                                            ShortTickPeriod = sTick,
                                            ShortDayPeriod = sDay,
                                            LongTickPeriod = lTick,
                                            LongDayPeriod = lDay,
                                            Reaction = reaction,
                                            Hedge = hedge,
                                            BasicAssets = set.Capital,
                                            PathLog = path,
                                            Strategy = string.Concat(sDay.ToString("D2"), '^', sTick.ToString("D2"), '^', lDay.ToString("D2"), '^', lTick.ToString("D2"), '^', reaction.ToString("D2"), '^', hedge.ToString("D2"))
                                        });
                                        pro.ProgressBarValue++;

                                        if (Max.Equals(pro.ProgressBarValue))
                                        {
                                            new Storage(string.Concat(Path.Combine(Environment.CurrentDirectory, @"..\"), @"\Statistics\", DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"), ".csv"));
                                            SetMarketTick();
                                        }
                                    }).Start();
                                    Application.DoEvents();
                                }
        }
        private void SetMarketTick()
        {
            if (TimerBox.Show("Do You Want to Continue with Trading??\n\nIf You don't Want to Proceed,\nPress 'No'.\n\nAfter 30 Seconds the Program is Terminated.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 32735).Equals((DialogResult)6))
                Process.Start("Kospi200.exe");

            SendMarket?.Invoke(this, new OpenMarket(0));
        }
        private void CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!button.ForeColor.Equals(Color.Maroon))
            {
                if (button.ForeColor.Equals(Color.Ivory) && CheckCurrent)
                {
                    set = new StrategySetting
                    {
                        ShortTick = SetValue((int)numericPST.Value, (int)numericIST.Value, (int)numericDST.Value),
                        ShortDay = SetValue((int)numericPSD.Value, (int)numericISD.Value, (int)numericDSD.Value),
                        LongTick = SetValue((int)numericPLT.Value, (int)numericILT.Value, (int)numericDLT.Value),
                        LongDay = SetValue((int)numericPLD.Value, (int)numericILD.Value, (int)numericDLD.Value),
                        Reaction = SetValue((int)numericPR.Value, (int)numericIR.Value, (int)numericDR.Value),
                        Capital = (long)numericCapital.Value
                    };
                    button.Text = string.Concat("Estimated Back Testing Time is ", pro.Rate(set.EstimatedTime()).ToString("N0"), " Minutes.");
                    checkBox.Text = "Reset";
                    checkBox.ForeColor = Color.Yellow;
                    button.ForeColor = Color.Gold;

                    return;
                }
                button.Text = "The Recommended Time is 900 Minutes.";
                button.ForeColor = Color.Ivory;
                checkBox.ForeColor = Color.Ivory;
                checkBox.Text = "Guide";
            }
        }
        private void ButtonClick(object sender, EventArgs e)
        {
            if (CheckCurrent && button.ForeColor.Equals(Color.Gold))
                StartBackTesting(set);

            else if (button.ForeColor.Equals(Color.Maroon))
                button.Text = string.Concat(((Max - pro.ProgressBarValue) / 210).ToString("N0"), " Minutes left to Complete.");
        }
        private int[] SetValue(int sp, int interval, int destination)
        {
            int[] value = new int[(destination - sp) / interval + 1];
            int i;

            for (i = 0; i < value.Length; i++)
                value[i] = sp + interval * i;

            return value;
        }
        private void TimerTick(object sender, EventArgs e)
        {
            timer.Stop();

            if (TimerBox.Show("Start Back Testing.\n\nClick 'No' to Do this Manually.\n\nIf Not Selected,\nIt will Automatically Proceed after 20 Seconds.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 25617).Equals((DialogResult)7))
                return;

            StartBackTesting(new StrategySetting
            {
                ShortTick = SetValue((int)numericPST.Value, (int)numericIST.Value, (int)numericDST.Value),
                ShortDay = SetValue((int)numericPSD.Value, (int)numericISD.Value, (int)numericDSD.Value),
                LongTick = SetValue((int)numericPLT.Value, (int)numericILT.Value, (int)numericDLT.Value),
                LongDay = SetValue((int)numericPLD.Value, (int)numericILD.Value, (int)numericDLD.Value),
                Reaction = SetValue((int)numericPR.Value, (int)numericIR.Value, (int)numericDR.Value),
                Capital = (long)numericCapital.Value
            });
        }
        private bool CheckCurrent
        {
            get
            {
                return checkBox.Checked;
            }
        }
        private int Max
        {
            get; set;
        }
        private Progress pro;
        private IStrategySetting set;
        public event EventHandler<OpenMarket> SendMarket;
    }
}