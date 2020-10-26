using System.Text;

using AxKHOpenAPILib;

using ShareInvest.Analysis;

namespace ShareInvest.OpenAPI.Catalog
{
    class 옵션호가잔량 : Real
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

                if (double.TryParse(param[1].StartsWith("-") ? param[1].Substring(1) : param[1], out double offer) && double.TryParse(param[2].StartsWith("-") ? param[2].Substring(1) : param[2], out double bid))
                {
                    hs.Offer = offer;
                    hs.Bid = bid;
                }
            }
            if (Connect.Options != null && Connect.Options.TryGetValue(e.sRealKey, out Analysis.OpenAPI.Collect collect))
            {
                int i = 0;
                var time = API.GetCommRealData(e.sRealKey, fid[i]);

                if (string.Compare(time, initiate) > i && string.Compare(time, closing) < i)
                {
                    var sb = new StringBuilder();

                    for (i = 0; i < 0x30; i++)
                        sb.Append(API.GetCommRealData(e.sRealKey, fid[i + 3])).Append(';');

                    collect.ToCollect(time, sb.Remove(sb.Length - 1, 1));
                }
            }
        }
        const string initiate = "085959";
        const string closing = "153500";
        readonly int[] fid = new int[] { 21, 27, 28, 41, 61, 81, 101, 51, 71, 91, 111, 42, 62, 82, 102, 52, 72, 92, 112, 43, 63, 83, 103, 53, 73, 93, 113, 44, 64, 84, 104, 54, 74, 94, 114, 45, 65, 85, 105, 55, 75, 95, 115, 121, 122, 123, 125, 126, 127, 137, 128, 13, 23, 238, 200, 201, 291, 293, 294, 295 };
    }
}