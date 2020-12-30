using System.IO;

using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
    class 주식우선호가 : Real
    {
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        internal override StreamWriter Server
        {
            get; set;
        }
        internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            var param = base.OnReceiveRealData(e, Fid);

            if (string.IsNullOrEmpty(param[0]) == false && string.IsNullOrEmpty(param[1]) == false)
                Server.WriteLine(string.Concat(e.sRealType, '|', e.sRealKey, '|', param[0], ';', param[1]));
        }
        protected internal override int[] Fid => new int[] { 0x1B, 0x1C };
    }
}