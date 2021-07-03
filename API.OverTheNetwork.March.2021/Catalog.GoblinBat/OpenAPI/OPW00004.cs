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
			get => code; set => code = value[0].Equals('A') ? value[1..] : value;
		}
		[DataMember, JsonProperty("종목명")]
		public string Name
		{
			get; set;
		}
		[DataMember, JsonProperty("보유수량")]
		public int Quantity
		{
			get; set;
		}
		[DataMember, JsonProperty("평균단가")]
		public int Average
		{
			get; set;
		}
		[DataMember, JsonProperty("현재가")]
		public int Current
		{
			get; set;
		}
		[DataMember, JsonProperty("평가금액")]
		public long Evaluation
		{
			get; set;
		}
		[DataMember, JsonProperty("손익금액")]
		public long Amount
		{
			get; set;
		}
		[DataMember, JsonProperty("손익율")]
		public double Rate
		{
			get; set;
		}
		[DataMember, JsonProperty("대출일")]
		public string Loan
		{
			get; set;
		}
		[DataMember, JsonProperty("매입금액")]
		public long Purchase
		{
			get; set;
		}
		[DataMember, JsonProperty("결제잔고")]
		public long Balance
		{
			get; set;
		}
		[DataMember, JsonProperty("전일매수수량")]
		public int PreviousPurchaseQuantity
		{
			get; set;
		}
		[DataMember, JsonProperty("전일매도수량")]
		public int PreviousSalesQuantity
		{
			get; set;
		}
		[DataMember, JsonProperty("금일매수수량")]
		public int PurchaseQuantity
		{
			get; set;
		}
		[DataMember, JsonProperty("금일매도수량")]
		public int SalesQuantity
		{
			get; set;
		}
		string code;
	}
}