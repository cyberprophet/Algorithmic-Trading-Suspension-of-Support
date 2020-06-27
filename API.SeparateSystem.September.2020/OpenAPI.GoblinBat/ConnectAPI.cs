using System;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI
{
    public sealed partial class ConnectAPI : UserControl, ISecuritiesAPI
    {
        public dynamic API
        {
            get; private set;
        }
        public ConnectAPI()
        {
            InitializeComponent();
        }
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}