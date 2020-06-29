using System;
using System.Windows.Forms;

namespace ShareInvest.Controls
{
    public partial class Accounts : UserControl
    {
        void ButtonStartProgressClick(object sender, EventArgs e)
        {
            if (textPassword.TextLength == 4 && comboAccounts.SelectedItem is string str)
                Send?.Invoke(this, new EventHandler.SendSecuritiesAPI(this, str, textPassword.Text));
        }
        public Accounts(string[] accounts)
        {
            InitializeComponent();

            foreach (var str in accounts)
                comboAccounts.Items.Add(str.Insert(9, "-"));
        }
        public Accounts(string accounts)
        {
            InitializeComponent();
            textPassword.Text = open;
            textPassword.ReadOnly = true;

            foreach (var str in accounts.Split(';'))
                if (str.Length > 0)
                    comboAccounts.Items.Add(str.Insert(4, "-").Insert(9, "-"));
        }
        public event EventHandler<EventHandler.SendSecuritiesAPI> Send;
    }
}