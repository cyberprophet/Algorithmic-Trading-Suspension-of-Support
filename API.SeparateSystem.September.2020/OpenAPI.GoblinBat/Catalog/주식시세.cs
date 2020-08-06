using AxKHOpenAPILib;

using ShareInvest.Analysis;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주식시세 : Real
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

                if (int.TryParse(param[3].StartsWith("-") ? param[3].Substring(1) : param[3], out int offer) && int.TryParse(param[4].StartsWith("-") ? param[4].Substring(1) : param[4], out int bid) && int.TryParse(param[0].StartsWith("-") ? param[0].Substring(1) : param[0], out int current))
                {
                    hs.Current = current;
                    hs.Offer = offer;
                    hs.Bid = bid;
                }
            }
        }
        readonly int[] fid = new int[] { 10, 11, 12, 27, 28, 13, 14, 16, 17, 18, 25, 26, 29, 30, 31, 32, 311, 567, 568 };
    }
}