using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class Contents
	{
		[Column(Order = 1), MinLength(6), MaxLength(8)]
		public string Code
		{
			get; set;
		}
		[Column(Order = 2), StringLength(8)]
		public string Date
		{
			get; set;
		}
		[Required]
		public string CompressedContents
		{
			get; set;
		}
	}
}