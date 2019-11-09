using System.Windows.Forms;

namespace ShareInvest.SecondaryForms
{
    public partial class SecondaryForm : Form
    {
        public SecondaryForm()
        {
            Controls.Add(AccountSelection.Get());
            InitializeComponent();
        }
    }
}