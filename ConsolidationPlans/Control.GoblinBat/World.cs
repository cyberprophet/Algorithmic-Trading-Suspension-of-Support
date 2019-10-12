using System.Windows.Forms;

namespace ShareInvest.Control
{
    public partial class World : UserControl
    {
        public World()
        {
            InitializeComponent();
            api = Worlds.Get();
            api.SetAPI(axAPI);
            api.StartProgress();
        }
        private readonly Worlds api;
    }
}