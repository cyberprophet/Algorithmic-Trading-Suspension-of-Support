using System.Windows.Forms;
using ShareInvest.Catalog;
using ShareInvest.OpenAPI;

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
            new Temporary();
        }
        private readonly ConnectAPI api;
    }
}