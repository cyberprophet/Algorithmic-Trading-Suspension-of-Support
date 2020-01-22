using System.Windows.Forms;

namespace ShareInvest.Controls
{
    public partial class ConnectKHOpenAPI : UserControl
    {
        public ConnectKHOpenAPI()
        {
            InitializeComponent();
            api = ConnectAPI.Get();
            api.SetAPI(axAPI);
            api.StartProgress(new RealType());
        }
        private readonly ConnectAPI api;
    }
}