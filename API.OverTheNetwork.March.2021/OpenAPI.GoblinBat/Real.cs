using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI
{
    abstract class Real : ISendSecuritiesAPI<SendSecuritiesAPI>
    {
        public abstract event EventHandler<SendSecuritiesAPI> Send;
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
        internal abstract void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e);
    }
}