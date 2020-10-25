using System.Threading.Tasks;

using AxKHOpenAPILib;

using ShareInvest.Analysis;

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
        readonly int[] fid = new int[] { 20, 10, 11, 12, 27, 28, 15, 13, 14, 16, 17, 18, 25, 26, 29, 30, 31, 32, 228, 311, 290, 691, 567, 568 };
    }
}