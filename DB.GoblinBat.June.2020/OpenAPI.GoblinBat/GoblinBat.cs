using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using ShareInvest.Catalog;

namespace ShareInvest.OpenAPI
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
            api = ShareInvest.Connect.ConnectAPI.Get();
            api.SetAPI(axAPI);
            api.StartProgress(new RealType(), new SqlConnection(new Connect().ConnectString));
        }
        private void GoblinBatResize(object sender, EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;
            }
        }
        private readonly ShareInvest.Connect.ConnectAPI api;
    }
}