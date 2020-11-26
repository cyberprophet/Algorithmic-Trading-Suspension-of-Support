using AxKHOpenAPILib;

using ShareInvest.Analysis;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주식호가잔량 : Real
    {
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            if (Connect.HoldingStock.TryGetValue(e.sRealKey, out Holding hs))
            {
                var param = base.OnReceiveRealData(e, fid);

                if (int.TryParse(param[1].StartsWith("-") ? param[1].Substring(1) : param[1], out int offer) && int.TryParse(param[4].StartsWith("-") ? param[4].Substring(1) : param[4], out int bid))
                {
                    hs.Offer = offer;
                    hs.Bid = bid;
                }
            }
        }
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        readonly int[] fid
            = new int[] { 21, 41, 61, 81, 51, 71, 91, 42, 62, 82, 52, 72, 92, 43, 63, 83, 53, 73, 93, 44, 64, 84, 54, 74, 94, 45, 65, 85, 55, 75, 95, 46, 66, 86, 56, 76, 96, 47, 67, 87, 57, 77, 97, 48, 68, 88, 58, 78, 98, 49, 69, 89, 59, 79, 99, 50, 70, 90, 60, 80, 100, 121, 122, 125, 126, 23, 24, 128, 129, 138, 139, 200, 201, 238, 291, 292, 293, 294, 295, 621, 631, 622, 632, 623, 633, 624, 634, 625, 635, 626, 636, 627, 637, 628, 638, 629, 639, 630, 640, 13, 299, 215, 216 };
    }
}