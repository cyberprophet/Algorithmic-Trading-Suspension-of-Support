using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI
{
    abstract class Chejan
    {
        internal abstract AxKHOpenAPI API
        {
            get; set;
        }
        internal abstract void OnReceiveChejanData(_DKHOpenAPIEvents_OnReceiveChejanDataEvent e);
    }
    enum ChejanType
    {
        주문체결 = 0,
        잔고 = 1,
        파생잔고 = 4
    }
}