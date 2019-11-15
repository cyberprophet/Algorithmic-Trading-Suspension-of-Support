using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.Analysize;
using ShareInvest.Const;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.FindByName;
using ShareInvest.OpenAPI;
using ShareInvest.SecondaryForms;

namespace ShareInvest.Kospi200HedgeVersion
{
    public partial class Kospi200 : Form
    {
        public Kospi200()
        {
            InitializeComponent();
            ChooseStrategy(new ChooseAnalysis(), new SelectStrategies());
            Dispose();
            Environment.Exit(0);
        }
        private void ChooseStrategy(ChooseAnalysis analysis, SelectStrategies strategy)
        {
            analysis.SendClose += strategy.OnReceiveClose;
            strategy.OnReceiveClose(analysis.Key.Split('^'));
            splitContainerStrategy.Panel1.Controls.Add(analysis);
            splitContainerStrategy.Panel2.Controls.Add(strategy);
            analysis.Dock = DockStyle.Fill;
            strategy.Dock = DockStyle.Fill;
            Size = new Size(1650, 920);
            splitContainerStrategy.SplitterDistance = 287;
            splitContainerStrategy.BackColor = Color.FromArgb(121, 133, 130);
            strategy.SendClose += OnReceiveClose;
            ShowDialog();
        }
        private void StartTrading(Balance bal, ConfirmOrder order, AccountSelection account, ConnectKHOpenAPI api)
        {
            Controls.Add(api);
            splitContainerBalance.Panel2.Controls.Add(order);
            splitContainerBalance.Panel1.Controls.Add(bal);
            api.Dock = DockStyle.Fill;
            order.Dock = DockStyle.Fill;
            bal.Dock = DockStyle.Fill;
            bal.BackColor = Color.FromArgb(203, 212, 206);
            order.BackColor = Color.FromArgb(121, 133, 130);
            api.Hide();
            account.SendSelection += OnReceiveAccount;
            splitContainerBalance.SplitterDistance = 544;
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
            api.LookUpTheDeposit(e.AccNo);
        }
        private void OnReceiveDeposit(object sender, Deposit e)
        {
            for (int i = 0; i < e.ArrayDeposit.Length; i++)
                if (e.ArrayDeposit[i].Length > 0)
                    string.Concat("balance", i).FindByName<Label>(this).Text = int.Parse(e.ArrayDeposit[i]).ToString("N0");

            splitContainerAccount.BackColor = Color.FromArgb(121, 133, 130);
            tabControl.SelectedIndex = 1;
            strategy.SetAccount(new InQuiry { AccNo = account.Text, BasicAssets = long.Parse(e.ArrayDeposit[26]) });
        }
        private void TabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            Size = new Size(FormSizes[tabControl.SelectedIndex, 0], FormSizes[tabControl.SelectedIndex, 1]);
            splitContainerBalance.AutoScaleMode = AutoScaleMode.Font;
            CenterToScreen();
        }
        private void ServerCheckedChanged(object sender, EventArgs e)
        {
            if (CheckCurrent)
            {
                server.Text = "During Auto Renew";
                server.ForeColor = Color.Ivory;
                account.ForeColor = Color.Ivory;
                id.ForeColor = Color.Ivory;
                timer.Start();

                return;
            }
            timer.Stop();
            server.Text = "Stop Renewal";
            server.ForeColor = Color.Maroon;
        }
        private void TimerTick(object sender, EventArgs e)
        {
            ConnectAPI.Get().LookUpTheDeposit(account.Text);
        }
        private bool CheckCurrent
        {
            get
            {
                return server.Checked;
            }
        }
        private int[,] FormSizes
        {
            get;
        } =
        {
            { 1650, 920 },
            { 750, 370 },
            { 594, 321 }
        };
        private Strategy strategy;
    }
}