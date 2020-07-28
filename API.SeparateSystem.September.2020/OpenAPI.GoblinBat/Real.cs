using System;
using System.Diagnostics;

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
        [Conditional("DEBUG")]
        protected internal void SendMessage(string message) => Console.WriteLine(message);
        internal abstract AxKHOpenAPI API
        {
            get; set;
        }
        internal abstract void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e);
    }
    enum Operation
    {
        장시작전 = 0,
        장마감전_동시호가 = 2,
        장시작 = 3,
        장종료_예상지수종료 = 4,
        장마감 = 8,
        장종료_시간외종료 = 9,
        시간외_종가매매_시작 = 'a',
        시간외_종가매매_종료 = 'b',
        시간외_단일가_매매시작 = 'c',
        시간외_단일가_매매종료 = 'd',
        선옵_장마감전_동시호가_시작 = 's',
        선옵_장마감전_동시호가_종료 = 'e'
    }
}