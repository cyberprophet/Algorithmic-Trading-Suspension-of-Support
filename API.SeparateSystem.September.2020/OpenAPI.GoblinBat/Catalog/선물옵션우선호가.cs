using AxKHOpenAPILib;

using ShareInvest.Analysis;

namespace ShareInvest.OpenAPI.Catalog
{
    class 선물옵션우선호가 : Real
    {
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            if (Connect.HoldingStock.TryGetValue(e.sRealKey, out Holding hs))
            {
                var param = base.OnReceiveRealData(e, fid);

                if (e.sRealKey[1].Equals('0') && double.TryParse(param[0].StartsWith("-") ? param[0].Substring(1) : param[0], out double current) && double.TryParse(param[1].StartsWith("-") ? param[1].Substring(1) : param[1], out double offer) && double.TryParse(param[2].StartsWith("-") ? param[2].Substring(1) : param[2], out double bid))
                {
                    hs.Current = current;
                    hs.Offer = offer;
                    hs.Bid = bid;
                }
                else if (int.TryParse(param[0].StartsWith("-") ? param[0].Substring(1) : param[0], out int price) && int.TryParse(param[1].StartsWith("-") ? param[1].Substring(1) : param[1], out int sq) && int.TryParse(param[2].StartsWith("-") ? param[2].Substring(1) : param[2], out int bq))
                {
                    hs.Current = price;
                    hs.Offer = sq;
                    hs.Bid = bq;
                }
            }
        }
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        readonly int[] fid = new int[] { 10, 27, 28 };
    }
}