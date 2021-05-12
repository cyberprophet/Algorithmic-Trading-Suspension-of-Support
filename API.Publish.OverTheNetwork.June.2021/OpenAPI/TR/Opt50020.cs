using System;
using System.Collections.Generic;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
	class Opt50020 : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
	{
		const string id = "만기년월";
		const string code = "opt50020";
		const string name = "복수종목결제월별시세요청";
		readonly string[] multiple = { "현재가", "전일대비", "등락율", "대비기호", "시간가치", "내재가치", "내재변동성", "이론가", "괴리율", "미결제약정전일대비", "미결제약정", "누적거래대금", "누적거래량", "매수호가총잔량", "매도호가총잔량", "매수호가수량", "매수호가", "매도호가수량", "매도호가", "기준가", "시가", "고가", "저가", "기준가대비시가등락율", "기준가대비고가등락율", "기준가대비저가등락율", "지수환산", "예상체결가전일종가대비등락율", "예상체결가전일종가대비기호", "예상체결가전일종가대비", "예상체결가", "종목코드", "ATM구분", "행사가", "대칭구분", "풋_현재가", "풋_전일대비", "풋_등락율", "풋_대비기호", "풋_시간가치", "풋_내재가치", "풋_내재변동성", "풋_이론가", "풋_괴리율", "풋_미결제약정전일대비", "풋_미결제약정", "풋_누적거래대금", "풋_누적거래량", "풋_매수호가총잔량", "풋_매도호가총잔량", "풋_매수호가수량", "풋_매수호가", "풋_매도호가수량", "풋_매도호가", "풋_기준가", "풋_시가", "풋_고가", "풋_저가", "풋_기준가대비시가등락율", "풋_기준가대비고가등락율", "풋_기준가대비저가등락율", "풋_지수환산", "풋_예상체결가전일종가대비등락율", "풋_예상체결가전일종가대비기호", "풋_예상체결가전일종가대비", "풋_예상체결가", "풋_종목코드", "풋_ATM구분", "풋_행사가" };
		internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
		{
			var response = base.OnReceiveTrData(null, multiple, e);

			if (response.Item2 is Queue<string[]> && response.Item2.Count > 0)
				Send?.Invoke(this, new SendSecuritiesAPI(new Queue<string[]>()));
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
		public override event EventHandler<SendSecuritiesAPI> Send;
	}
}