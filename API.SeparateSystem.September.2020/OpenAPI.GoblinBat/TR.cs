using System;
using System.Collections.Generic;
using System.Diagnostics;

using AxKHOpenAPILib;

using ShareInvest.Catalog.OpenAPI;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI
{
    abstract class TR : ISendSecuritiesAPI
    {
        [Conditional("DEBUG")]
        protected internal void SendMessage(string code, string message) => Console.WriteLine(code + "\t" + message);
        protected internal static uint Screen
        {
            get; set;
        }
        protected internal static uint Count
        {
            get; set;
        }
        protected internal virtual string LookupScreenNo
        {
            get
            {
                if (count++ == 0x95)
                    count = 0;

                return (0xBB8 + count).ToString("D4");
            }
        }
        protected internal virtual (string[], Stack<string[]>) OnReceiveTrData(string[] single, string[] multi, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            int i;
            var sTemp = single != null ? new string[single.Length] : null;

            if (single != null)
                for (i = 0; i < single.Length; i++)
                    sTemp[i] = API.GetCommData(e.sTrCode, e.sRQName, 0, single[i]);

            if (multi != null)
            {
                var catalog = new Stack<string[]>();

                for (int j = 0; j < API.GetRepeatCnt(e.sTrCode, e.sRQName); j++)
                {
                    var temp = new string[multi.Length];

                    for (i = 0; i < multi.Length; i++)
                        temp[i] = API.GetCommData(e.sTrCode, e.sRQName, j, multi[i]);

                    catalog.Push(temp);
                }
                return (sTemp, catalog);
            }
            return (sTemp, null);
        }
        internal abstract void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e);
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