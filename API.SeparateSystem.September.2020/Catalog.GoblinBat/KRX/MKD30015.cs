using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.KRX
{
    public struct MKD30015
    {
        [DataMember, JsonProperty("rn")]
        public string No
        {
            get; set;
        }
        [DataMember, JsonProperty("isu_cd")]
        public string Code
        {
            get; set;
        }
        [DataMember, JsonProperty("kor_shrt_isu_nm")]
        public string Name
        {
            get; set;
        }
        [DataMember, JsonProperty("isu_cur_pr")]
        public string Price
        {
            get; set;
        }
        [DataMember, JsonProperty("fluc_tp_cd")]
        public string CompareTo
        {
            get; set;
        }
        [DataMember, JsonProperty("prv_dd_cmpr")]
        public string PreviousDay
        {
            get; set;
        }
        [DataMember, JsonProperty("updn_rate")]
        public string Rate
        {
            get; set;
        }
        [DataMember, JsonProperty("isu_tr_vl")]
        public string Volume
        {
            get; set;
        }
        [DataMember, JsonProperty("isu_tr_amt")]
        public string Amount
        {
            get; set;
        }
        [DataMember, JsonProperty("opnprc")]
        public string Open
        {
            get; set;
        }
        [DataMember, JsonProperty("hgprc")]
        public string High
        {
            get; set;
        }
        [DataMember, JsonProperty("lwprc")]
        public string Low
        {
            get; set;
        }
        [DataMember, JsonProperty("cur_pr_tot_amt")]
        public string MarketCap
        {
            get; set;
        }
        [DataMember, JsonProperty("tot_amt_per")]
        public string ShareOfMarketCapitalization
        {
            get; set;
        }
        [DataMember, JsonProperty("lst_stk_vl")]
        public string ListedShares
        {
            get; set;
        }
        [DataMember, JsonProperty("f1")]
        public string ForeignOwnership
        {
            get; set;
        }
        [DataMember, JsonProperty("f2")]
        public string ShareOfForeignOwnership
        {
            get; set;
        }
        [DataMember, JsonProperty("totCnt")]
        public string TotalCnt
        {
            get; set;
        }
    }
}