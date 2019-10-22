using System;
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

namespace ShareInvest.Kosdaq150
{
    public partial class Kosdaq150 : Form
    {
        public Kosdaq150()
        {
            InitializeComponent();
            Trading(ChooseResult(Choose.Show("Please Select the Button You Want to Proceed. . .", "ShareInvest GoblinBat Kosdaq150 TradingSystem", "Trading", "BackTest", "Exit")));
        }
        private string[] ChooseResult(DialogResult result)
        {
            if (result.Equals(DialogResult.Yes))
            {
                using (ChooseAnalysis ca = new ChooseAnalysis())
                {
                    tableLayoutPanel.SuspendLayout();
                    panel.SuspendLayout();
                    tableLayoutPanel.RowStyles.Clear();
                    tableLayoutPanel.Controls.Add(webBrowser, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 37)));
                    tableLayoutPanel.Controls.Add(panel, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 63)));
                    panel.Controls.Add(ca);
                    ca.Dock = DockStyle.Fill;
                    panel.ResumeLayout();
                    tableLayoutPanel.ResumeLayout();
                    ca.SendQuit += OnReceiveDialogClose;
                    ShowDialog();

                    return ca.TempText.Split('.');
                }
            }
            else if (result.Equals(DialogResult.No))
            {
                using (Progress pro = new Progress())
                {
                    tableLayoutPanel.SuspendLayout();
                    panel.SuspendLayout();
                    tableLayoutPanel.RowStyles.Clear();
                    tableLayoutPanel.Controls.Add(webBrowser, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70)));
                    tableLayoutPanel.Controls.Add(panel, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)));
                    panel.Controls.Add(pro);
                    pro.Dock = DockStyle.Fill;
                    panel.ResumeLayout();
                    tableLayoutPanel.ResumeLayout();
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
            using (ConnectKHOpenAPI api = new ConnectKHOpenAPI(new FreeVersion(), new SpecifyKosdaq150
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
                tableLayoutPanel.SuspendLayout();
                panel.SuspendLayout();
                tableLayoutPanel.RowStyles.Clear();
                tableLayoutPanel.Controls.Add(webBrowser, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70)));
                tableLayoutPanel.Controls.Add(panel, 0, tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)));
                panel.Controls.Add(api);
                panel.Controls.Add(cf);
                cf.Dock = DockStyle.Fill;
                api.Dock = DockStyle.Fill;
                api.Hide();
                panel.ResumeLayout();
                tableLayoutPanel.ResumeLayout();
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
        } = 50;
        private readonly int[] smp = { 5 };
        private readonly int[] lmp = { 60 };
        private readonly int[] sdp = { 5 };
        private readonly int[] ldp = { 60 };
        public event EventHandler<ProgressRate> SendRate;
    }
}