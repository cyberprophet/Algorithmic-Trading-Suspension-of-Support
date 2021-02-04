using System.Collections.Generic;

namespace ShareInvest.Catalog.Models
{
	public class User
	{
		public Account Account
		{
			get; set;
		}
		public Queue<Log> Logs
		{
			get; set;
		}
		public Dictionary<string, Balance> Balance
		{
			get; set;
		}
	}
}