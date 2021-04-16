using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace ShareInvest.Catalog.Dart
{
	public class TiStory
	{
		[DataMember, JsonProperty("index")]
		public string Index
		{
			get; set;
		}
		[DataMember, JsonProperty("title")]
		public string Title
		{
			get; set;
		}
	}
}