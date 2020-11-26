using System.Threading.Tasks;

using AxKHOpenAPILib;

using ShareInvest.Analysis;

namespace ShareInvest.OpenAPI.Catalog
{
    class 잔고 : Chejan
    {
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override void OnReceiveChejanData(_DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            var param = base.OnReceiveChejanData(e, fid);

            if (Connect.HoldingStock.TryGetValue(param[1].Substring(1), out Holding hs))
                new Task(() => hs.OnReceiveBalance(param)).Start();
        }
        readonly int[] fid
            = new int[] { 9201, 9001, 917, 916, 302, 10, 930, 931, 932, 933, 945, 946, 950, 951, 27, 28, 307, 8019, 957, 958, 918, 990, 991, 992, 993, 959, 924 };
    }
}