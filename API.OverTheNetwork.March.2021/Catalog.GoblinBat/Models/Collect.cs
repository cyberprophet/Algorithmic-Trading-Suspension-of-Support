using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.Models
{
    public struct Collect
    {
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