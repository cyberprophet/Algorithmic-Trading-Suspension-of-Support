using System.Text;
using System.Threading.Tasks;

using AxKHOpenAPILib;

using ShareInvest.Analysis;
using ShareInvest.Analysis.OpenAPI;

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
            if (Connect.Collection != null && Connect.Collection.TryGetValue(e.sRealKey, out Collect collect))
            {
                string temp = API.GetCommRealData(e.sRealKey, fid[6]), time = API.GetCommRealData(e.sRealKey, fid[0]);

                if (string.Compare(time, initiate) > 0 && string.Compare(time, closing) < 0 && int.TryParse(temp, out int volume) && volume != 0)
                    collect.ToCollect(string.Concat(time, collect.GetTime(time[time.Length - 1])), new StringBuilder(API.GetCommRealData(e.sRealKey, fid[1])).Append(';').Append(temp));
            }
        }
        const string initiate = "085959";
        const string closing = "154559";
        readonly int[] fid = new int[] { 20, 10, 11, 12, 27, 28, 15, 13, 14, 16, 17, 18, 195, 182, 186, 190, 191, 193, 192, 194, 181, 25, 26, 137, 187, 197, 246, 247, 248, 219, 196, 188, 189, 30, 391, 392, 393, 1365, 1366, 1367, 305, 306 };
    }
}