using AxKHOpenAPILib;

using ShareInvest.Analysis;

namespace ShareInvest.OpenAPI.Catalog
{
    class 선물호가잔량 : Real
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

                if (e.sRealKey[1].Equals('0') && double.TryParse(param[1].StartsWith("-") ? param[1].Substring(1) : param[1], out double offer) && double.TryParse(param[2].StartsWith("-") ? param[2].Substring(1) : param[2], out double bid))
                {
                    hs.Offer = offer;
                    hs.Bid = bid;
                }
                else if (int.TryParse(param[1].StartsWith("-") ? param[1].Substring(1) : param[1], out int sOffer) && int.TryParse(param[2].StartsWith("-") ? param[2].Substring(1) : param[2], out int sBid))
                {
                    hs.Offer = sOffer;
                    hs.Bid = sBid;
                }
            }
            if (Connect.Collect != null && Connect.Collect.TryGetValue(e.sRealKey, out Analysis.OpenAPI.Collect collect))
            {
                var temp = base.OnReceiveRealData(e, fid);

                if (string.Compare(temp[0], initiate) > 0 && string.Compare(temp[0], closing) < 0)
                {
                    collect.ToCollect(temp[0]);

                    for (int i = 0; i < 0x30; i++)
                        collect.Data.Append(temp[i + 3]).Append(';');

                    collect.Data.Replace(';', '|', collect.Data.Length - 1, 1);
                }
            }
        }
        const string initiate = "085959";
        const string closing = "153500";
        readonly int[] fid = new int[] { 21, 27, 28, 41, 61, 81, 101, 51, 71, 91, 111, 42, 62, 82, 102, 52, 72, 92, 112, 43, 63, 83, 103, 53, 73, 93, 113, 44, 64, 84, 104, 54, 74, 94, 114, 45, 65, 85, 105, 55, 75, 95, 115, 121, 122, 123, 125, 126, 127, 137, 128, 13, 23, 238, 200, 201, 291, 293, 294, 295 };
    }
}