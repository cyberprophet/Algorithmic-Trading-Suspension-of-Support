using System.IO;

using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI
{
    abstract class Real
    {
        protected internal virtual string[] OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e, int[] fid)
        {
            var param = new string[fid.Length];

            for (int i = 0; i < fid.Length; i++)
                param[i] = API.GetCommRealData(e.sRealKey, fid[i]);

            return param;
        }
        protected internal abstract int[] Fid
        {
            get;
        }
        internal abstract AxKHOpenAPI API
        {
            get; set;
        }
        internal abstract StreamWriter Server
        {
            get; set;
        }
        internal abstract void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e);
    }
}