using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class StockTags
	{
		[ForeignKey("Codes"), Column(Order = 1), MinLength(6), MaxLength(8)]
		public string Code
		{
			get; set;
		}
		public string ID
		{
			get; set;
		}
		public string Tags
		{
			get; set;
		}
		public string Size
		{
			get; set;
		}
	}
}