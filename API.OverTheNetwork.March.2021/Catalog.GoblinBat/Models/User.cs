using System.Collections.Generic;
using System.Net.WebSockets;

namespace ShareInvest.Catalog.Models
{
	public class User
	{
		public string Message
		{
			get; set;
		}
		public Account Account
		{
			get; set;
		}
		public Queue<Log> Logs
		{
			get; set;
		}
		public WebSocket Socket
		{
			get; set;
		}
		public Dictionary<string, Balance> Balance
		{
			get; set;
		}
	}
}