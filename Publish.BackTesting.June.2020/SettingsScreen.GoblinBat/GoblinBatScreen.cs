using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.BackTesting.Analysis;
using ShareInvest.Communication;
using ShareInvest.GoblinBatContext;
using ShareInvest.RemainingDate;

namespace ShareInvest.BackTesting.SettingsScreen
{
    public partial class GoblinBatScreen : UserControl
    {
        public GoblinBatScreen()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(121, 133, 130);
            using var db = new GoblinBatDbContext();
            var instance = SqlProviderServices.Instance;

            foreach (var code in db.Codes.ToList())
                comboBox.Items.Add(code.Name.Replace(" ", string.Empty));
                        
            comboBox.SelectedItem = db.Codes.Where(o => o.Code.Substring(0, 3).Equals("101") && o.Code.Substring(5, 3).Equals("000")).Max(o => o.Name).Replace(" ", string.Empty);
            comboBox.DropDownHeight = 156;
        }
        public void SetProgress(Progress pro)
        {
            this.pro = pro;
            BeginInvoke(new Action(() =>
            {
                SuspendLayout();
                ResumeLayout();
            }));
            Application.DoEvents();
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
        private void StartBackTesting(IStrategySetting set)
        {
            button.ForeColor = Color.Maroon;
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
                    BasicAssets = set.Capital,
                    PathLog = path,
                    Strategy = str
                });
            }
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

            for (long i = 0; i < wait; i++)
            {
                if (wait * 0.75 < i && Count > Process.GetCurrentProcess().Threads.Count)
                    break;

                if (pro.ProgressBarValue > pro.Maximum)
                    continue;

                pro.ProgressBarValue = (int)i / 1000;
            }
        }
        private void ButtonClick(object sender, EventArgs e)
        {
            if (button.ForeColor.Equals(Color.Gold))
            {
                //StartBackTesting();
            }

            else if (InterLink == false && button.ForeColor.Equals(Color.Ivory) && TimerBox.Show("Do You Want to Store Only Existing Data\nWithout Back Testing?\n\nIf Not Selected,\nIt will be Saved after 30 Seconds and the Program will Exit.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 32735).Equals((DialogResult)6))
                SetMarketTick(0);
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
        private void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox cb)
            {
                using var db = new GoblinBatDbContext();
                cb.Name = db.Codes.First(o => o.Name.Replace(" ", string.Empty).Equals(cb.Text)).Code;
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
        private List<string> Estimate
        {
            get; set;
        }
        private Progress pro;
    }
}