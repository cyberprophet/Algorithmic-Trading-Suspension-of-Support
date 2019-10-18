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
            ChooseResult(Choose.Show("Please Select the Button You Want to Proceed. . .", "ShareInvest GoblinBat Kospi200 TradingSystem", "Trading", "BackTest", "Exit"));
        }
        private void ChooseResult(DialogResult result)
        {
            if (result.Equals(DialogResult.Yes))
            {
                using (ChooseAnalysis ca = new ChooseAnalysis())
                {
                    panel.Controls.Add(ca);
                    Size = ca.Size;
                    ca.Dock = DockStyle.Fill;
                    StartPosition = FormStartPosition.CenterScreen;
                    ShowDialog();

                    string[] arr = ca.TempText.Split('.');

                    using (new ConnectKHOpenAPI(new SpecifyKospi200
                    {
                        Division = true,
                        Reaction = int.Parse(arr[0]),
                        ShortMinPeriod = int.Parse(arr[1]),
                        ShortDayPeriod = int.Parse(arr[2]),
                        LongMinPeriod = int.Parse(arr[3]),
                        LongDayPeriod = int.Parse(arr[4])
                    }))
                    {

                    }
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
        }
        private void BackTesting()
        {
            int i, j, h, f, g, reaction = 100, period = 21;

            for (i = 1; i < reaction; i++)
                for (j = 5; j < period; j++)
                    for (h = 5; h < period; h++)
                        for (g = 20; g < period * 5 - 5; g++)
                            for (f = 20; f < period * 5 - 5; f++)
                            {
                                if (j >= h || g >= f)
                                    continue;

                                IAsyncResult result = BeginInvoke(new Action(() => new Strategy(new SpecifyKospi200
                                {
                                    Division = true,
                                    Reaction = i,
                                    ShortMinPeriod = j,
                                    ShortDayPeriod = h,
                                    LongMinPeriod = g,
                                    LongDayPeriod = f,
                                    Strategy = string.Concat(i.ToString("D2"), '^', j.ToString("D2"), '^', h.ToString("D2"), '^', g.ToString("D2"), '^', f.ToString("D2"))
                                })));
                                EndInvoke(result);
                                SendRate?.Invoke(this, new ProgressRate(result));
                            }
            new Storage();
            Box.Show("The Analysis was Successful. . .♬", "Notice", 3750);
        }
        public event EventHandler<ProgressRate> SendRate;
    }
}