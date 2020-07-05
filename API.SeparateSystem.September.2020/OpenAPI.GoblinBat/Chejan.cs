using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI
{
    abstract class Chejan
    {
        protected internal virtual string[] OnReceiveChejanData(_DKHOpenAPIEvents_OnReceiveChejanDataEvent e, int[] fid)
        {
            var param = new string[fid.Length];

            for (int i = 0; i < fid.Length; i++)
                param[i] = API.GetChejanData(fid[i]);

            return param;
        }
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