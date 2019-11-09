using System.Windows.Forms;
using ShareInvest.Controls;

namespace ShareInvest.Kospi200HedgeVersion
{
    public partial class Kospi200 : Form
    {
        public Kospi200()
        {
            InitializeComponent();
            StartTrading();
        }
        private void StartTrading()
        {
            using (ConnectKHOpenAPI api = new ConnectKHOpenAPI())
            {
                Controls.Add(api);
                api.Dock = DockStyle.Fill;
                api.Hide();
                ShowDialog();
            }
        }
    }
}