using AxKHOpenAPILib;

using ShareInvest.Analysis;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주식우선호가 : Real
    {
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            if (Connect.HoldingStock.TryGetValue(e.sRealKey, out Holding hs))
            {
                var param = base.OnReceiveRealData(e, fid);

                if (int.TryParse(param[0].StartsWith("-") ? param[0].Substring(1) : param[0], out int offer) && int.TryParse(param[1].StartsWith("-") ? param[1].Substring(1) : param[1], out int bid))
                {
                    hs.Offer = offer;
                    hs.Bid = bid;
                }
            }
        }
        readonly int[] fid = new int[] { 0x1B, 0x1C };
    }
}