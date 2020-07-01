using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;
using ShareInvest.OpenAPI;

namespace ShareInvest.Catalog
{
    class KOA_CREATE_FO_ORD : TR
    {
        internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e) => Send?.Invoke(this, new SendSecuritiesAPI(API.GetCommData(e.sTrCode, e.sRQName, 0, ordNo)));
        internal override string ID
        {
            get;
        }
        internal override string Value
        {
            get; set;
        }
        internal override string RQName
        {
            get; set;
        }
        internal override string TrCode
        {
            get;
        }
        internal override int PrevNext
        {
            get; set;
        }
        internal override string ScreenNo
        {
            get;
        }
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        public override event EventHandler<SendSecuritiesAPI> Send;
        const string ordNo = "주문번호";
    }
}