using System.Windows.Forms;
using ShareInvest.OpenAPI;

namespace ShareInvest.DataBase
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
            api = ConnectAPI.GetInstance();
            api.SetAPI(axAPI);
            api.StartProgress();
            new Temporary();
            WindowState = FormWindowState.Minimized;
        }
        private void GoblinBatResize(object sender, System.EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;
            }
        }
        private readonly ConnectAPI api;
    }
}