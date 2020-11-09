using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
    class OPTKWFID : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
    {
        public override event EventHandler<SendSecuritiesAPI> Send;
        internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            var param = base.OnReceiveTrData(null, multi, e).Item2;

            while (param.Count > 0)
            {
                var str = param.Dequeue();
                var code = str[0].Trim();
                Send?.Invoke(this, new SendSecuritiesAPI(code, str[1].Trim(), API.GetMasterStockState(str[0].Trim()), str[3].Trim(), API.KOA_Functions(info, code).Split(';')[0].Contains(market) ? 1 : 2));
            }
        }
        internal override void SendErrorMessage(short error) => Send?.Invoke(this, new SendSecuritiesAPI(error));
        protected internal override string LookupScreenNo => (Count++ + 0x1B58).ToString("D4");
        internal override string ID
        {
            get;
        }
        internal override string Value
        {
            get; set;
        }
        internal override string RQName
        {
            set
            {

            }
            get => name;
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
        readonly string[] multi = { "종목코드", "종목명", "현재가", "기준가", "전일대비", "전일대비기호", "등락율", "거래량", "거래대금", "체결량", "체결강도", "전일거래량대비", "매도호가", "매수호가", "매도1차호가", "매도2차호가", "매도3차호가", "매도4차호가", "매도5차호가", "매수1차호가", "매수2차호가", "매수3차호가", "매수4차호가", "매수5차호가", "상한가", "하한가", "시가", "고가", "저가", "종가", "체결시간", "예상체결가", "예상체결량", "자본금", "액면가", "시가총액", "주식수", "호가시간", "일자", "우선매도잔량", "우선매수잔량", "우선매도건수", "우선매수건수", "총매도잔량", "총매수잔량", "총매도건수", "총매수건수", "패리티", "기어링", "손익분기", "자본지지", "ELW행사가", "전환비율", "ELW만가일", "미결제약정", "미결제전일대비", "이론가", "내재변동성", "델타", "감마", "쎄타", "베가", "로" };
        const string name = "관심종목정보요청";
        const string code = "OPTKWFID";
        const string market = "거래소";
        const string info = "GetMasterStockInfo";
    }
}