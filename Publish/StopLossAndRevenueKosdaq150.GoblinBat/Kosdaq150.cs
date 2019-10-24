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

namespace ShareInvest.Kosdaq150.StopLossAndRevenue
{
    public partial class Kosdaq150 : Form
    {
        public Kosdaq150()
        {
            InitializeComponent();
            GetTermsAndConditions();
            Trading(ChooseResult(Choose.Show("Please Select the Button You Want to Proceed. . .", "ShareInvest GoblinBat Kospi200 TradingSystem", "Trading", "BackTest", "Exit")));
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
                Size = new Size(5, 5);
                StartPosition = FormStartPosition.Manual;
                Location = new Point(3, 960);
                panel.Controls.Add(pro);
                pro.Dock = DockStyle.Fill;
                SendRate += pro.Rate;
                new Task(() => BackTesting(pro, string.Concat(Environment.CurrentDirectory, @"\Statistics\", DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"), ".csv"), string.Concat(Environment.CurrentDirectory, @"\Log\", DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"), @"\"))).Start();
                SendRate?.Invoke(this, new ProgressRate(Enum.GetValues(typeof(IStopLossAndRevenue.StopLossAndRevenue)).Length * revenue.Length * stoploss.Length * Reaction * smp.Length * sdp.Length * lmp.Length * ldp.Length));
                ShowDialog();
            }
            Dispose();
            Environment.Exit(0);

            return null;
        }
        private void Trading(string[] st)
        {
            using (ConnectKHOpenAPI api = new ConnectKHOpenAPI(new VerifyIdentity(), new SpecifyKosdaq150
            {
                BasicAssets = 5000000,
                StopLoss = int.Parse(st[0]),
                Revenue = int.Parse(st[1]),
                Stop = (IStopLossAndRevenue.StopLossAndRevenue)int.Parse(st[2]),
                Division = false,
                Reaction = int.Parse(st[3]),
                ShortMinPeriod = int.Parse(st[4]),
                ShortDayPeriod = int.Parse(st[5]),
                LongMinPeriod = int.Parse(st[6]),
                LongDayPeriod = int.Parse(st[7]),
                Strategy = string.Concat(st[0], ".", st[1], ".", st[2], ".", st[3], ".", st[4], ".", st[5], ".", st[6], ".", st[7])
            }))
            {
                ConfirmOrder cf = ConfirmOrder.Get();
                panel.Controls.Add(api);
                panel.Controls.Add(cf);
                Location = new Point(2, 950);
                StartPosition = FormStartPosition.Manual;
                Size = cf.Size;
                cf.Dock = DockStyle.Fill;
                api.Dock = DockStyle.Fill;
                api.Hide();
                api.SendQuit += OnReceiveDialogClose;
                ShowDialog();
            }
            Dispose();
            Environment.Exit(0);
        }
        private void BackTesting(Progress pro, string strategy, string log)
        {
            int i, j, h, f, g, s, r;

            for (i = 2; i < Reaction; i++)
                for (j = 0; j < smp.Length; j++)
                    for (h = 0; h < sdp.Length; h++)
                        for (g = 0; g < lmp.Length; g++)
                            for (f = 0; f < ldp.Length; f++)
                                for (s = 0; s < stoploss.Length; s++)
                                    for (r = 0; r < revenue.Length; r++)
                                        foreach (IStopLossAndRevenue.StopLossAndRevenue val in Enum.GetValues(typeof(IStopLossAndRevenue.StopLossAndRevenue)))
                                        {
                                            new Strategy(new SpecifyKosdaq150
                                            {                                                
                                                PathLog = log,
                                                BasicAssets = 5000000,
                                                StopLoss = stoploss[s],
                                                Revenue = revenue[r],
                                                Stop = val,
                                                Division = true,
                                                Reaction = i,
                                                ShortMinPeriod = smp[j],
                                                ShortDayPeriod = sdp[h],
                                                LongMinPeriod = lmp[g],
                                                LongDayPeriod = ldp[f],
                                                Strategy = string.Concat(stoploss[s].ToString("D2"), '^', revenue[r].ToString("D2"), '^', ((int)val).ToString("D2"), '^', i.ToString("D2"), '^', smp[j].ToString("D2"), '^', sdp[h].ToString("D2"), '^', lmp[g].ToString("D2"), '^', ldp[f].ToString("D2"))
                                            });
                                            pro.ProgressBarValue += 1;
                                        }
            new Storage(strategy);
            Box.Show("The Analysis was Successful. . .♬", "Notice", 3750);
            Environment.Exit(0);
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
        private int Reaction
        {
            get;
        } = 30;
        private readonly int[] stoploss = { 10, 15, 20, 25 };
        private readonly int[] revenue = { 10, 15, 20, 25, 30, 35 };
        private readonly int[] smp = { 2, 3, 7 };
        private readonly int[] lmp = { 10, 20 };
        private readonly int[] sdp = { 2, 3, 7 };
        private readonly int[] ldp = { 10, 20 };
        public event EventHandler<ProgressRate> SendRate;
    }
}