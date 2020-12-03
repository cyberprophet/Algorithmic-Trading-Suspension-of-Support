using System;

namespace ShareInvest.Catalog.Models
{
	public struct Log
	{
		public DateTime Time
		{
			get; set;
		}
		public string Message
		{
			get; set;
		}
		public string Code
		{
			get; set;
		}
		public string Screen
		{
			get; set;
		}
	}
}