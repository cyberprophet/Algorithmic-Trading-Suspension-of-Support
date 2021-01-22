using System;

namespace ShareInvest.Catalog.Models
{
	public struct UserInformation
	{
		public string Key
		{
			get; set;
		}
		public string Check
		{
			get; set;
		}
		public string Name
		{
			get; set;
		}
		public string[] Account
		{
			get; set;
		}
		public DateTime Remaining
		{
			get; set;
		}
	}
}