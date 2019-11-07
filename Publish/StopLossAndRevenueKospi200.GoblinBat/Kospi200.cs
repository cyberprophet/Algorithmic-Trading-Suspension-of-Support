using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Analysize;
using ShareInvest.AutoMessageBox;
using ShareInvest.BackTest;
using ShareInvest.Communicate;
using ShareInvest.Const;
using ShareInvest.Control;
using ShareInvest.EventHandler;
using ShareInvest.Publish;
using ShareInvest.SelectableMessageBox;

namespace ShareInvest.StopLossAndRevenueKospi200
{
    public partial class Kospi200 : Form
    {
        public Kospi200()
        {
            InitializeComponent();
            GetTermsAndConditions();
            Trading(ChooseResult(Choose.Show("Please Select the Button You Want to Proceed. . .", "ShareInvest GoblinBat Kospi200 TradingSystem", "Trading", "BackTest", "Exit")));
        }
        private void Trading(string[] st)
        {
            using (ConnectKHOpenAPI api = new ConnectKHOpenAPI(new FreeVersion(), new SpecifyKospi200
            {
                Division = false,
                BasicAssets = 15000000,
                Reaction = int.Parse(st[0]),
                Stop = (IStopLossAndRevenue.StopLossAndRevenue)int.Parse(st[1]),
                StopLoss = int.Parse(st[2]),
                Revenue = int.Parse(st[3]),
                ShortMinPeriod = int.Parse(st[4]),
                ShortDayPeriod = int.Parse(st[5]),
                LongMinPeriod = int.Parse(st[6]),
                LongDayPeriod = int.Parse(st[7]),
                ShortTickPeriod = 5,
                LongTickPeriod = 60,
                Strategy = string.Concat(st[0], ".", st[1], ".", st[2], ".", st[3], ".", st[4], ".", st[5], ".", st[6], ".", st[7])
            }))
            {
                ConfirmOrder cf = ConfirmOrder.Get();
                panel.Controls.Add(api);
                panel.Controls.Add(cf);
                Location = new Point(2, 1000);
                StartPosition = FormStartPosition.Manual;
                Size = cf.Size;
                Opacity = 0.65;
                cf.Dock = DockStyle.Fill;
                cf.BackColor = Color.FromArgb(203, 212, 206);
                api.Dock = DockStyle.Fill;
                api.Hide();
                api.SendQuit += OnReceiveDialogClose;
                ShowDialog();
            }
            Dispose();
            Environment.Exit(0);
        }
        private string[] ChooseResult(DialogResult result)
        {
            if (result.Equals(DialogResult.Yes))
            {
                using ChooseAnalysis ca = new ChooseAnalysis();
                Size = new Size(5, 5);
                panel.Controls.Add(ca);
                ca.Dock = DockStyle.Fill;
                StartPosition = FormStartPosition.CenterScreen;
                ca.SendQuit += OnReceiveDialogClose;
                ShowDialog();

                return ca.TempText.Split('.');
            }
            else if (result.Equals(DialogResult.No))
            {
                using Progress pro = new Progress();
                sc = new ScalarKospi200();
                new Task(() => BackTesting(pro, string.Concat(Environment.CurrentDirectory, @"\Statistics\", DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"), ".csv"), string.Concat(Environment.CurrentDirectory, @"\Log\", DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"), @"\"))).Start();
                Size = new Size(5, 5);
                StartPosition = FormStartPosition.Manual;
                Location = new Point(1, 1010);
                panel.Controls.Add(pro);
                pro.Dock = DockStyle.Fill;
                SendRate += pro.Rate;
                SendRate?.Invoke(this, new ProgressRate(Enum.GetValues(typeof(IStopLossAndRevenue.StopLossAndRevenue)).Length * sc.StopLoss.Length * sc.Revenue.Length * sc.Reaction.Length * sc.ShortMinutePeriod.Length * sc.ShortDayPeriod.Length * sc.LongMinutePeriod.Length * sc.LongDayPeriod.Length));
                ShowDialog();
            }
            Dispose();
            Environment.Exit(0);

            return null;
        }
        private void BackTesting(Progress pro, string strategy, string log)
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
            Box.Show("The Analysis was Successful. . .♬", "Notice", 3750);
            Environment.Exit(0);
        }
        private void GetTermsAndConditions()
        {
            using TermsConditions tc = new TermsConditions();
            panel.Controls.Add(tc);
            tc.Dock = DockStyle.Fill;
            Size = tc.Size;
            StartPosition = FormStartPosition.CenterScreen;
            tc.SendQuit += OnReceiveDialogClose;
            ShowDialog();
        }
        private void OnReceiveDialogClose(object sender, ForceQuit e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                Box.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", 3750);
                Application.Restart();
            }
        }
        private IScalar sc;
        public event EventHandler<ProgressRate> SendRate;
    }
}