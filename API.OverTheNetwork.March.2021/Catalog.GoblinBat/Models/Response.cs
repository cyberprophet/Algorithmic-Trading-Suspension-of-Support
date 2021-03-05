using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.Models
{
	public struct Response
	{
		[DataMember, JsonProperty("status")]
		public string Status
		{
			get; set;
		}
		[DataMember, JsonProperty("postId")]
		public string Post
		{
			get; set;
		}
		[DataMember, JsonProperty("url")]
		public string Url
		{
			get; set;
		}
	}
}