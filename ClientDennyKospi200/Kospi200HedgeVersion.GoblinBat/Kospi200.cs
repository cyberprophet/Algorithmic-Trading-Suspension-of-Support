using System.Windows.Forms;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.SecondaryForms;

namespace ShareInvest.Kospi200HedgeVersion
{
    public partial class Kospi200 : Form
    {
        public Kospi200()
        {
            InitializeComponent();
            StartTrading();
        }
        private void StartTrading()
        {
            using ConnectKHOpenAPI api = new ConnectKHOpenAPI();
            Controls.Add(api);
            api.Dock = DockStyle.Fill;
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