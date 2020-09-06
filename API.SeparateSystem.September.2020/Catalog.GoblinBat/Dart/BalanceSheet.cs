using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.Dart
{
    public struct BalanceSheet
    {
        [DataMember, JsonProperty("지배기업소유주지분")]
        public long OwnersEquity
        {
            get; set;
        }
        [DataMember, JsonProperty("지배기업의소유주에게귀속되는자본")]
        public long OwnerEquity
        {
            set => OwnersEquity = value;
        }
        [DataMember, JsonProperty("보통주자본금")]
        public long CommonStock
        {
            get; set;
        }
        [DataMember, JsonProperty("우선주자본금")]
        public long PreferredStock
        {
            get; set;
        }
        [DataMember, JsonProperty("자본금")]
        public long EquityCapital
        {
            get; set;
        }
        [DataMember, JsonProperty("자본잉여금")]
        public long CapitalSurplus
        {
            get; set;
        }
        [DataMember, JsonProperty("주식발행초과금")]
        public long StockIssuanceExcess
        {
            set => CapitalSurplus += value;
        }
        [DataMember, JsonProperty("기타자본항목")]
        public long OtherCapitalSurplus
        {
            set => OtherComponentsOfCapital = value;
        }
        [DataMember, JsonProperty("기타자본구성요소")]
        public long OtherComponentsOfCapital
        {
            set; get;
        }
        [DataMember, JsonProperty("이익잉여금(결손금)")]
        public long EarnedSurplus
        {
            get; set;
        }
        [DataMember, JsonProperty("매입채무")]
        public long AccountPayable
        {
            get; set;
        }
        [DataMember, JsonProperty("미지급금")]
        public long AccruedCompensationAndBenefits
        {
            get; set;
        }
        [DataMember, JsonProperty("미지급비용")]
        public long AccruedExpensesAndOtherCurrentLiabilities
        {
            get; set;
        }
        [DataMember, JsonProperty("이연수익")]
        public long DeferredRevenue
        {
            get; set;
        }
        [DataMember, JsonProperty("선수금")]
        public long Advance
        {
            get; set;
        }
        [DataMember, JsonProperty("미지급법인세")]
        public long IncomeTaxesPayble
        {
            get; set;
        }
        [DataMember, JsonProperty("장기차입금")]
        public long LongTermDebt
        {
            get; set;
        }
        [DataMember, JsonProperty("유동성이연수익")]
        public long CurrentDeferredRevenue
        {
            get; set;
        }
        [DataMember, JsonProperty("이연법인세부채")]
        public long DeferredIncomeTaxesLiabilities
        {
            get; set;
        }
        [DataMember, JsonProperty("금용리스부채")]
        public long OperatingLeaseLiabilities
        {
            get; set;
        }
        [DataMember, JsonProperty("기타유동부채")]
        public long OtherCurrentLiablities
        {
            get; set;
        }
        [DataMember, JsonProperty("기타비유동부채")]
        public long OtherNonCurrentLiablities
        {
            get; set;
        }
        [DataMember, JsonProperty("이연법인세자산")]
        public long DeferredIncomeTaxesAssets
        {
            get; set;
        }
        [DataMember, JsonProperty]
        public string Code
        {
            get; set;
        }
        [DataMember, JsonProperty]
        public string Date
        {
            get; set;
        }
        [DataMember, JsonProperty("현금및현금성자산")]
        public long CashAndCashEquivalents
        {
            get; set;
        }
        [DataMember, JsonProperty("단기금융상품")]
        public long MarketableSecurities
        {
            get; set;
        }
        [DataMember, JsonProperty("매출채권")]
        public long AccountsReceivable
        {
            get; set;
        }
        [DataMember, JsonProperty("재고자산")]
        public long Inventory
        {
            get; set;
        }
        [DataMember, JsonProperty("선급비용")]
        public long PrepaidExpenses
        {
            get; set;
        }
        [DataMember, JsonProperty("선급금")]
        public long AdvancedPayments
        {
            get; set;
        }
        [DataMember, JsonProperty("기타유동자산")]
        public long OtherCurrentAssets
        {
            get; set;
        }
        [DataMember, JsonProperty("유형자산")]
        public long PropertyAndEquipment
        {
            get; set;
        }
        [DataMember, JsonProperty("무형자산")]
        public long IntangibleAssets
        {
            get; set;
        }
        [DataMember, JsonProperty("기타비유동자산")]
        public long OtherNonCurrentAssets
        {
            get; set;
        }
        [DataMember, JsonProperty("자산총계")]
        public long TotalAssets
        {
            get; set;
        }
        [DataMember, JsonProperty("유동자산")]
        public long CurrentAssets
        {
            get; set;
        }
        [DataMember, JsonProperty("비유동자산")]
        public long NonMarketableInvestments
        {
            get; set;
        }
        [DataMember, JsonProperty("부채총계")]
        public long TotalLiabilites
        {
            get; set;
        }
        [DataMember, JsonProperty("유동부채")]
        public long CurrentLiabilities
        {
            get; set;
        }
        [DataMember, JsonProperty("비유동부채")]
        public long NonCurrentLiabilities
        {
            get; set;
        }
        [DataMember, JsonProperty("자본총계")]
        public long TotalEquity
        {
            get; set;
        }
    }
}