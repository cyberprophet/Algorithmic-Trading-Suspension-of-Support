using System.Windows.Forms;

namespace ShareInvest.OpenAPI
{
    public partial class ConnectAPI : UserControl
    {
        public ConnectAPI()
        {
            InitializeComponent();
        }
        public int CommConnect() => axAPI.CommConnect();
    }
}