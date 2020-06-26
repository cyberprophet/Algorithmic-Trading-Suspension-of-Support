using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;

namespace ShareInvest.XingAPI
{
    public sealed partial class ConnectAPI : UserControl, ISecuritiesAPI
    {
        public ConnectAPI()
        {
            InitializeComponent();
        }
        public dynamic API
        {
            get; private set;
        }
        void ButtonStartProgressClick(object sender, System.EventArgs e)
        {
            API = Connect.GetInstance(new Privacy
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
        }
    }
}