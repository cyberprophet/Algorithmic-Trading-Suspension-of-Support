using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주문체결 : Chejan
    {
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override void OnReceiveChejanData(_DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {

        }
    }
}