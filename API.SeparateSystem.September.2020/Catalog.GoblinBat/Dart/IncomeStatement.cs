using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.Dart
{
    public struct IncomeStatement
    {
        [DataMember, JsonProperty("수익(매출액)")]
        public long Revenues
        {
            get; set;
        }
        [DataMember, JsonProperty("매출원가")]
        public long CostOfSales
        {
            get; set;
        }
        [DataMember, JsonProperty("매출총이익")]
        public long GrossProfit
        {
            get; set;
        }
        [DataMember, JsonProperty("판매비와관리비")]
        public long CostsAndExpenses
        {
            get; set;
        }
        [DataMember, JsonProperty("영업이익")]
        public long IncomeFromOperations
        {
            get; set;
        }
        [DataMember, JsonProperty("기타수익")]
        public long OtherIncome
        {
            get; set;
        }
        [DataMember, JsonProperty("기타비용")]
        public long OtherExpense
        {
            get; set;
        }
        [DataMember, JsonProperty("지분법이익")]
        public long EquityMethodGains
        {
            get; set;
        }
        [DataMember, JsonProperty("금융수익")]
        public long FinancialIncome
        {
            get; set;
        }
        [DataMember, JsonProperty("금융원가")]
        public long FinancialCost
        {
            get; set;
        }
        [DataMember, JsonProperty("법인세비용차감전순이익(손실)")]
        public long IncomeBeforeIncomeTaxes
        {
            get; set;
        }
        [DataMember, JsonProperty("법인세비용")]
        public long ProvisionForIncomeTaxes
        {
            get; set;
        }
        [DataMember, JsonProperty("당기순이익(손실)")]
        public long NetIncome
        {
            get; set;
        }
        [DataMember, JsonProperty("주당이익")]
        public long BasicEarningsPerShare
        {
            get; set;
        }
        [DataMember, JsonProperty("주당이익(손실)")]
        public long DilutedEarningsPerShare
        {
            set => BasicEarningsPerShare = value;
        }
        [DataMember, JsonProperty("매출")]
        public long Revenue
        {
            set => Revenues = value;
        }
        [DataMember, JsonProperty("영업이익(손실)")]
        public long IncomeFromOperation
        {
            set => IncomeFromOperations = value;
        }
        [DataMember, JsonProperty("기타이익")]
        public long OtherIncomes
        {
            set => OtherIncome = value;
        }
        [DataMember, JsonProperty("기타영업외수익")]
        public long OtherNonOperatingIncomes
        {
            set => OtherIncome += value;
        }
        [DataMember, JsonProperty("기타손실")]
        public long OtherExpenses
        {
            set => OtherExpense = value;
        }
        [DataMember, JsonProperty("기타영업외비용")]
        public long OtherNonOperatingExpense
        {
            set => OtherExpense += value;
        }
        [DataMember, JsonProperty("지분법이익(손실)")]
        public long EquityMethodGain
        {
            set => EquityMethodGains = value;
        }
        [DataMember, JsonProperty("금융비용")]
        public long FinancialExpenses
        {
            set => FinancialCost = value;
        }
        [DataMember, JsonProperty("법인세비용(수익)")]
        public long ProvisionForIncomeTax
        {
            set => ProvisionForIncomeTaxes = value;
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
    }
}