using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.Const;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
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
        private void StartTrading(ConfirmOrder order, AccountSelection account, ConnectKHOpenAPI api)
        {
            Controls.Add(api);
            splitContainerBalance.Panel2.Controls.Add(order);
            api.Dock = DockStyle.Fill;
            order.Dock = DockStyle.Fill;
            order.BackColor = Color.FromArgb(203, 212, 206);
            api.Hide();
            account.SendSelection += OnReceiveAccount;
            ResumeLayout();
        }
        private void OnReceiveClose(object sender, DialogClose e)
        {
            SuspendLayout();
            StartTrading(ConfirmOrder.Get(), new AccountSelection(), new ConnectKHOpenAPI(new Specify
            {
                Reaction = e.Reaction,
                ShortDayPeriod = e.ShortDay,
                ShortTickPeriod = e.ShortTick,
                LongDayPeriod = e.LongDay,
                LongTickPeriod = e.LongTick
            }));
        }
        private void OnReceiveAccount(object sender, Account e)
        {
            account.Text = e.AccNo;
        }
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            Size = new Size(FormSizes[tabControl.SelectedIndex, 0], FormSizes[tabControl.SelectedIndex, 1]);
            CenterToScreen();
        }
        private int[,] FormSizes
        {
            get;
        } =
        {
            { 1650, 920 },
            { 900, 700 },
            { 600, 350 }
        };
    }
}