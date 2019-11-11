using System;
using System.Windows.Forms;
using ShareInvest.AutoMessageBox;
using ShareInvest.EventHandler;
using ShareInvest.OpenAPI;

namespace ShareInvest.SecondaryForms
{
    public partial class AccountSelection : Form
    {
        public AccountSelection()
        {
            InitializeComponent();
            ConnectAPI.Get().SendAccount += OnReceiveAccount;
        }
        private void OnReceiveAccount(object sender, Account e)
        {
            foreach (string acc in e.AccountCategory)
                if (acc.Length > 0 && acc.Substring(8).Equals("31"))
                    comboBox.Items.Add(acc.Insert(4, "-").Insert(9, "-"));

            if (comboBox.Items.Count > 1)
            {
                ShowDialog();

                return;
            }
            if (comboBox.Items.Count < 1)
            {
                Box.Show("The Futures Option Account does not Exist.\n\nQuit the program.", "Notice", 3750);
                Environment.Exit(0);

                return;
            }
            SendSelection?.Invoke(this, new Account(comboBox.Items[0].ToString()));
        }
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SendSelection?.Invoke(this, new Account(comboBox.Text));
            Close();
        }
        public event EventHandler<Account> SendSelection;
    }
}