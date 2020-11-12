using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주식체결 : Real
    {
        public override event EventHandler<SendSecuritiesAPI> Send;
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e) => Send?.Invoke(this, new SendSecuritiesAPI(e.sRealKey, API.GetCommRealData(e.sRealKey, Fid[0]), API.GetCommRealData(e.sRealKey, Fid[1]), API.GetCommRealData(e.sRealKey, Fid[6])));
        protected internal override int[] Fid => new int[] { 20, 10, 11, 12, 27, 28, 15, 13, 14, 16, 17, 18, 25, 26, 29, 30, 31, 32, 228, 311, 290, 691, 567, 568 };
    }
}