using System.Threading.Tasks;

using AxKHOpenAPILib;

using ShareInvest.Catalog;

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
            var param = base.OnReceiveRealData(e, fid);

            if (Connect.HoldingStock.TryGetValue(e.sRealKey, out HoldingStocks hs))
                new Task(() => hs.OnReceiveEvent(param)).Start();
        }
        readonly int[] fid = new int[] { 20, 10, 11, 12, 27, 28, 15, 13, 14, 16, 17, 18, 25, 26, 29, 30, 31, 32, 228, 311, 290, 691, 567, 568 };
    }
}