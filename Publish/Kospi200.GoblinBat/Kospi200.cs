using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Analysize;
using ShareInvest.AutoMessageBox;
using ShareInvest.BackTest;
using ShareInvest.Const;
using ShareInvest.Control;
using ShareInvest.EventHandler;
using ShareInvest.Publish;
using ShareInvest.SelectableMessageBox;

namespace ShareInvest.Kospi200
{
    public partial class Kospi200 : Form
    {
        public Kospi200()
        {
            InitializeComponent();
            GetTermsAndConditions();
            Trading(ChooseResult(Choose.Show("Please Select the Button You Want to Proceed. . .", "ShareInvest GoblinBat Kospi200 TradingSystem", "Trading", "BackTest", "Exit")));
        }
        private void GetTermsAndConditions()
        {
            using (TermsConditions tc = new TermsConditions())
            {
                panel.Controls.Add(tc);
                tc.Dock = DockStyle.Fill;
                Size = tc.Size;
                StartPosition = FormStartPosition.CenterScreen;
                tc.SendQuit += OnReceiveDialogClose;
                ShowDialog();
            }
        }
        private string[] ChooseResult(DialogResult result)
        {
            if (result.Equals(DialogResult.Yes))
            {
                using (ChooseAnalysis ca = new ChooseAnalysis())
                {
                    Size = new Size(5, 5);
                    panel.Controls.Add(ca);
                    ca.Dock = DockStyle.Fill;
                    StartPosition = FormStartPosition.CenterScreen;
                    ca.SendQuit += OnReceiveDialogClose;
                    ShowDialog();

                    return ca.TempText.Split('.');
                }
            }
            else if (result.Equals(DialogResult.No))
            {
                using (Progress pro = new Progress())
                {
                    Size = new Size(5, 5);
                    StartPosition = FormStartPosition.Manual;
                    Location = new Point(3, 1010);
                    panel.Controls.Add(pro);
                    pro.Dock = DockStyle.Fill;
                    SendRate += pro.Rate;
                    new Task(() => BackTesting(pro)).Start();
                    SendRate?.Invoke(this, new ProgressRate(Reaction * smp.Length * sdp.Length * lmp.Length * ldp.Length));
                    ShowDialog();
                }
            }
            Dispose();
            Environment.Exit(0);

            return null;
        }
        private void Trading(string[] st)
        {
            using (ConnectKHOpenAPI api = new ConnectKHOpenAPI(new FreeVersion(), new SpecifyKospi200
            {
                Division = false,
                Reaction = int.Parse(st[0]),
                ShortMinPeriod = int.Parse(st[1]),
                ShortDayPeriod = int.Parse(st[2]),
                LongMinPeriod = int.Parse(st[3]),
                LongDayPeriod = int.Parse(st[4]),
                Strategy = string.Concat(st[0], ".", st[1], ".", st[2], ".", st[3], ".", st[4])
            }))
            {
                ConfirmOrder cf = ConfirmOrder.Get();
                panel.Controls.Add(api);
                panel.Controls.Add(cf);
                Location = new Point(15, 15);
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
        private void BackTesting(Progress pro)
        {
            int i, j, h, f, g;

            for (i = 10; i < Reaction; i++)
                for (j = 0; j < smp.Length; j++)
                    for (h = 0; h < sdp.Length; h++)
                        for (g = 0; g < lmp.Length; g++)
                            for (f = 0; f < ldp.Length; f++)
                            {
                                new Strategy(new SpecifyKospi200
                                {
                                    Division = true,
                                    Reaction = i,
                                    ShortMinPeriod = smp[j],
                                    ShortDayPeriod = sdp[h],
                                    LongMinPeriod = lmp[g],
                                    LongDayPeriod = ldp[f],
                                    Strategy = string.Concat(i.ToString("D2"), '^', smp[j].ToString("D2"), '^', sdp[h].ToString("D2"), '^', lmp[g].ToString("D2"), '^', ldp[f].ToString("D2"))
                                });
                                pro.ProgressBarValue += 1;
                            }
            new Storage();
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
                Environment.Exit(0);
            }
        }
        private int Reaction
        {
            get; set;
        } = 100;
        private readonly int[] smp = { 2, 3, 5, 7 };
        private readonly int[] lmp = { 10, 20, 35, 60 };
        private readonly int[] sdp = { 2, 3, 5, 7 };
        private readonly int[] ldp = { 10, 20, 35, 60 };
        public event EventHandler<ProgressRate> SendRate;
    }
}