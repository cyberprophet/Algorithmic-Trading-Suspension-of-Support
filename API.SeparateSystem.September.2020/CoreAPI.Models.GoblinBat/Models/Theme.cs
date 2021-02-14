using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class Theme
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None), Key]
		public string Index
		{
			get; set;
		}
		[Required]
		public string Name
		{
			get; set;
		}
		public double Rate
		{
			get; set;
		}
		public double Average
		{
			get; set;
		}
		[MaxLength(0xD), Required]
		public string Code
		{
			get; set;
		}
		[StringLength(6), Required]
		public string Date
		{
			get; set;
		}
	}
}