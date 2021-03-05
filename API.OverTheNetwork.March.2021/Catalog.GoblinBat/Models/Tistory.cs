using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.Models
{
	public struct Tistory
	{
		[DataMember, JsonProperty("access_token")]
		public string Token
		{
			get; set;
		}
		[DataMember, JsonProperty("output")]
		public string Type
		{
			get; set;
		}
		[DataMember, JsonProperty("blogName")]
		public string Name
		{
			get; set;
		}
		[DataMember, JsonProperty("title")]
		public string Title
		{
			get; set;
		}
		[DataMember, JsonProperty("content")]
		public string Content
		{
			get; set;
		}
		[DataMember, JsonProperty("visibility")]
		public string Visibility
		{
			get; set;
		}
		[DataMember, JsonProperty("category")]
		public string Category
		{
			get; set;
		}
		[DataMember, JsonProperty("published")]
		public string Publish
		{
			get; set;
		}
		[DataMember, JsonProperty("slogan")]
		public string Slogan
		{
			get; set;
		}
		[DataMember, JsonProperty("tag")]
		public string Tag
		{
			get; set;
		}
		[DataMember, JsonProperty("acceptComment")]
		public string Comment
		{
			get; set;
		}
		[DataMember, JsonProperty("password")]
		public string Password
		{
			get; set;
		}
	}
}