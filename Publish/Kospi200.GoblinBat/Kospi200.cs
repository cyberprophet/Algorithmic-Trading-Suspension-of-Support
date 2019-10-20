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

namespace ShareInvest.Kospi200
{
    public partial class Kospi200 : Form
    {
        public Kospi200()
        {
            InitializeComponent();
            Trading(ChooseResult(Choose.Show("Please Select the Button You Want to Proceed. . .", "ShareInvest GoblinBat Kospi200 TradingSystem", "Trading", "BackTest", "Exit")));
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
            using (ConnectKHOpenAPI api = new ConnectKHOpenAPI(new SpecifyKospi200
            {
                Division = false,
                Reaction = int.Parse(st[0]),
                ShortMinPeriod = int.Parse(st[1]),
                ShortDayPeriod = int.Parse(st[2]),
                LongMinPeriod = int.Parse(st[3]),
                LongDayPeriod = int.Parse(st[4])
            }))
            {
                panel.Controls.Add(api);
                panel.Controls.Add(ConfirmOrder.Get());
                Location = new Point(15, 15);
                StartPosition = FormStartPosition.Manual;
                Size = api.Size;
                api.Dock = DockStyle.Fill;
                api.Hide();
                ShowDialog();
            }
            Dispose();
            Environment.Exit(0);
        }
        private void BackTesting()
        {
            int i, j, h, f, g, reaction = 100;

            for (i = 10; i < reaction; i++)
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
                                SendRate?.Invoke(this, new ProgressRate(reaction * smp.Length * sdp.Length * lmp.Length * ldp.Length));
                            }
            new Storage();
            Box.Show("The Analysis was Successful. . .♬", "Notice", 3750);
        }
        private void OnReceiveDialogClose(object sender, ForceQuit e)
        {
            Close();
        }
        private readonly int[] smp = { 3, 5, 7 };
        private readonly int[] lmp = { 20, 35, 60 };
        private readonly int[] sdp = { 3, 5, 7 };
        private readonly int[] ldp = { 20, 35, 60 };
        public event EventHandler<ProgressRate> SendRate;
    }
}