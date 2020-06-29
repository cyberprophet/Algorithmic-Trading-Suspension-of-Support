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
            Size = new Size(0x177, com is XingAPI.ConnectAPI ? 0x127 : 0xC3);
            Opacity = 0.8135;
            BackColor = Color.FromArgb(0x79, 0x85, 0x82);
            FormBorderStyle = FormBorderStyle.None;
            CenterToScreen();
            com.Send += OnReceiveSecuritiesAPI;
        }
        void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e)
        {
            if (e.Accounts == null && Balance != null)
                BeginInvoke(new Action(() =>
                {
                    if (e.Convey is string message)
                        Balance.OnReceiveMessage(message);

                    else if (e.Convey is Tuple<long, long> tuple)
                        Balance.OnReceiveDeposit(tuple);
                }));
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
                FormBorderStyle = FormBorderStyle.FixedSingle;
                WindowState = FormWindowState.Minimized;
                strategy.Text = balance;
                accounts.Hide();
                accounts.Send -= OnReceiveSecuritiesAPI;
                var param = str.Split(';');
                var info = com.SetPrivacy(com is OpenAPI.ConnectAPI ? new Privacy { Account = param[0] } : new Privacy
                {
                    Account = param[0],
                    AccountPassword = param[1]
                });
                Balance = new Balance(info);
                Controls.Add(Balance);
                Balance.Dock = DockStyle.Fill;
                Text = info.Nick;
                notifyIcon.Text = info.Nick;
            }
        }
        void GoblinBatResize(object sender, EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;

                if (strategy.Text.Equals(balance) && Balance != null)
                {
                    Balance.Hide();

                    if (com is XingAPI.ConnectAPI api)
                        foreach (var ctor in api.querys)
                            ctor.Send -= OnReceiveSecuritiesAPI;
                }
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

            else if (Visible && ShowIcon && notifyIcon.Visible == false && FormBorderStyle.Equals(FormBorderStyle.None) && WindowState.Equals(FormWindowState.Normal) && (com is XingAPI.ConnectAPI || com is OpenAPI.ConnectAPI))
                com.SetForeColor(colors[DateTime.Now.Second % 3]);
        }
        void OnItemClick(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name.Equals(st))
            {
                if (strategy.Text.Equals(balance) && Balance != null)
                {
                    Balance.Show();
                    Size = new Size(0x3B8, 0x63 + 0x28);

                    if (com is XingAPI.ConnectAPI api)
                        foreach (var ctor in api.querys)
                        {
                            ctor.Send += OnReceiveSecuritiesAPI;
                            ctor.QueryExcute();
                        }
                }
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                WindowState = FormWindowState.Normal;

                if (com is XingAPI.ConnectAPI xing && xing.API == null || com is OpenAPI.ConnectAPI open && open.API == null)
                    StartProgress();
            }
            else
                Close();
        }
        Balance Balance
        {
            get; set;
        }
        readonly ISecuritiesAPI com;
        readonly Color[] colors;
        readonly string[] icon;
    }
}