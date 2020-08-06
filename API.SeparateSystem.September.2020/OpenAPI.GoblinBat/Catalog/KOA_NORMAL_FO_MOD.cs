using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
    class KOA_NORMAL_FO_MOD : TR
    {
        internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            var order = API.GetCommData(e.sTrCode, e.sRQName, 0, ordNo);
            Send?.Invoke(this, new SendSecuritiesAPI(order));
            SendMessage(order, e.sScrNo);
        }
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