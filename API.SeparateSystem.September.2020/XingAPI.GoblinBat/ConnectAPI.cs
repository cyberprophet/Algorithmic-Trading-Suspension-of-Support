using System;
using System.Drawing;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.FindByName;
using ShareInvest.XingAPI.Catalog;

namespace ShareInvest.XingAPI
{
    public sealed partial class ConnectAPI : UserControl, ISecuritiesAPI
    {
        void ButtonStartProgressClick(object sender, EventArgs e)
        {
            if (textCertificate.Text.Length > 9 && textIdentity.Text.Length < 9 && textPassword.Text.Length < 9)
                BeginInvoke(new Action(() =>
                {
                    API = Connect.GetInstance(new Privacies
                    {
                        Identity = textIdentity.Text,
                        Password = textPassword.Text,
                        Certificate = textCertificate.Text
                    },
                    new LoadServer
                    {
                        Server = checkDemo.Checked ? demo : hts,
                        Date = labelMessage.Text
                    });
                    if (API is Connect api)
                        Send?.Invoke(this, new SendSecuritiesAPI(FormWindowState.Minimized, new Accounts(api.Accounts)));
                }));
            else
                buttonStartProgress.Text = error;
        }
        void OnReceiveControls(object sender, EventArgs e)
        {
            if (sender is TextBox text && text.ForeColor.Equals(Color.DarkGray))
            {
                if (e is PreviewKeyDownEventArgs key && key.KeyData.Equals(Keys.Tab))
                    return;

                text.UseSystemPasswordChar = text.Name.Equals(identity) == false;
                text.ForeColor = Color.Black;

                if (e is MouseEventArgs || text.UseSystemPasswordChar)
                    text.Text = string.Empty;

                text.MouseDown -= OnReceiveControls;
                text.PreviewKeyDown -= OnReceiveControls;
            }
        }
        void FindControlRecursive(Control control)
        {
            foreach (Control sub in control.Controls)
                if (sub.Controls.Count == 0 && sub.GetType().Name.Equals(text))
                {
                    var con = sub.Name.FindByName<TextBox>(this);
                    con.MouseDown += OnReceiveControls;
                    con.PreviewKeyDown += OnReceiveControls;
                }
                else if (sub.Controls.Count > 0)
                    FindControlRecursive(sub);
        }
        public AccountInformation SetPrivacy(Privacies privacy)
        {
            var ai = new AccountInformation
            {
                Identity = textIdentity.Text,
                Account = privacy.Account,
                Server = checkDemo.Checked
            };
            if (API is Connect api)
            {
                var name = api.SetAccountName(privacy.Account, privacy.AccountPassword);
                ai.Name = name.Item2;
                ai.Nick = name.Item3;

                if (checkPrivacy.Checked)
                    new Models.Privacy
                    {
                        SecuritiesAPI = (char)SecuritiesCOM.XingAPI,
                        Identity = textIdentity.Text,
                        Password = textPassword.Text,
                        Certificate = textCertificate.Text,
                        Account = privacy.Account,
                        AccountPassword = privacy.AccountPassword,
                        Server = checkDemo.Checked,
                        Date = labelMessage.Text,
                        IP = api.GetClientIP()
                    };
            }
            return ai;
        }
        public void SetForeColor(Color color) => labelXingAPI.ForeColor = color;
        public dynamic API
        {
            get; private set;
        }
        public ConnectAPI()
        {
            InitializeComponent();

            foreach (Control control in Controls)
                FindControlRecursive(control);
        }
        public readonly IQuerys<SendSecuritiesAPI>[] querys = (DateTime.Now.Hour == 15 && DateTime.Now.Minute < 45 || DateTime.Now.Hour < 15) && DateTime.Now.Hour > 4 ? new IQuerys<SendSecuritiesAPI>[] { new CFOBQ10500(), new T0441() } : new IQuerys<SendSecuritiesAPI>[] { new CCEBQ10500(), new CCEAQ50600() };
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}