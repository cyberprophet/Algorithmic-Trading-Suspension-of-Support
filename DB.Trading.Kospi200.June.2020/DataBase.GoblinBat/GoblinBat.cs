using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.GoblinBatControls;
using ShareInvest.Interface.Struct;
using ShareInvest.Message;
using ShareInvest.NotifyIcon;
using ShareInvest.OpenAPI;
using ShareInvest.Strategy;

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
            CenterToScreen();
            BackColor = Color.FromArgb(121, 133, 130);
            api.SendCount += OnReceiveNotifyIcon;
            strip.ItemClicked += OnItemClick;
            WindowState = FormWindowState.Minimized;
        }
        private void OnItemClick(object sender, ToolStripItemClickedEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                switch (e.ClickedItem.Name)
                {
                    case "quotes":
                        Quotes.Show();
                        break;

                    case "exit":
                        Close();
                        return;

                    case "strategy":
                        Statistical.Show();
                        break;
                };
                SuspendLayout();
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                WindowState = FormWindowState.Normal;
                Application.DoEvents();
                ResumeLayout(true);
                PerformLayout();
            }));
        }
        private void OnReceiveNotifyIcon(object sender, NotifyIconText e)
        {
            switch (e.NotifyIcon.GetType().Name)
            {
                case "StatisticalAnalysis":
                    Statistical = (StatisticalAnalysis)e.NotifyIcon;
                    panel.Controls.Add(Statistical);
                    Statistical.Dock = DockStyle.Fill;
                    return;

                case "QuotesControl":
                    Quotes = (QuotesControl)e.NotifyIcon;
                    panel.Controls.Add(Quotes);
                    Quotes.Dock = DockStyle.Fill;
                    api.SendQuotes += Quotes.OnReceiveQuotes;
                    return;

                case "Int32":
                    if ((int)e.NotifyIcon == 0)
                    {
                        notifyIcon.Text = "GoblinBat";
                        api.StartProgress(3605);

                        return;
                    }
                    notifyIcon.Text = e.NotifyIcon.ToString();
                    return;

                case "StringBuilder":
                    var check = e.NotifyIcon.ToString().Split(';');
                    Account = new string[check.Length - 3];

                    if (check[check.Length - 1].Equals("1") ? false : new VerifyIdentity().Identify(check[check.Length - 3], check[check.Length - 2]) == false)
                    {
                        TimerBox.Show(new Message(check[check.Length - 2]).Identify, "Caution", MessageBoxButtons.OK, MessageBoxIcon.Warning, 3750);
                        Dispose();
                        Environment.Exit(0);

                        return;
                    }
                    for (int i = 0; i < check.Length - 3; i++)
                    {
                        Account[i] = check[i];
                    }
                    return;

                case "String":
                    var specify = new Specify
                    {
                        Account = Account,
                        Assets = 35000000,
                        Code = e.NotifyIcon.ToString(),
                        Time = 30,
                        Short = 4,
                        Long = 60
                    };
                    new Trading(api, specify, new Strategy.Quotes(specify, api));
                    return;
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
        private string[] Account
        {
            get; set;
        }
        private QuotesControl Quotes
        {
            get; set;
        }
        private StatisticalAnalysis Statistical
        {
            get; set;
        }
        private readonly ConnectAPI api;
    }
}