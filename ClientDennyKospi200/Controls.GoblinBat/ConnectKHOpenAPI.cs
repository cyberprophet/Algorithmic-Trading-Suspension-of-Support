using System.Windows.Forms;
using ShareInvest.Analysize;
using ShareInvest.Catalog;
using ShareInvest.Interface;
using ShareInvest.OpenAPI;

namespace ShareInvest.Controls
{
    public partial class ConnectKHOpenAPI : UserControl
    {
        public ConnectKHOpenAPI(IStatistics statistics)
        {
            InitializeComponent();
            api = ConnectAPI.Get();
            api.SetAPI(axAPI);
            api.StartProgress(new RealType());
            new Temporary();
            new Strategy(statistics);
        }
        private readonly ConnectAPI api;
    }
}