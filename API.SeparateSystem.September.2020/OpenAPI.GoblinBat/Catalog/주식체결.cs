using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주식체결 : Real
    {
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {

        }
    }
}