using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
    class Opt50001 : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
    {
        public override event EventHandler<SendSecuritiesAPI> Send;
        internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            var temp = base.OnReceiveTrData(single, e.sRQName.Equals(name) ? multi : null, e);
            var code = e.sRQName.Split(';')[1];

            if (temp.Item1 != null)
                Send?.Invoke(this, new SendSecuritiesAPI(code, temp.Item1[0x48].Trim(), temp.Item1[0x3F].Trim()[2..], temp.Item1[0x33].Trim(), 0));

            while (temp.Item2?.Count > 0)
                foreach (var pop in temp.Item2.Dequeue())
                    Base.SendMessage(GetType(), e.sRQName, pop);
        }
        internal override void SendErrorMessage(short error) => Send?.Invoke(this, new SendSecuritiesAPI(error));
        protected internal override string LookupScreenNo
        {
            get
            {
                if (Count++ == 0x63)
                    Count = 0;

                return (0x1388 + (Count == 0 ? ++Screen : Screen)).ToString("D4");
            }
        }
        internal override string ID => id;
        internal override string Value
        {
            get; set;
        }
        internal override string RQName
        {
            set
            {
                Code = value;
            }
            get
            {
                return string.Concat(name, ';', Code);
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
        string Code
        {
            get; set;
        }
        const string name = "선옵현재가정보요청";
        const string code = "opt50001";
        const string id = "종목코드";
        readonly string[] single = { "매도호가5", "매도호가4", "매도호가3", "매도호가2", "매도호가1", "매수호가1", "매수호가2", "매수호가3", "매수호가4", "매수호가5", "매도건수5", "매도건수4", "매도건수3", "매도건수2", "매도건수1", "매도수량5", "매도수량4", "매도수량3", "매도수량2", "매도수량1", "매수수량1", "매수수량2", "매수수량3", "매수수량4", "매수수량5", "매수건수1", "매수건수2", "매수건수3", "매수건수4", "매수건수5", "매도호가총건수", "매도호가총잔량", "호가시간", "매수호가총잔량", "매수호가총건수", "매도수익율5", "매도수익율4", "매도수익율3", "매도수익율2", "매도수익율1", "매수수익율1", "매수수익율2", "매수수익율3", "매수수익율4", "매수수익율5", "현재가", "대비기호", "전일대비", "등락율", "거래량", "거래량대비", "기준가", "이론가", "이론베이시스", "괴리도", "괴리율", "시장베이시스", "누적거래대금", "상한가", "하한가", "CB상한가", "CB하한가", "대용가", "최종거래일", "잔존일수", "영업일기준잔존일", "상장중최고가", "상장중최고가대비율", "상장중최고가일", "상장중최저가", "상장중최저가대비율", "상장중최저가일", "종목명", "시가", "고가", "저가", "2차저항", "1차저항", "피봇", "1차저지", "2차저지", "미결제약정", "미결제약정전일대비", "순매수잔량", "매도호가총잔량직전대비", "매수호가총잔량직전대비", "예상체결가", "예상체결가전일종가대비기호", "예상체결가전일종가대비", "예상체결가전일종가대비등락율", "이자율" };
        readonly string[] multi = { "체결시간", "현재가n", "대비기호n", "전일대비n", "체결량", "미결제약정n", "코스피200", "시장베이시스n", "역사적변동성", "표면이자", "배당액지수", "기준가n", "등락율n", "누적거래량" };
    }
}