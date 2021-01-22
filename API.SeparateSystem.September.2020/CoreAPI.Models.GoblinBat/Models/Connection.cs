using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class Connection
	{
		[Column(Order = 2)]
		public string Kiwoom
		{
			get; set;
		}
		[Column(Order = 3)]
		public string Account
		{
			get; set;
		}
		[Column(Order = 4)]
		public long Payment
		{
			get; set;
		}
		[Column(Order = 5)]
		public int Coupon
		{
			get; set;
		}
		[Column(Order = 1), EmailAddress]
		public string Email
		{
			get; set;
		}
	}
}