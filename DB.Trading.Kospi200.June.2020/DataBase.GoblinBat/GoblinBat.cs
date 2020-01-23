using System;
using System.Windows.Forms;
using ShareInvest.EventHandler;
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
            api.SendCount += OnReceiveNotifyIcon;
        }
        private void OnReceiveNotifyIcon(object sender, NotifyIconText e)
        {
            notifyIcon.Text = e.Count;

            if (e.Count.Equals("0"))
                notifyIcon.Text = "GoblinBat";
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
        private readonly ConnectAPI api;
    }
}