using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.Dart
{
    public struct FinancialStatement
    {
        [DataMember, JsonProperty("종목코드")]
        public string Code
        {
            get; set;
        }
        [DataMember, JsonProperty("연간")]
        public string Date
        {
            get; set;
        }
        [DataMember, JsonProperty("매출액")]
        public string Revenues
        {
            get; set;
        }
        [DataMember, JsonProperty("영업이익")]
        public string IncomeFromOperations
        {
            get; set;
        }
        [DataMember, JsonProperty("영업이익(발표기준)")]
        public string IncomeFromOperation
        {
            get; set;
        }
        [DataMember, JsonProperty("세전계속사업이익")]
        public string ProfitFromContinuingOperations
        {
            get; set;
        }
        [DataMember, JsonProperty("당기순이익")]
        public string NetIncome
        {
            get; set;
        }
        [DataMember, JsonProperty("당기순이익(지배)")]
        public string ControllingNetIncome
        {
            get; set;
        }
        [DataMember, JsonProperty("당기순이익(비지배)")]
        public string NonControllingNetIncome
        {
            get; set;
        }
        [DataMember, JsonProperty("자산총계")]
        public string TotalAssets
        {
            get; set;
        }
        [DataMember, JsonProperty("부채총계")]
        public string TotalLiabilites
        {
            get; set;
        }
        [DataMember, JsonProperty("자본총계")]
        public string TotalEquity
        {
            get; set;
        }
        [DataMember, JsonProperty("자본총계(지배)")]
        public string ControllingEquity
        {
            get; set;
        }
        [DataMember, JsonProperty("자본총계(비지배)")]
        public string NonControllingEquity
        {
            get; set;
        }
        [DataMember, JsonProperty("자본금")]
        public string EquityCapital
        {
            get; set;
        }
        [DataMember, JsonProperty("영업활동현금흐름")]
        public string OperatingActivities
        {
            get; set;
        }
        [DataMember, JsonProperty("투자활동현금흐름")]
        public string InvestingActivities
        {
            get; set;
        }
        [DataMember, JsonProperty("재무활동현금흐름")]
        public string FinancingActivities
        {
            get; set;
        }
        [DataMember, JsonProperty("CAPEX")]
        public string CAPEX
        {
            get; set;
        }
        [DataMember, JsonProperty("FCF")]
        public string FCF
        {
            get; set;
        }
        [DataMember, JsonProperty("이자발생부채")]
        public string InterestAccruingLiabilities
        {
            get; set;
        }
        [DataMember, JsonProperty("영업이익률")]
        public string OperatingMargin
        {
            get; set;
        }
        [DataMember, JsonProperty("순이익률")]
        public string NetMargin
        {
            get; set;
        }
        [DataMember, JsonProperty("ROE(%)")]
        public string ROE
        {
            get; set;
        }
        [DataMember, JsonProperty("ROA(%)")]
        public string ROA
        {
            get; set;
        }
        [DataMember, JsonProperty("부채비율")]
        public string DebtRatio
        {
            get; set;
        }
        [DataMember, JsonProperty("자본유보율")]
        public string RetentionRatio
        {
            get; set;
        }
        [DataMember, JsonProperty("EPS(원)")]
        public string EPS
        {
            get; set;
        }
        [DataMember, JsonProperty("PER(배)")]
        public string PER
        {
            get; set;
        }
        [DataMember, JsonProperty("BPS(원)")]
        public string BPS
        {
            get; set;
        }
        [DataMember, JsonProperty("PBR(배)")]
        public string PBR
        {
            get; set;
        }
        [DataMember, JsonProperty("현금DPS(원)")]
        public string DPS
        {
            get; set;
        }
        [DataMember, JsonProperty("현금배당수익률")]
        public string DividendYield
        {
            get; set;
        }
        [DataMember, JsonProperty("현금배당성향(%)")]
        public string PayoutRatio
        {
            get; set;
        }
    }
}