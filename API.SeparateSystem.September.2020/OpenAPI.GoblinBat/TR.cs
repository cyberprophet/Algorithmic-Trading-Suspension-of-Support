using System;

using AxKHOpenAPILib;

using ShareInvest.Catalog.OpenAPI;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI
{
    abstract class TR : ISendSecuritiesAPI
    {
        protected internal virtual string LookupScreenNo
        {
            get
            {
                if (count++ == 179)
                    count = 0;

                return (0xBB8 + count).ToString("D4");
            }
        }
        internal abstract void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e);
        internal abstract string ID
        {
            get;
        }
        internal abstract string Value
        {
            get; set;
        }
        internal abstract string RQName
        {
            get; set;
        }
        internal abstract string TrCode
        {
            get;
        }
        internal abstract int PrevNext
        {
            get; set;
        }
        internal abstract string ScreenNo
        {
            get;
        }
        internal abstract AxKHOpenAPI API
        {
            get; set;
        }
        static uint count;
        public abstract event EventHandler<SendSecuritiesAPI> Send;
    }
}