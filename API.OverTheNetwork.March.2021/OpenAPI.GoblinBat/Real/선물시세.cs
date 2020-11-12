using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
    class 선물시세 : Real
    {
        public override event EventHandler<SendSecuritiesAPI> Send;
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e) => Send?.Invoke(this, new SendSecuritiesAPI(e.sRealKey, API.GetCommRealData(e.sRealKey, Fid[0]), API.GetCommRealData(e.sRealKey, Fid[1]), API.GetCommRealData(e.sRealKey, Fid[6])));
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        protected internal override int[] Fid => new int[] { 20, 10, 11, 12, 27, 28, 15, 13, 14, 16, 17, 18, 195, 182, 184, 183, 186, 181, 185, 25, 197, 26, 246, 247, 248, 30, 196, 1365, 1366, 1367, 305, 306, };
    }
}