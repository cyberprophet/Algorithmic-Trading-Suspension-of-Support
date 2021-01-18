using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ShareInvest
{
	public class CustomUserAccount : RemoteUserAccount
	{
		[JsonPropertyName("amr")]
		public string[] AuthenticationMethod
		{
			get; set;
		}
	}
}