using System.Windows.Forms;

using ShareInvest.Catalog;

namespace ShareInvest.XingAPI
{
    public sealed partial class ConnectAPI : UserControl, ISecuritiesAPI
    {
        public ConnectAPI()
        {
            InitializeComponent();
        }
        public char API
        {
            get;
        }
    }
}