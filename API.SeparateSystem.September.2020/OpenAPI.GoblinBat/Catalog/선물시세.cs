using System.Threading.Tasks;

using AxKHOpenAPILib;

using ShareInvest.Analysis;

namespace ShareInvest.OpenAPI.Catalog
{
    class 선물시세 : Real
    {
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
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        readonly int[] fid = new int[] { 20, 10, 11, 12, 27, 28, 15, 13, 14, 16, 17, 18, 195, 182, 184, 183, 186, 181, 185, 25, 197, 26, 246, 247, 248, 30, 196, 1365, 1366, 1367, 305, 306, };
    }
}