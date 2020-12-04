using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.OpenAPI
{
	public struct Derivatives
	{
		[DataMember, JsonProperty("계좌번호")]
		public string Account
		{
			get; set;
		}
		[DataMember, JsonProperty("종목코드_업종코드")]
		public string Code
		{
			get; set;
		}
		[DataMember, JsonProperty("종목명")]
		public string Name
		{
			get; set;
		}
		[DataMember, JsonProperty("현재가")]
		public string Current
		{
			get; set;
		}
		[DataMember, JsonProperty("보유수량")]
		public string Quantity
		{
			get; set;
		}
		[DataMember, JsonProperty("매입단가")]
		public string Purchase
		{
			get; set;
		}
		[DataMember, JsonProperty("총매입가")]
		public string TotalPurchasePrice
		{
			get; set;
		}
		[DataMember, JsonProperty("주문가능수량")]
		public string QuantityAvailable
		{
			get; set;
		}
		[DataMember, JsonProperty("당일순매수량")]
		public string NetPurchaseOnTheDay
		{
			get; set;
		}
		[DataMember, JsonProperty("매도_매수구분")]
		public string TradingClassification
		{
			get; set;
		}
		[DataMember, JsonProperty("당일총매도손익")]
		public string TotalSalesOnTheDay
		{
			get; set;
		}
		[DataMember, JsonProperty("예수금")]
		public string Deposit
		{
			get; set;
		}
		[DataMember, JsonProperty("매도호가")]
		public string Offer
		{
			get; set;
		}
		[DataMember, JsonProperty("매수호가")]
		public string Bid
		{
			get; set;
		}
		[DataMember, JsonProperty("기준가")]
		public string ReferencePrice
		{
			get; set;
		}
		[DataMember, JsonProperty("손익율")]
		public string Rate
		{
			get; set;
		}
		[DataMember, JsonProperty("파생상품거래단위")]
		public string Unit
		{
			get; set;
		}
		[DataMember, JsonProperty("상한가")]
		public string Upper
		{
			get; set;
		}
		[DataMember, JsonProperty("하한가")]
		public string Lower
		{
			get; set;
		}
	}
}