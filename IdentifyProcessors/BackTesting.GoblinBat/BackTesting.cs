using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Analysize.x64;
using ShareInvest.Controls.x64;
using ShareInvest.EventHandler.x64;
using ShareInvest.Interface.x64;
using ShareInvest.Variable.x64;

namespace ShareInvest.BackTesting
{
    public partial class BackTesting : Form
    {
        public BackTesting()
        {
            InitializeComponent();
            StartProgress();
        }
        private void StartProgress()
        {
            using Progress pro = new Progress();
            IScalar sc = new ScalarKospi200();
            new Task(() => StartBackTesting(sc, pro, string.Concat(Path.Combine(Environment.CurrentDirectory, @"..\"), @"\Statistics\", DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"), ".csv"), string.Concat(Path.Combine(Environment.CurrentDirectory, @"..\"), @"\Log\", DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"), @"\"))).Start();
            Size = new Size(5, 5);
            StartPosition = FormStartPosition.Manual;
            Location = new Point(1, 1010);
            DoubleBufferedTableLayoutPanel db = new DoubleBufferedTableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 1,
                AutoSize = true,
                Margin = new Padding(0),
                Dock = DockStyle.Fill,
            };
            pro.Dock = DockStyle.Fill;
            db.Controls.Add(pro);
            Controls.Add(db);
            SendRate += pro.Rate;
            SendRate?.Invoke(this, new Rate(Enum.GetValues(typeof(IStopLossAndRevenue.StopLossAndRevenue)).Length * sc.StopLoss.Length * sc.Revenue.Length * sc.Reaction.Length * sc.ShortMinutePeriod.Length * sc.ShortDayPeriod.Length * sc.LongMinutePeriod.Length * sc.LongDayPeriod.Length));
            ShowDialog();
        }
        private void StartBackTesting(IScalar sc, Progress pro, string strategy, string log)
        {
            int i, j, h, f, g, s, r;

            for (s = 0; s < sc.StopLoss.Length; s++)
                for (r = 0; r < sc.Revenue.Length; r++)
                    for (i = 0; i < sc.Reaction.Length; i++)
                        for (j = 0; j < sc.ShortMinutePeriod.Length; j++)
                            for (h = 0; h < sc.ShortDayPeriod.Length; h++)
                                for (g = 0; g < sc.LongMinutePeriod.Length; g++)
                                    for (f = 0; f < sc.LongDayPeriod.Length; f++)
                                        foreach (IStopLossAndRevenue.StopLossAndRevenue val in Enum.GetValues(typeof(IStopLossAndRevenue.StopLossAndRevenue)))
                                        {
                                            new Strategy(new SpecifyKospi200
                                            {
                                                PathLog = log,
                                                BasicAssets = 15000000,
                                                StopLoss = sc.StopLoss[s],
                                                Revenue = sc.Revenue[r],
                                                Stop = val,
                                                Division = true,
                                                Reaction = sc.Reaction[i],
                                                ShortMinPeriod = sc.ShortMinutePeriod[j],
                                                ShortDayPeriod = sc.ShortDayPeriod[h],
                                                LongMinPeriod = sc.LongMinutePeriod[g],
                                                LongDayPeriod = sc.LongDayPeriod[f],
                                                ShortTickPeriod = 5,
                                                LongTickPeriod = 60,
                                                Strategy = string.Concat(sc.Reaction[i].ToString("D2"), '^', ((int)val).ToString("D2"), '^', sc.StopLoss[s].ToString("D2"), '^', sc.Revenue[r].ToString("D2"), '^', sc.ShortMinutePeriod[j].ToString("D2"), '^', sc.ShortDayPeriod[h].ToString("D2"), '^', sc.LongMinutePeriod[g].ToString("D2"), '^', sc.LongDayPeriod[f].ToString("D2"))
                                            });
                                            pro.ProgressBarValue += 1;
                                        }
            new Storage(strategy);
        }
        public event EventHandler<Rate> SendRate;
    }
}