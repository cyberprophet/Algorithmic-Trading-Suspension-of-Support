using System;
using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.OpenAPI;
using ShareInvest.GoblinBatControls;
using ShareInvest.Strategy;
using ShareInvest.Interface.Struct;

namespace ShareInvest.GoblinBatForms
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
            strip.ItemClicked += OnItemClick;
        }
        private void OnItemClick(object sender, ToolStripItemClickedEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                switch (e.ClickedItem.Name)
                {
                    case "exit":
                        Close();
                        return;

                    case "strategy":
                        if (panel.Controls.Contains(Statistical) == false)
                            panel.Controls.Add(Statistical);

                        Statistical.Dock = DockStyle.Fill;
                        break;
                };
                SuspendLayout();
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                WindowState = FormWindowState.Normal;
                ResumeLayout();
            }));
        }
        private void OnReceiveNotifyIcon(object sender, NotifyIconText e)
        {
            switch (e.NotifyIcon.GetType().Name)
            {
                case "StatisticalAnalysis":
                    Statistical = (StatisticalAnalysis)e.NotifyIcon;
                    break;

                case "Int32":
                    notifyIcon.Text = e.NotifyIcon.ToString().Equals("0") ? "GoblinBat" : e.NotifyIcon.ToString();
                    break;

                case "String":
                    new Trading(new Specify
                    {
                        Assets = 35000000,
                        Code = e.NotifyIcon.ToString(),
                        Time = 30,
                        Short = 4,
                        Long = 60
                    });
                    break;
            };
        }
        private void GoblinBatFormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(new Message().Exit, "Caution", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning).Equals(DialogResult.Cancel))
            {
                e.Cancel = true;

                return;
            }
            Dispose();
            Environment.Exit(0);
        }
        private void GoblinBatResize(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                if (WindowState.Equals(FormWindowState.Minimized))
                {
                    Visible = false;
                    ShowIcon = false;
                    notifyIcon.Visible = true;

                    return;
                }
            }));
        }
        private StatisticalAnalysis Statistical
        {
            get; set;
        }
        private readonly ConnectAPI api;
    }
}