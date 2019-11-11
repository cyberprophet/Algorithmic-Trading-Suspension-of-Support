using System;
using System.Drawing;
using System.Windows.Forms;
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
            StartTrading(ConfirmOrder.Get(), new ChooseAnalysis());
            Dispose();
            Environment.Exit(0);
        }
        private void StartTrading(ConfirmOrder order, ChooseAnalysis analysis)
        {
            using ConnectKHOpenAPI api = new ConnectKHOpenAPI();
            Size = new Size(1650, 900);
            Controls.Add(api);
            splitContainerStrategy.Panel1.Controls.Add(analysis);
            splitContainerBalance.Panel2.Controls.Add(order);
            analysis.Dock = DockStyle.Fill;
            api.Dock = DockStyle.Fill;
            order.Dock = DockStyle.Fill;
            order.BackColor = Color.FromArgb(203, 212, 206);
            api.Hide();
            new AccountSelection().SendSelection += OnReceiveAccount;
            analysis.SendClose += OnReceiveClose;
            ShowDialog();
            analysis.Dispose();
        }
        private void OnReceiveClose(object sender, DialogClose e)
        {
            splitContainerStrategy.Panel1.Controls.Clear();
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
            { 1650, 900 },
            { 900, 700 },
            { 600, 350 }
        };
    }
}