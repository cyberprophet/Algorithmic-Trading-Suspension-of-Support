using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.OpenAPI
{
	public struct Conclusion
	{
		[DataMember, JsonProperty("계좌번호")]
		public string Account
		{
			get; set;
		}
		[DataMember, JsonProperty("주문번호")]
		public string OrderNumber
		{
			get; set;
		}
		[DataMember, JsonProperty("관리자사번")]
		public string AdminNumber
		{
			get; set;
		}
		[DataMember, JsonProperty("종목코드_업종코드")]
		public string Code
		{
			get; set;
		}
		[DataMember, JsonProperty("주문업무분류")]
		public string OrderBusinessClassification
		{
			get; set;
		}
		[DataMember, JsonProperty("주문상태")]
		public string OrderState
		{
			get; set;
		}
		[DataMember, JsonProperty("종목명")]
		public string Name
		{
			get; set;
		}
		[DataMember, JsonProperty("주문수량")]
		public string OrderQuantity
		{
			get; set;
		}
		[DataMember, JsonProperty("주문가격")]
		public string OrderPrice
		{
			get; set;
		}
		[DataMember, JsonProperty("미체결수량")]
		public string UnsettledQuantity
		{
			get; set;
		}
		[DataMember, JsonProperty("체결누계금액")]
		public string TotalExecutionAmount
		{
			get; set;
		}
		[DataMember, JsonProperty("원주문번호")]
		public string OriginalOrderNumber
		{
			get; set;
		}
		[DataMember, JsonProperty("주문구분")]
		public string OrderClassification
		{
			get; set;
		}
		[DataMember, JsonProperty("매매구분")]
		public string SalesClassification
		{
			get; set;
		}
		[DataMember, JsonProperty("매도수구분")]
		public string TradingClassification
		{
			get; set;
		}
		[DataMember, JsonProperty("주문_체결시간")]
		public string Time
		{
			get; set;
		}
		[DataMember, JsonProperty("체결번호")]
		public string ConclusionNumber
		{
			get; set;
		}
		[DataMember, JsonProperty("체결가")]
		public string ConclusionPrice
		{
			get; set;
		}
		[DataMember, JsonProperty("체결량")]
		public string ConclusionQuantity
		{
			get; set;
		}
		[DataMember, JsonProperty("현재가")]
		public string CurrentPrice
		{
			get; set;
		}
		[DataMember, JsonProperty("매도호가")]
		public string OfferPrice
		{
			get; set;
		}
		[DataMember, JsonProperty("매수호가")]
		public string BidPrice
		{
			get; set;
		}
		[DataMember, JsonProperty("단위체결가")]
		public string UnitConclusionPrice
		{
			get; set;
		}
		[DataMember, JsonProperty("단위체결량")]
		public string UnitConclusionQuantity
		{
			get; set;
		}
		[DataMember, JsonProperty("당일매매수수료")]
		public string Commission
		{
			get; set;
		}
		[DataMember, JsonProperty("당일매매세금")]
		public string Tax
		{
			get; set;
		}
		[DataMember, JsonProperty("거부사유")]
		public string ReasonForRejection
		{
			get; set;
		}
		[DataMember, JsonProperty("화면번호")]
		public string ScreenNumber
		{
			get; set;
		}
		[DataMember, JsonProperty("터미널번호")]
		public string TerminalNumber
		{
			get; set;
		}
		[DataMember, JsonProperty("신용구분")]
		public string CreditClassification
		{
			get; set;
		}
		[DataMember, JsonProperty("대출일")]
		public string LoanDate
		{
			get; set;
		}
	}	
}