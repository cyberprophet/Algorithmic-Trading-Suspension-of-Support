using System.Windows.Forms;
using ShareInvest.Communicate;
using ShareInvest.Publish;

namespace ShareInvest.Control
{
    public partial class ConnectKHOpenAPI : UserControl
    {
        public ConnectKHOpenAPI(IStrategy st)
        {
            InitializeComponent();
            api = PublicFutures.Get();
            api.SetAPI(axAPI);
            api.StartProgress(st);
        }
        private readonly PublicFutures api;
    }
}