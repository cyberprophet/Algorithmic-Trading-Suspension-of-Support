using System;
using AxKFOpenAPILib;
using ShareInvest.AutoMessageBox;
using ShareInvest.Catalog;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Secret;

namespace ShareInvest
{
    public class Worlds : Conceal
    {
        public static Worlds Get()
        {
            if (api == null)
                api = new Worlds();

            return api;
        }
        public void SetAPI(AxKFOpenAPI axAPI)
        {
            this.axAPI = axAPI;
            axAPI.OnEventConnect += OnEventConnect;
        }
        public void StartProgress()
        {
            if (axAPI != null)
            {
                ErrorCode = axAPI.CommConnect(1);

                if (ErrorCode != 0)
                    new Error(ErrorCode);

                return;
            }
            Box.Show("API Not Found. . .", "Caution", waiting);
            SendExit?.Invoke(this, new ForceQuit(end));
        }
        private void OnEventConnect(object sender, _DKFOpenAPIEvents_OnEventConnectEvent e)
        {

        }
        private Worlds()
        {
            request = Delay.GetInstance(delay);
            request.Run();
        }
        private int ErrorCode
        {
            get; set;
        }
        private static Worlds api;
        private readonly Delay request;
        private AxKFOpenAPI axAPI;
        public event EventHandler<ForceQuit> SendExit;
    }
}