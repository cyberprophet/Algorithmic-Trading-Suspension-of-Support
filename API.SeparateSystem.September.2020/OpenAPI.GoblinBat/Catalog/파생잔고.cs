using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
    class 파생잔고 : Chejan
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