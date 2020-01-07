using System;
using System.Windows.Forms;
using ShareInvest.AutoMessageBox;
using ShareInvest.Const;
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
            if ((e.Server.Equals("1") ? new FreeVersion().Identify(e.ID, e.Name) : new VerifyIdentity().Identify(e.ID, e.Name)) == false)
            {
                Box.Show("The User is Not Registered.\n\nQuit the Program.", "Caution", 3750);
                Application.ExitThread();
                Environment.Exit(0);
            }
            foreach (string acc in e.AccountCategory)
                if (acc.Length > 0 && acc.Substring(8).Equals("31"))
                    comboBox.Items.Add(acc.Insert(4, "-").Insert(9, "-"));

            if (comboBox.Items.Count > 1)
            {
                comboBox.Name = e.ID;
                comboBox.MaxDropDownItems = int.Parse(e.Server);
                ShowDialog();

                return;
            }
            if (comboBox.Items.Count < 1)
            {
                Box.Show("The Futures Option Account does not Exist.\n\nQuit the Program.", "Notice", 3750);
                Application.ExitThread();
                Environment.Exit(0);

                return;
            }
            SendSelection?.Invoke(this, new Account(comboBox.Items[0].ToString(), e.ID, e.Server));
        }
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SendSelection?.Invoke(this, new Account(comboBox.Text, comboBox.Name, comboBox.MaxDropDownItems.ToString()));
            Close();
        }
        public event EventHandler<Account> SendSelection;
    }
}