using System.Windows.Forms;

namespace ShareInvest.Controls
{
    public partial class Accounts : UserControl
    {
        public Accounts(string[] accounts)
        {
            InitializeComponent();

            foreach (var str in accounts)
                comboAccounts.Items.Add(str.Insert(9, "-"));
        }
        public Accounts(string accounts)
        {
            InitializeComponent();
            textPassword.ReadOnly = true;

            foreach (var str in accounts.Split(';'))
                comboAccounts.Items.Add(str.Insert(4, "-").Insert(9, "-"));
        }
    }
}