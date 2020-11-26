using System;

using AxKHOpenAPILib;

using ShareInvest.Analysis;
using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
    class OPW20007 : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
    {
        internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            var temp = base.OnReceiveTrData(single, multiple, e);

            if (temp.Item1 != null)
                for (int i = 0; i < temp.Item1.Length; i++)
                    SendMessage(single[i], temp.Item1[i]);

            while (temp.Item2?.Count > 0)
            {
                var futures = temp.Item2.Dequeue();

                if (int.TryParse(futures[4], out int purchase) && int.TryParse(futures[5], out int current))
                {
                    var param
                        = new SendSecuritiesAPI(new string[] { futures[0], futures[1].Split('F')[0].Trim().Replace("_", string.Empty), futures[2], string.Empty, futures[3], futures[0][1].Equals('0') ? futures[4].Insert(futures[4].Length - 2, ".") : futures[4].Remove(futures[4].Length - 2, 2), futures[0][1].Equals('0') ? futures[5].Insert(futures[5].Length - 2, ".") : futures[5].Remove(futures[5].Length - 2, 2), string.Empty, futures[6], ((current / (double)purchase - 1) * 0x64).ToString("F5") });

                    if (param.Convey is Tuple<string, string, int, dynamic, dynamic, long, double> balance
                        && Connect.HoldingStock.TryGetValue(balance.Item1, out Holding hs))
                    {
                        hs.Code = balance.Item1;
                        hs.Quantity = balance.Item3;
                        hs.Purchase = futures[0][1].Equals('0') ? (double)balance.Item4 : (int)balance.Item4;
                        hs.Current = futures[0][1].Equals('0') ? (double)balance.Item5 : (int)balance.Item5;
                        hs.Revenue = balance.Item6;
                        hs.Rate = balance.Item3 > 0 ? balance.Item7 : -balance.Item7;
                        Connect.HoldingStock[balance.Item1] = hs;
                    }
                    Send?.Invoke(this, param);
                }
            }
        }
        internal override string ID => id;
        internal override string Value
        {
            get; set;
        }
        internal override string RQName
        {
            get => name;
            set
            {

            }
        }
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
        const string id = "계좌번호;비밀번호;비밀번호입력매체구분";
        const string name = "선옵잔고현황정산가기준요청";
        const string code = "OPW20007";
        readonly string[] single = { "약정금액합계", "평가손익합계", "출력건수" };
        readonly string[] multiple = { "종목코드", "종목명", "매도매수구분", "수량", "매입단가", "현재가", "평가손익", "청산가능수량", "약정금액", "평가금액" };
        public override event EventHandler<SendSecuritiesAPI> Send;
    }
}