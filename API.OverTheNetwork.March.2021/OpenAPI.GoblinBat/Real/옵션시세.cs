using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
    class 옵션시세 : Real
    {
        public override event EventHandler<SendSecuritiesAPI> Send;
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e) => Send?.Invoke(this, new SendSecuritiesAPI(e.sRealKey, API.GetCommRealData(e.sRealKey, Fid[0]), API.GetCommRealData(e.sRealKey, Fid[1]), API.GetCommRealData(e.sRealKey, Fid[6])));
        protected internal override int[] Fid => new int[] { 20, 10, 11, 12, 27, 28, 15, 13, 14, 16, 17, 18, 195, 182, 186, 190, 191, 193, 192, 194, 181, 25, 26, 137, 187, 197, 246, 247, 248, 219, 196, 188, 189, 30, 391, 392, 393, 1365, 1366, 1367, 305, 306 };
    }
}