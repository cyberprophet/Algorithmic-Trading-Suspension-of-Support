using System;
using System.Collections.Generic;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
	class OPT50021 : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
	{
		const string id = "만기년월";
		const string code = nameof(OPT50021);
		const string name = "콜종목결제월별시세요청";
		readonly string[] multiple = { "지수환산", "ATM구분", "종목코드", "행사가", "현재가", "대비기호", "전일대비", "등락율", "시가", "기준가대비시가등락율", "고가", "기준가대비고가등락율", "저가", "기준가대비저가등락율", "기준가", "매도호가", "매도호가수량", "매수호가", "매수호가수량", "매도호가총잔량", "매수호가총잔량", "누적거래량", "누적거래대금", "미결제약정", "미결제약정전일대비", "괴리율", "이론가", "내재변동성", "내재가치", "시간가치", "델타", "감마", "세타", "베가", "로" };
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