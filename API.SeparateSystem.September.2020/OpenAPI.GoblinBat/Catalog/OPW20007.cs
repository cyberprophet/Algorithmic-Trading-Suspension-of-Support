using System;

using AxKHOpenAPILib;

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

                for (int i = 0; i < futures.Length; i++)
                    SendMessage(multiple[i], futures[i]);
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