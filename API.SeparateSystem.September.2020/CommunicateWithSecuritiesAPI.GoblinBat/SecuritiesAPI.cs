using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Controls;
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
            colors = new Color[] { Color.Maroon, Color.Ivory, Color.DeepSkyBlue };
            strip.ItemClicked += OnItemClick;
            timer.Start();
        }
        void StartProgress()
        {
            var control = (Control)com;
            Controls.Add(control);
            control.Dock = DockStyle.Fill;
            control.Show();
            Size = new Size(0x177, 0x127);
            Opacity = 0.8135;
            BackColor = Color.FromArgb(0x79, 0x85, 0x82);
            FormBorderStyle = FormBorderStyle.None;
            CenterToScreen();
            strategy.Text = balance;
            com.Send += OnReceiveSecuritiesAPI;
        }
        void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e)
        {
            if (e.Accounts == null && e.Convey is string message)
            {
                Console.WriteLine(message);
            }
            else if (e.Convey is FormWindowState state)
            {
                WindowState = state;
                com.Send -= OnReceiveSecuritiesAPI;
                ((Control)com).Hide();
                Controls.Add(e.Accounts);
                e.Accounts.Dock = DockStyle.Fill;
                e.Accounts.Show();
                Size = new Size(0x13B, 0x7D);
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                WindowState = FormWindowState.Normal;
                CenterToScreen();

                if (e.Accounts is Accounts accounts)
                    accounts.Send += OnReceiveSecuritiesAPI;
            }
            else if (e.Convey is string str && e.Accounts is Accounts accounts)
            {
                WindowState = FormWindowState.Minimized;
                accounts.Hide();
                accounts.Send -= OnReceiveSecuritiesAPI;

                if (com is XingAPI.ConnectAPI xing)
                {
                    var param = str.Split(';');
                    xing.SetPrivacy(new Catalog.XingAPI.Privacy
                    {
                        Account = param[0],
                        AccountPassword = param[1]
                    });
                    foreach (var ctor in xing.querys)
                    {
                        ctor.Send += OnReceiveSecuritiesAPI;
                        ctor.QueryExcute();
                    }
                }
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
            switch (MessageBox.Show(rExit, notifyIcon.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3))
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    WindowState = FormWindowState.Minimized;
                    return;

                case DialogResult.Yes:
                    break;
            }
            timer.Stop();
            strip.ItemClicked -= OnItemClick;
            Dispose();
        }
        void TimerTick(object sender, EventArgs e)
        {
            if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) == false)
                WindowState = FormWindowState.Minimized;

            else if (Controls.Contains((Control)com) == false && WindowState.Equals(FormWindowState.Minimized))
                strip.Items.Find(st, false).First(o => o.Name.Equals(st)).PerformClick();

            else if (Visible == false && ShowIcon == false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
                notifyIcon.Icon = (Icon)resources.GetObject(icon[DateTime.Now.Second % 4]);

            else if (Visible && ShowIcon && notifyIcon.Visible == false && FormBorderStyle.Equals(FormBorderStyle.None) && WindowState.Equals(FormWindowState.Normal) && com is XingAPI.ConnectAPI api)
                api.SetForeColor(colors[DateTime.Now.Second % 3]);
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
        readonly Color[] colors;
        readonly string[] icon;
    }
}