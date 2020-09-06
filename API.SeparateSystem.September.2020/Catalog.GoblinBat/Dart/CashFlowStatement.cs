using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.Dart
{
    public struct CashFlowStatement
    {
        [DataMember, JsonProperty("영업활동현금흐름")]
        public long OperatingActivities
        {
            get; set;
        }
        [DataMember, JsonProperty("영업에서창출된현금흐름")]
        public long CashFlow
        {
            get; set;
        }
        [DataMember, JsonProperty("당기순이익")]
        public long NetIncome
        {
            get; set;
        }
        [DataMember, JsonProperty("조정")]
        public long Adjustments
        {
            get; set;
        }
        [DataMember, JsonProperty("영업활동으로인한자산부채의변동")]
        public long NetChangeInWorkingCapital
        {
            get; set;
        }
        [DataMember, JsonProperty("이자의수취")]
        public long InterestIncome
        {
            get; set;
        }
        [DataMember, JsonProperty("이자의지급")]
        public long InterestExpense
        {
            get; set;
        }
        [DataMember, JsonProperty("배당금수입")]
        public long DividendIncome
        {
            get; set;
        }
        [DataMember, JsonProperty("법인세납부액")]
        public long IncomeTaxExpense
        {
            get; set;
        }
        [DataMember, JsonProperty("투자활동현금흐름")]
        public long InvestingActivities
        {
            get; set;
        }
        [DataMember, JsonProperty("유형자산의처분")]
        public long DepreciationAndImpairmentOfPropertyAndEquipment
        {
            get; set;
        }
        [DataMember, JsonProperty("유형자산의취득")]
        public long PurchasesOfPropertyAndEquipment
        {
            get; set;
        }
        [DataMember, JsonProperty("무형자산의처분")]
        public long AmortizationAndImpairmentOfIntangibleAssets
        {
            get; set;
        }
        [DataMember, JsonProperty("무형자산의취득")]
        public long AcquisitionOfIntangibleAssets
        {
            get; set;
        }
        [DataMember, JsonProperty("재무활동현금흐름")]
        public long FinancingActivities
        {
            get; set;
        }
        [DataMember, JsonProperty("배당금지급")]
        public long StockBasedCompensationExpense
        {
            get; set;
        }
        [DataMember, JsonProperty("자기주식의취득")]
        public long PurchasesOfMarketableSecurities
        {
            get; set;
        }
        [DataMember, JsonProperty("현금및현금성자산의순증가(감소)")]
        public long NetIncreaseInCashAndCashEquivalents
        {
            get; set;
        }
        [DataMember, JsonProperty("기초현금및현금성자산")]
        public long CashAndCashEquivalentsAtBeginningOfPeriod
        {
            get; set;
        }
        [DataMember, JsonProperty("기말현금및현금성자산")]
        public long CashAndCashEquivalentsAtEndOfPeriod
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
    }
}