using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog
{
    public struct Transmit
    {
        [DataMember, JsonProperty("C")]
        public string Code
        {
            get; set;
        }
        [DataMember, JsonProperty("T")]
        public string Time
        {
            get; set;
        }
        [DataMember, JsonProperty("D")]
        public string Datum
        {
            get; set;
        }
    }
}