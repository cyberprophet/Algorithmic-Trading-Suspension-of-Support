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

namespace ShareInvest.Kosdaq150
{
    public partial class Kosdaq150 : Form
    {
        public Kosdaq150()
        {
            InitializeComponent();
            GetTermsAndConditions();
            Trading(ChooseResult(Choose.Show("Please Select the Button You Want to Proceed. . .", "ShareInvest GoblinBat Kosdaq150 TradingSystem", "Trading", "BackTest", "Exit")));
        }
        private void GetTermsAndConditions()
        {
            using TermsConditions tc = new TermsConditions();
            tableLayoutPanel.RowStyles.Clear();
            tableLayoutPanel.Controls.Add(webBrowser, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0)));
            tableLayoutPanel.Controls.Add(panel, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)));
            webBrowser.Hide();
            panel.Controls.Add(tc);
            panel.BorderStyle = BorderStyle.Fixed3D;
            Size = tc.Size;
            tc.Dock = DockStyle.Fill;
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
                tableLayoutPanel.RowStyles.Clear();
                tableLayoutPanel.Controls.Add(webBrowser, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0)));
                tableLayoutPanel.Controls.Add(panel, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)));
                panel.Controls.Add(ca);
                ca.Dock = DockStyle.Fill;
                ca.SendQuit += OnReceiveDialogClose;
                ShowDialog();

                return ca.TempText.Split('.');
            }
            else if (result.Equals(DialogResult.No))
            {
                using Progress pro = new Progress();
                tableLayoutPanel.RowStyles.Clear();
                tableLayoutPanel.Controls.Add(webBrowser, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70)));
                tableLayoutPanel.Controls.Add(panel, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 31)));
                panel.Controls.Add(pro);
                pro.Dock = DockStyle.Fill;
                panel.BorderStyle = BorderStyle.None;
                WindowState = FormWindowState.Maximized;
                webBrowser.Show();
                SendRate += pro.Rate;
                new Task(() => BackTesting(pro)).Start();
                SendRate?.Invoke(this, new ProgressRate(Reaction * smp.Length * sdp.Length * lmp.Length * ldp.Length));
                ShowDialog();
            }
            Dispose();
            Environment.Exit(0);

            return null;
        }
        private void Trading(string[] st)
        {
            using (ConnectKHOpenAPI api = new ConnectKHOpenAPI(new FreeVersion(), new SpecifyKosdaq150
            {
                Stop = IStopLossAndRevenue.StopLossAndRevenue.UnUsed,
                BasicAssets = 5000000,
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
                webBrowser.Show();
                tableLayoutPanel.RowStyles.Clear();
                tableLayoutPanel.Controls.Add(webBrowser, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70)));
                tableLayoutPanel.Controls.Add(panel, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)));
                panel.Controls.Add(api);
                panel.Controls.Add(cf);
                cf.Dock = DockStyle.Fill;
                api.Dock = DockStyle.Fill;
                api.Hide();
                panel.BorderStyle = BorderStyle.None;
                WindowState = FormWindowState.Maximized;
                api.SendQuit += OnReceiveDialogClose;
                ShowDialog();
            }
            Dispose();
            Environment.Exit(0);
        }
        private void BackTesting(Progress pro)
        {
            int i, j, h, f, g;

            for (i = 1; i < Reaction; i++)
                for (j = 0; j < smp.Length; j++)
                    for (h = 0; h < sdp.Length; h++)
                        for (g = 0; g < lmp.Length; g++)
                            for (f = 0; f < ldp.Length; f++)
                            {
                                new Strategy(new SpecifyKosdaq150
                                {
                                    Stop = IStopLossAndRevenue.StopLossAndRevenue.UnUsed,
                                    BasicAssets = 5000000,
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
                Application.Restart();
            }
        }
        private int Reaction
        {
            get; set;
        } = 50;
        private readonly int[] smp = { 2, 3, 5, 7 };
        private readonly int[] lmp = { 10, 20, 35, 60 };
        private readonly int[] sdp = { 2, 3, 5, 7 };
        private readonly int[] ldp = { 10, 20, 35, 60 };
        public event EventHandler<ProgressRate> SendRate;
    }
}