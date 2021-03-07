using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class Response
	{
		[StringLength(6), Column(Order = 1), Key, ForeignKey("Group")]
		public string Code
		{
			get; set;
		}
		[Column(Order = 2)]
		public int Tistory
		{
			get; set;
		}
	}
}