using System;
using System.Drawing;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest
{
    partial class SecuritiesAPI : Form
    {
        internal SecuritiesAPI(ISecuritiesAPI com)
        {
            this.com = com;
            InitializeComponent();
            icon = new string[] { mono, duo, tri, quad };
            strip.ItemClicked += OnItemClick;
            timer.Start();
        }
        void StartProgress()
        {
            var control = (Control)com;
            Controls.Add(control);
            control.Dock = DockStyle.Fill;
            control.Show();
            Size = new Size(0x177, 0x113);
            Opacity = 0.8135;
            BackColor = Color.FromArgb(0x79, 0x85, 0x82);
            FormBorderStyle = FormBorderStyle.None;
            CenterToScreen();
            com.Send += OnReceiveSecuritiesAPI;
        }
        void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e)
        {
            if (e.Convey is FormWindowState state)
            {
                WindowState = state;
                com.Send -= OnReceiveSecuritiesAPI;
            }
        }
        void GoblinBatResize(object sender, EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;
            }
        }
        void GoblinBatFormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show(rExit, notifyIcon.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result.Equals(DialogResult.Cancel))
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;

                return;
            }
            if (result.Equals(DialogResult.Yes))
            {

            }
            timer.Stop();
            strip.ItemClicked -= OnItemClick;
            Dispose();
        }
        void TimerTick(object sender, EventArgs e)
        {
            if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) == false)
                WindowState = FormWindowState.Minimized;

            notifyIcon.Icon = (Icon)resources.GetObject(icon[DateTime.Now.Second % 4]);
        }
        void OnItemClick(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name.Equals(st))
            {
                if (com is XingAPI.ConnectAPI xing && xing.API != null)
                {


                    return;
                }
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                WindowState = FormWindowState.Normal;
                StartProgress();
            }
            else
                Close();
        }
        readonly ISecuritiesAPI com;
        readonly string[] icon;
    }
}