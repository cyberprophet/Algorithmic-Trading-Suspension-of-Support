using Newtonsoft.Json;

using System.Runtime.Serialization;

namespace ShareInvest.Catalog.OpenAPI
{
	public struct OPW00004
	{
		[DataMember, JsonProperty("account")]
		public string Account
		{
			get; set;
		}
		[DataMember, JsonProperty("종목코드")]
		public string Code
		{
			get; set;
		}
		[DataMember, JsonProperty("종목명")]
		public string Name
		{
			get; set;
		}
		[DataMember, JsonProperty("보유수량")]
		public string Quantity
		{
			get; set;
		}
		[DataMember, JsonProperty("평균단가")]
		public string Average
		{
			get; set;
		}
		[DataMember, JsonProperty("현재가")]
		public string Current
		{
			get; set;
		}
		[DataMember, JsonProperty("평가금액")]
		public string Evaluation
		{
			get; set;
		}
		[DataMember, JsonProperty("손익금액")]
		public string Amount
		{
			get; set;
		}
		[DataMember, JsonProperty("손익율")]
		public string Rate
		{
			get; set;
		}
		[DataMember, JsonProperty("대출일")]
		public string Loan
		{
			get; set;
		}
		[DataMember, JsonProperty("매입금액")]
		public string Purchase
		{
			get; set;
		}
		[DataMember, JsonProperty("결제잔고")]
		public string Balance
		{
			get; set;
		}
		[DataMember, JsonProperty("전일매수수량")]
		public string PreviousPurchaseQuantity
		{
			get; set;
		}
		[DataMember, JsonProperty("전일매도수량")]
		public string PreviousSalesQuantity
		{
			get; set;
		}
		[DataMember, JsonProperty("금일매수수량")]
		public string PurchaseQuantity
		{
			get; set;
		}
		[DataMember, JsonProperty("금일매도수량")]
		public string SalesQuantity
		{
			get; set;
		}
	}
}