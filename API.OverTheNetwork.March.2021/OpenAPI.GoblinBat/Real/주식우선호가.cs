using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주식우선호가 : Real
    {
        public override event EventHandler<SendSecuritiesAPI> Send;
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            var param = base.OnReceiveRealData(e, Fid);

            if (string.IsNullOrEmpty(param[0]) == false && string.IsNullOrEmpty(param[1]) == false && int.TryParse(param[0][0] == '-' ? param[0][1..] : param[0], out int offer) && int.TryParse(param[1][0] == '-' ? param[1][1..] : param[1], out int bid))
                Send?.Invoke(this, new SendSecuritiesAPI(e.sRealKey, offer, bid));
        }
        protected internal override int[] Fid => new int[] { 0x1B, 0x1C };
    }
}