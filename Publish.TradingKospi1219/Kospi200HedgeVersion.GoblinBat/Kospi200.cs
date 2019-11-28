using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ShareInvest.Analysize;
using ShareInvest.Const;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.FindByName;
using ShareInvest.Management;
using ShareInvest.OpenAPI;
using ShareInvest.SecondaryForms;
using ShareInvest.TimerMessageBox;

namespace ShareInvest.Kospi200HedgeVersion
{
    public partial class Kospi200 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        public Kospi200()
        {
            InitializeComponent();
            SuspendLayout();
            SendMessageW(Handle, WM_APPCOMMAND, Handle, (IntPtr)APPCOMMAND_VOLUME_MUTE);
            ChooseStrategy(new GuideGoblinBat(), new ChooseAnalysis(), new SelectStrategies());
            Dispose();
            Environment.Exit(0);
        }
        private void ChooseStrategy(GuideGoblinBat guide, ChooseAnalysis analysis, SelectStrategies strategy)
        {
            analysis.SendClose += strategy.OnReceiveClose;
            strategy.OnReceiveClose(analysis.Key.Split('^'));
            splitContainerStrategy.Panel1.Controls.Add(analysis);
            splitContainerStrategy.Panel2.Controls.Add(strategy);
            splitContainerGuide.Panel1.Controls.Add(guide);
            analysis.Dock = DockStyle.Fill;
            strategy.Dock = DockStyle.Fill;
            guide.Dock = DockStyle.Fill;
            Size = new Size(1650, 920);
            splitContainerStrategy.SplitterDistance = 287;
            splitContainerStrategy.BackColor = Color.FromArgb(121, 133, 130);
            splitContainerGuide.Panel1.BackColor = Color.FromArgb(121, 133, 130);
            strategy.SendClose += OnReceiveClose;
            ResumeLayout();
            ShowDialog();
        }
        private void StartTrading(Balance bal, ConfirmOrder order, AccountSelection account, ConnectKHOpenAPI api)
        {
            Controls.Add(api);
            splitContainerBalance.Panel1.Controls.Add(order);
            splitContainerBalance.Panel2.Controls.Add(bal);
            api.Dock = DockStyle.Fill;
            order.Dock = DockStyle.Fill;
            bal.Dock = DockStyle.Fill;
            bal.BackColor = Color.FromArgb(203, 212, 206);
            order.BackColor = Color.FromArgb(121, 133, 130);
            api.Hide();
            account.SendSelection += OnReceiveAccount;
            splitContainerBalance.Panel2MinSize = 3;
            splitContainerBalance.Panel1MinSize = 96;
            order.SendTab += OnReceiveTabControl;
            bal.SendReSize += OnReceiveSize;
            ResumeLayout();
        }
        private void OnReceiveClose(object sender, DialogClose e)
        {
            SuspendLayout();
            StartTrading(Balance.Get(), ConfirmOrder.Get(), new AccountSelection(), new ConnectKHOpenAPI());
            strategy = new Strategy(new Specify
            {
                Reaction = e.Reaction,
                ShortDayPeriod = e.ShortDay,
                ShortTickPeriod = e.ShortTick,
                LongDayPeriod = e.LongDay,
                LongTickPeriod = e.LongTick,
                HedgeType = e.Hedge
            });
        }
        private void OnReceiveAccount(object sender, Account e)
        {
            account.Text = e.AccNo;
            id.Text = e.ID;
            ConnectAPI api = ConnectAPI.Get();
            api.SendDeposit += OnReceiveDeposit;
            api.LookUpTheDeposit(e.AccNo, true);
        }
        private void OnReceiveDeposit(object sender, Deposit e)
        {
            for (int i = 0; i < e.ArrayDeposit.Length; i++)
                if (e.ArrayDeposit[i].Length > 0)
                    string.Concat("balance", i).FindByName<Label>(this).Text = long.Parse(e.ArrayDeposit[i]).ToString("N0");

            splitContainerAccount.BackColor = Color.FromArgb(121, 133, 130);
            long trading = long.Parse(e.ArrayDeposit[20]), deposit = long.Parse(e.ArrayDeposit[19]);

            if (Account == false)
            {
                strategy.SetDeposit(new InQuiry { AccNo = account.Text, BasicAssets = deposit < trading || deposit < Deposit ? deposit : Deposit });

                return;
            }
            if (Account)
            {
                string[] assets = new Assets().ReadCSV().Split(',');
                long temp = 0, backtesting = long.Parse(assets[1]);
                DialogResult result = TimerBox.Show("Are You using Automatic Login??\n\nThe Automatic Login Compares the Asset setup\namount with the Current Asset during the Back Testing\nand sets a Small amount as a Deposit.\n\nIf You aren't using It,\nClick 'Cancel'.\n\nAfter 10 Seconds,\nIt's Regarded as an Automatic Mode and Proceeds.", "Notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 9617);

                switch (result)
                {
                    case DialogResult.OK:
                        temp = backtesting >= trading ? trading : backtesting;
                        break;

                    case DialogResult.Cancel:

                        if (TimerBox.Show(string.Concat("The set amount at the Time of the Test is ￦", backtesting.ToString("N0"), "\nand the Current Assets are ￦", trading.ToString("N0"), ".\n\nClick 'Yes' to set it to ￦", backtesting.ToString("N0"), ".\n\nIf You don't Choose,\nYou'll Set it as Current Asset."), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, 9712).Equals(DialogResult.No))
                            temp = trading;

                        else
                            temp = backtesting;

                        break;
                }
                Deposit = temp;
                Account = strategy.SetAccount(new InQuiry { AccNo = account.Text, BasicAssets = temp });
            }
        }
        private void OnReceiveSize(object sender, GridReSize e)
        {
            splitContainerBalance.SplitterDistance = splitContainerBalance.Height - e.ReSize - splitContainerBalance.SplitterWidth;
            CenterToScreen();

            if (e.Count < 8)
                FormSizes[2, 1] = 315;

            else if (e.Count > 7)
                FormSizes[2, 1] = 328 + (e.Count - 7) * 21;
        }
        private void OnReceiveTabControl(object sender, Mining e)
        {
            if (e.Tab != 9 && Tap == false)
            {
                Tap = true;
                tabControl.SelectedIndex = e.Tab;
            }
            else if (e.Tab == 9 && Tap)
                Close();
        }
        private void TabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            Size = new Size(FormSizes[tabControl.SelectedIndex, 0], FormSizes[tabControl.SelectedIndex, 1]);
            splitContainerBalance.AutoScaleMode = AutoScaleMode.Font;
            CenterToScreen();

            if (tabControl.SelectedIndex.Equals(3))
                BeginInvoke(new Action(() =>
                {
                    webBrowser.Navigate(@"https://sharecompany.tistory.com/guestbook");
                    splitContainerGuide.Panel2.BackColor = Color.FromArgb(118, 130, 127);
                    SendMessageW(Handle, WM_APPCOMMAND, Handle, (IntPtr)APPCOMMAND_VOLUME_UP);
                    webBrowser.Hide();
                }));

            else if (tabControl.SelectedIndex.Equals(1))
                BeginInvoke(new Action(() =>
                {
                    SendMessageW(Handle, WM_APPCOMMAND, Handle, (IntPtr)APPCOMMAND_VOLUME_DOWN);
                    webBrowser.Navigate(@"https://youtu.be/d1MQsMr4pxQ");
                    SendMessageW(Handle, WM_APPCOMMAND, Handle, (IntPtr)APPCOMMAND_VOLUME_MUTE);
                }));
        }
        private void ServerCheckedChanged(object sender, EventArgs e)
        {
            if (CheckCurrent)
            {
                server.Text = "During Auto Renew";
                server.ForeColor = Color.Ivory;
                account.ForeColor = Color.Ivory;
                id.ForeColor = Color.Ivory;
                timer.Interval = 19531;
                timer.Start();

                return;
            }
            timer.Stop();
            server.Text = "Stop Renewal";
            server.ForeColor = Color.Maroon;
        }
        private void TimerTick(object sender, EventArgs e)
        {
            ConnectAPI api = ConnectAPI.Get();
            api.LookUpTheDeposit(account.Text, api.OnReceiveBalance);

            if (DateTime.Now.Hour > 14 && DateTime.Now.Minute > 44)
                timer.Stop();
        }
        private long Deposit
        {
            get; set;
        }
        private bool CheckCurrent
        {
            get
            {
                return server.Checked;
            }
        }
        private bool Tap
        {
            get; set;
        }
        private bool Account
        {
            get; set;
        } = true;
        private int[,] FormSizes
        {
            get; set;
        } =
        {
            { 1650, 920 },
            { 750, 370 },
            { 594, 315 },
            { 405, 450 }
        };
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;
        private Strategy strategy;
    }
}