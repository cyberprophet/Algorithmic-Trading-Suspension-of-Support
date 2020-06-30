using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI
{
    abstract class Real
    {
        internal abstract AxKHOpenAPI API
        {
            get; set;
        }
        internal abstract void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e);
    }
}