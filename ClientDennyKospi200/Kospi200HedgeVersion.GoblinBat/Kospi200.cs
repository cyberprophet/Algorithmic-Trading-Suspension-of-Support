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
            StartTrading(ConfirmOrder.Get());
            Dispose();
            Environment.Exit(0);
        }
        private void StartTrading(ConfirmOrder order)
        {
            using ConnectKHOpenAPI api = new ConnectKHOpenAPI();
            Controls.Add(api);
            splitContainer.Panel2.Controls.Add(order);
            api.Dock = DockStyle.Fill;
            order.Dock = DockStyle.Fill;
            order.BackColor = Color.FromArgb(203, 212, 206);
            api.Hide();
            new AccountSelection().SendSelection += OnReceiveAccount;
            ShowDialog();
        }
        private void OnReceiveAccount(object sender, Account e)
        {
            account.Text = e.AccNo;
        }
    }
}