using System;
using System.Windows.Forms;
using ShareInvest.Analysize;
using ShareInvest.Communicate;
using ShareInvest.EventHandler;
using ShareInvest.Publish;

namespace ShareInvest.Control
{
    public partial class ConnectKHOpenAPI : UserControl
    {
        public ConnectKHOpenAPI(IStrategy st)
        {
            InitializeComponent();
            api = PublicFutures.Get();
            new Strategy(st);
            new Temporary().Send += OnReceiveExit;
            api.SetAPI(axAPI);
            api.StartProgress(st);
        }
        private void OnReceiveExit(object sender, ForceQuit e)
        {
            SendQuit?.Invoke(this, new ForceQuit(1));
        }
        private readonly PublicFutures api;
        public event EventHandler<ForceQuit> SendQuit;
    }
}