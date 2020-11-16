using System.IO;

using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
    class 선물시세 : Real
    {
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            string time = API.GetCommRealData(e.sRealKey, Fid[0]), current = API.GetCommRealData(e.sRealKey, Fid[1]), volume = API.GetCommRealData(e.sRealKey, Fid[6]);

            if (string.IsNullOrEmpty(volume) == false && string.IsNullOrEmpty(current) == false && string.IsNullOrEmpty(time) == false)
                Server.WriteLine(string.Concat(e.sRealType, '|', e.sRealKey, '|', time, ';', current, ';', volume));
        }
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override StreamWriter Server
        {
            get; set;
        }
        protected internal override int[] Fid => new int[] { 20, 10, 11, 12, 27, 28, 15, 13, 14, 16, 17, 18, 195, 182, 184, 183, 186, 181, 185, 25, 197, 26, 246, 247, 248, 30, 196, 1365, 1366, 1367, 305, 306, };
    }
}