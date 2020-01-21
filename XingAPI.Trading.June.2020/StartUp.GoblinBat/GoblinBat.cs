using System.Windows.Forms;
using ShareInvest.XingAPI;

namespace ShareInvest.StartUp
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            new ConnectAPI();
            InitializeComponent();
        }
    }
}