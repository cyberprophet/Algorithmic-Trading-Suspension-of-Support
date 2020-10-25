using System.Threading.Tasks;

using AxKHOpenAPILib;

using ShareInvest.Analysis;

namespace ShareInvest.OpenAPI.Catalog
{
    class 옵션시세 : Real
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
                new Task(() => hs.OnReceiveEvent(param)).Start();
            }
            if (Connect.Collect != null && Connect.Collect.TryGetValue(e.sRealKey, out Analysis.OpenAPI.Collect collect))
            {
                var temp = base.OnReceiveRealData(e, fid);

                if (int.TryParse(temp[6], out int volume) && volume != 0)
                {
                    collect.ToCollect(temp[0]);
                    collect.Data.Append(temp[1]).Append(';').Append(temp[6]).Append('|');
                }
            }
        }
        readonly int[] fid = new int[] { 20, 10, 11, 12, 27, 28, 15, 13, 14, 16, 17, 18, 195, 182, 186, 190, 191, 193, 192, 194, 181, 25, 26, 137, 187, 197, 246, 247, 248, 219, 196, 188, 189, 30, 391, 392, 393, 1365, 1366, 1367, 305, 306 };
    }
}