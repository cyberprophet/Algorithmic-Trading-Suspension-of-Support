using System;
using System.Collections.Generic;
using System.Linq;

using AxKHOpenAPILib;

using ShareInvest.Catalog.OpenAPI;
using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
    class Opt10081 : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
    {
        protected internal override (string[], Queue<string[]>) OnReceiveTrData(string[] single, string[] multi, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            var sTemp = single != null ? new string[single.Length] : null;

            if (single != null)
                for (int i = 0; i < single.Length; i++)
                    sTemp[i] = API.GetCommData(e.sTrCode, e.sRQName, 0, single[i]).Trim();

            if (multi != null)
            {
                var temp = API.GetCommDataEx(e.sTrCode, e.sRQName);

                if (temp != null)
                {
                    int x, y, lx = ((object[,])temp).GetUpperBound(0), ly = ((object[,])temp).GetUpperBound(1);
                    var catalog = new Queue<string[]>();

                    for (x = 0; x <= lx; x++)
                    {
                        var str = new string[ly + 1];

                        for (y = 0; y <= ly; y++)
                            str[y] = ((string)((object[,])temp)[x, y]).Trim();

                        if (string.IsNullOrEmpty(e.sRQName) == false && (e.sRQName.Equals(rqName) || string.Compare(str[4], e.sRQName) < 0))
                            catalog.Enqueue(str);
                    }
                    return (sTemp, catalog);
                }
            }
            return (sTemp, null);
        }
        internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (int.TryParse(e.sPrevNext, out int next))
            {
                var temp = OnReceiveTrData(opSingle, opMutiple, e);
                var tr = Connect.TR.First(o => o.GetType().Name.Substring(1).Equals(e.sTrCode.Substring(1)) && o.RQName.Equals(e.sRQName));

                while (temp.Item2?.Count > 0)
                {
                    var param = temp.Item2.Dequeue();
                    storage.Push(new Days
                    {
                        Code = temp.Item1[0],
                        Date = param[4],
                        Price = param[1]
                    });
                    if (string.IsNullOrEmpty(param[8]) == false && param[9].Equals("0.00") == false)
                        SendMessage(API.GetMasterCodeName(temp.Item1[0]), string.Concat(param[4], ';', param[8], ';', param[9], ';', param[param.Length - 2]));
                }
                if (next > 0)
                {
                    tr.PrevNext = next;
                    SendMessage(e.sScrNo, e.sRQName);
                    Connect.GetInstance(API).InputValueRqData(tr);
                }
                else
                    Send?.Invoke(this, new SendSecuritiesAPI(storage));
            }
        }
        internal override string ID => id;
        internal override string Value
        {
            get => string.Concat(Code, ";1");
            set => Code = value;
        }
        internal override string RQName
        {
            get; set;
        } = rqName;
        internal override string TrCode => code;
        internal override int PrevNext
        {
            get; set;
        }
        internal override string ScreenNo => LookupScreenNo;
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        readonly Stack<Days> storage = new Stack<Days>();
        readonly string[] opSingle = { "종목코드" };
        readonly string[] opMutiple = { "종목코드", "현재가", "거래량", "거래대금", "일자", "시가", "고가", "저가", "수정주가구분", "수정비율", "대업종구분", "소업종구분", "종목정보", "수정주가이벤트", "전일종가" };
        const string code = "opt10081";
        const string id = "종목코드;기준일자;수정주가구분";
        const string rqName = "주식일봉차트조회요청";
        string Code
        {
            get; set;
        }
        public override event EventHandler<SendSecuritiesAPI> Send;
    }
}