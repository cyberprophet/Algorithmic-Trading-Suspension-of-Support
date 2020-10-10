using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.KRX
{
    public struct MKD99000001
    {
        [DataMember, JsonProperty("isu_cd")]
        public string Code
        {
            get; set;
        }
        [DataMember, JsonProperty("isu_nm")]
        public string Name
        {
            get; set;
        }
        [DataMember, JsonProperty("tdd_clsprc")]
        public string Price
        {
            get; set;
        }
        [DataMember, JsonProperty("fluc_tp_cd")]
        public string Compare
        {
            get; set;
        }
        [DataMember, JsonProperty("cmpprevdd_prc")]
        public string ComparePrice
        {
            get; set;
        }
        [DataMember, JsonProperty("updn_rate")]
        public string Rate
        {
            get; set;
        }
        [DataMember, JsonProperty("acc_trdval")]
        public string Transaction
        {
            get; set;
        }
        [DataMember, JsonProperty("mktcap")]
        public string Capitalization
        {
            get; set;
        }
    }
}