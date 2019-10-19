using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.Analysize;
using ShareInvest.AutoMessageBox;
using ShareInvest.BackTest;
using ShareInvest.Const;
using ShareInvest.Control;
using ShareInvest.EventHandler;
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
                    Size = ca.Size;
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
            new ConnectKHOpenAPI(new SpecifyKospi200
            {
                Division = true,
                Reaction = int.Parse(st[0]),
                ShortMinPeriod = int.Parse(st[1]),
                ShortDayPeriod = int.Parse(st[2]),
                LongMinPeriod = int.Parse(st[3]),
                LongDayPeriod = int.Parse(st[4])
            });
        }
        private void BackTesting()
        {
            int i, j, h, f, g, reaction = 100;

            for (i = 10; i < reaction; i++)
                for (j = 0; j < sp.Length; j++)
                    for (h = 0; h < sp.Length; h++)
                        for (g = 0; g < lp.Length; g++)
                            for (f = 0; f < lp.Length; f++)
                            {
                                if (sp[j] >= lp[g] || sp[h] >= lp[f])
                                    continue;

                                IAsyncResult result = BeginInvoke(new Action(() => new Strategy(new SpecifyKospi200
                                {
                                    Division = true,
                                    Reaction = i,
                                    ShortMinPeriod = sp[j],
                                    ShortDayPeriod = sp[h],
                                    LongMinPeriod = lp[g],
                                    LongDayPeriod = lp[f],
                                    Strategy = string.Concat(i.ToString("D2"), '^', sp[j].ToString("D2"), '^', sp[h].ToString("D2"), '^', lp[g].ToString("D2"), '^', lp[f].ToString("D2"))
                                })));
                                EndInvoke(result);
                                SendRate?.Invoke(this, new ProgressRate(result));
                            }
            new Storage();
            Box.Show("The Analysis was Successful. . .♬", "Notice", 3750);
        }
        private void OnReceiveDialogClose(object sender, ForceQuit e)
        {
            Close();
        }
        private readonly int[] sp = { 3, 5, 10, 15, 20 };
        private readonly int[] lp = { 20, 35, 60, 90, 120 };
        public event EventHandler<ProgressRate> SendRate;
    }
}