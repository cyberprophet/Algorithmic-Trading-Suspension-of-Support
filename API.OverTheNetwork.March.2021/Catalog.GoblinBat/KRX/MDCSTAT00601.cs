using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.KRX
{
	public struct MDCSTAT00601
    {
        [DataMember, JsonProperty("ISU_SRT_CD")]
        public string Code
        {
            get; set;
        }
        [DataMember, JsonProperty("ISU_ABBRV")]
        public string Name
        {
            get; set;
        }
        [DataMember, JsonProperty("TDD_CLSPRC")]
        public string Price
        {
            get; set;
        }
        [DataMember, JsonProperty("FLUC_TP_CD")]
        public string Compare
        {
            get; set;
        }
        [DataMember, JsonProperty("STR_CMP_PRC")]
        public string ComparePrice
        {
            get; set;
        }
        [DataMember, JsonProperty("FLUC_RT")]
        public string Rate
        {
            get; set;
        }
        [DataMember, JsonProperty("MKTCAP")]
        public string Capitalization
        {
            get; set;
        }
    }
}