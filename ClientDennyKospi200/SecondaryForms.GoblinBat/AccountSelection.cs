using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.OpenAPI;

namespace ShareInvest.SecondaryForms
{
    public partial class AccountSelection : UserControl
    {
        public static AccountSelection Get()
        {
            if (account == null)
                account = new AccountSelection();

            return account;
        }
        private void OnReceiveAccount(object sender, Account e)
        {
            foreach (string acc in e.AccountCategory)
                Box.Items.Add(acc);
        }
        private ComboBox Box
        {
            get; set;
        }
        private AccountSelection()
        {
            Box = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill
            };
            Controls.Add(Box);
            InitializeComponent();
            ConnectAPI.Get().SendAccount += OnReceiveAccount;
        }
        private static AccountSelection account;
    }
}