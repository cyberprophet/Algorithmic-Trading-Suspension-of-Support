using System;
using System.Drawing;
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
                    panel.Controls.Add(ca);
                    ca.Dock = DockStyle.Fill;
                    Size = ca.Size;
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
                    panel.Controls.Add(pro);
                    Size = pro.Size;
                    pro.Dock = DockStyle.Fill;
                    Location = new Point(3, 1010);
                    Show();
                    SendRate += pro.Rate;
                    BackTesting();
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
                panel.Controls.Add(api);
                panel.Controls.Add(ConfirmOrder.Get());
                Location = new Point(15, 15);
                StartPosition = FormStartPosition.Manual;
                Size = api.Size;
                api.Dock = DockStyle.Fill;
                api.Hide();
                api.SendQuit += OnReceiveDialogClose;
                ShowDialog();
            }
            Dispose();
            Environment.Exit(0);
        }
        private void BackTesting()
        {
            int i, j, h, f, g, reaction = 50;

            for (i = 1; i < reaction; i++)
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
                                SendRate?.Invoke(this, new ProgressRate(reaction * smp.Length * sdp.Length * lmp.Length * ldp.Length));
                            }
            new Storage();
            Box.Show("The Analysis was Successful. . .♬", "Notice", 3750);
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
        private void Kosdaq150_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
            Environment.Exit(0);
        }
        private readonly int[] smp = { 5 };
        private readonly int[] lmp = { 60 };
        private readonly int[] sdp = { 5 };
        private readonly int[] ldp = { 60 };
        public event EventHandler<ProgressRate> SendRate;
    }
}