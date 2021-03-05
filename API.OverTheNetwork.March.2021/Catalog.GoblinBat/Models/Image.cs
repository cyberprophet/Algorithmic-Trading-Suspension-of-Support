using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.Models
{
	public struct Image
	{
		[DataMember, JsonProperty("status")]
		public string Status
		{
			get; set;
		}
		[DataMember, JsonProperty("url")]
		public string Url
		{
			get; set;
		}
		[DataMember, JsonProperty("replacer")]
		public string Replace
		{
			get; set;
		}
	}
}