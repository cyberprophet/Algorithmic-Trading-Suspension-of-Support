using System;
using System.Windows.Forms;

using ShareInvest.EventHandler;
using ShareInvest.Interface;

namespace ShareInvest.OpenAPI
{
    public sealed partial class ConnectAPI : UserControl, ISecuritiesAPI<SendSecuritiesAPI>
    {
        public ConnectAPI()
        {
            InitializeComponent();
            API = axAPI;
        }
        public dynamic API
        {
            get;
        }
        public bool Start
        {
            get; private set;
        }
        public void StartProgress()
        {

        }
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}