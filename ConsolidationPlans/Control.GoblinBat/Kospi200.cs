using System.Windows.Forms;

namespace ShareInvest.Control
{
    public partial class Kospi200 : UserControl
    {
        public Kospi200()
        {
            InitializeComponent();

            api = Futures.Get();
            api.SetAPI(axAPI);
            api.StartProgress();
        }
        private readonly Futures api;
    }
}