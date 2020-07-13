using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Client;
using ShareInvest.Controls;
using ShareInvest.EventHandler;

namespace ShareInvest
{
    partial class SecuritiesAPI : Form
    {
        internal SecuritiesAPI(Privacies privacy, ISecuritiesAPI com)
        {
            this.com = com;
            this.privacy = privacy;
            random = new Random();
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
                    switch (e.Convey)
                    {
                        case string message:
                            Balance.OnReceiveMessage(message);
                            return;

                        case Tuple<string, string, int, dynamic, dynamic, long, double> balance:
                            SuspendLayout();
                            Size = new Size(0x3B8, 0x63 + 0x28 + Balance.OnReceiveBalance(balance));
                            ResumeLayout();
                            break;

                        case long available:
                            Balance.OnReceiveDeposit(available);
                            break;

                        case Tuple<long, long> tuple:
                            Balance.OnReceiveDeposit(tuple);
                            return;

                        case Tuple<int, string> kw:
                            if (com is OpenAPI.ConnectAPI open)
                            {
                                var connect = open.InputValueRqData(optkwFID, kw.Item2, kw.Item1);

                                if (connect != null)
                                    connect.Send += OnReceiveSecuritiesAPI;
                            }
                            else if (com is XingAPI.ConnectAPI xing)
                            {

                            }
                            return;
                    }
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
                Opacity = 0;
                FormBorderStyle = FormBorderStyle.FixedSingle;
                WindowState = FormWindowState.Minimized;
                strategy.Text = balance;
                accounts.Hide();
                accounts.Send -= OnReceiveSecuritiesAPI;
                var param = str.Split(';');
                var info = com.SetPrivacy(com is OpenAPI.ConnectAPI ? new Privacies { Account = param[0] } : new Privacies
                {
                    Account = param[0],
                    AccountPassword = param[1]
                });
                Balance = new Balance(info);
                Controls.Add(Balance);
                Balance.Dock = DockStyle.Fill;
                Text = info.Nick;
                notifyIcon.Text = info.Nick;
                Opacity = 0.79315;

                if (com is XingAPI.ConnectAPI connect)
                {
                    var ctor = connect.ConvertTheCodeToName();
                    ctor.Send += OnReceiveSecuritiesAPI;
                    ctor.QueryExcute();
                    Connect = int.MaxValue;
                }
            }
        }
        void GoblinBatResize(object sender, EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                SuspendLayout();
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;

                if (strategy.Text.Equals(balance) && Balance != null)
                {
                    Balance.Hide();

                    if (com is OpenAPI.ConnectAPI openAPI)
                    {
                        openAPI.OnConnectErrorMessage.Send -= OnReceiveSecuritiesAPI;
                        openAPI.Send -= OnReceiveSecuritiesAPI;
                        openAPI.InputValueRqData(false, opw00005).Send -= OnReceiveSecuritiesAPI;

                        var connect = openAPI.InputValueRqData(optkwFID, null, 0);

                        if (connect != null)
                            connect.Send -= OnReceiveSecuritiesAPI;

                        foreach (var ctor in openAPI.HoldingStocks)
                        {
                            Balance.SetDisconnectHoldingStock(ctor);
                            ctor.SendBalance -= OnReceiveSecuritiesAPI;
                        }
                    }
                    else if (com is XingAPI.ConnectAPI xing)
                        foreach (var ctor in xing.querys)
                            ctor.Send -= OnReceiveSecuritiesAPI;
                }
                ResumeLayout();
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
                    var code = GoblinBatClient.DeleteContext<Privacies>(privacy);

                    if (code > 0xC8)
                    {
                        notifyIcon.Text = OnReceiveErrorMessage(code);
                        e.Cancel = true;
                        WindowState = FormWindowState.Minimized;

                        return;
                    }
                    break;
            }
            timer.Stop();
            strip.ItemClicked -= OnItemClick;
            Dispose();
        }
        void TimerTick(object sender, EventArgs e)
        {
            if (com == null)
            {
                timer.Stop();
                strip.ItemClicked -= OnItemClick;
                Dispose();
            }
            else if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) == false)
                WindowState = FormWindowState.Minimized;

            else if (Controls.Contains((Control)com) == false && WindowState.Equals(FormWindowState.Minimized))
                strip.Items.Find(st, false).First(o => o.Name.Equals(st)).PerformClick();

            else if (Visible && ShowIcon && notifyIcon.Visible == false && FormBorderStyle.Equals(FormBorderStyle.None) && WindowState.Equals(FormWindowState.Normal) && (com is XingAPI.ConnectAPI || com is OpenAPI.ConnectAPI))
            {
                var now = DateTime.Now;
                var day = 0;

                switch (now.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        day = now.AddDays(1).Day;
                        break;

                    case DayOfWeek.Saturday:
                        day = now.AddDays(2).Day;
                        break;

                    case DayOfWeek weeks when weeks.Equals(DayOfWeek.Friday) && now.Hour > 8:
                        day = now.AddDays(3).Day;
                        break;

                    default:
                        day = (now.Hour > 8 || Array.Exists(holidays, o => o.Equals(now.ToString(dFormat))) ? now.AddDays(1) : now).Day;
                        break;
                }
                var remain = new DateTime(now.Year, now.Month, day, 9, 0, 0) - DateTime.Now;
                com.SetForeColor(colors[DateTime.Now.Second % 3], GetRemainingTime(remain));

                if (remain.TotalMinutes < 0x1F && com.Start == false && DateTime.Now.Hour == 8 && DateTime.Now.Minute > 0x1E && (Connect > 0x4B0 || random.Next(Connect++, 0x4B1) == 0x4B0))
                    com.StartProgress();
            }
            else if (Visible == false && ShowIcon == false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
                notifyIcon.Icon = (Icon)resources.GetObject(icon[DateTime.Now.Second % 4]);
        }
        void OnItemClick(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name.Equals(st))
            {
                if (strategy.Text.Equals(balance) && Balance != null)
                {
                    if (com is XingAPI.ConnectAPI xingAPI)
                    {
                        foreach (var ctor in xingAPI.querys)
                        {
                            ctor.Send += OnReceiveSecuritiesAPI;
                            ctor.QueryExcute();
                        }
                        if (Connect == int.MaxValue)
                        {
                            xingAPI.ConvertTheCodeToName().Send -= OnReceiveSecuritiesAPI;
                            Connect = int.MinValue;
                        }
                    }
                    else if (com is OpenAPI.ConnectAPI openAPI)
                    {
                        openAPI.OnConnectErrorMessage.Send += OnReceiveSecuritiesAPI;
                        openAPI.Send += OnReceiveSecuritiesAPI;
                        openAPI.InputValueRqData(true, opw00005).Send += OnReceiveSecuritiesAPI;

                        foreach (var ctor in openAPI.HoldingStocks)
                        {
                            Balance.SetConnectHoldingStock(ctor);
                            ctor.SendBalance += OnReceiveSecuritiesAPI;
                        }
                    }
                    Size = new Size(0x3B8, 0x63 + 0x28);
                    Balance.Show();
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
        int Connect
        {
            get; set;
        }
        readonly Random random;
        readonly Privacies privacy;
        readonly ISecuritiesAPI com;
        readonly Color[] colors;
        readonly string[] icon;
    }
}