using System.Windows.Forms;

using ShareInvest.Catalog;

namespace ShareInvest
{
    partial class SecuritiesAPI : Form
    {
        internal SecuritiesAPI(ISecuritiesAPI com)
        {
            this.com = com;
            InitializeComponent();
        }
        readonly ISecuritiesAPI com;
    }
}