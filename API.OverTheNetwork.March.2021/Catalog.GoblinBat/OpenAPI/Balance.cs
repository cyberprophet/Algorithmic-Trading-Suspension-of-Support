using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.OpenAPI
{
	public struct Balance
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
		[DataMember, JsonProperty("신용금액")]
		public string CreditAmount
		{
			get; set;
		}
		[DataMember, JsonProperty("신용이자")]
		public string CreditInterest
		{
			get; set;
		}
		[DataMember, JsonProperty("만기일")]
		public string ExpirationDate
		{
			get; set;
		}
		[DataMember, JsonProperty("당일실현손익_유가")]
		public string RealizedOnTheDay
		{
			get; set;
		}
		[DataMember, JsonProperty("당일실현손익률_유가")]
		public string RealizedRateOnTheDay
		{
			get; set;
		}
		[DataMember, JsonProperty("당일실현손익_신용")]
		public string RealizedOnTheDayCredit
		{
			get; set;
		}
		[DataMember, JsonProperty("당일실현손익률_신용")]
		public string RealizedRateOnTheDayCredit
		{
			get; set;
		}
		[DataMember, JsonProperty("담보대출수량")]
		public string LoanQuantity
		{
			get; set;
		}
		[DataMember, JsonProperty("ExtraItem")]
		public string ExtraItem
		{
			get; set;
		}
	}
}