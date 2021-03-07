using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class Group
	{
		[StringLength(6), Column(Order = 1), Key]
		public string Code
		{
			get; set;
		}
		[Required]
		public string Title
		{
			get; set;
		}
		[ForeignKey("Theme"), Column(Order = 2)]
		public string Index
		{
			get; set;
		}
		[ForeignKey("Code"), Required]
		public virtual GroupDetail Details
		{
			get; set;
		}
		[ForeignKey("Code")]
		public virtual Response Page
		{
			get; set;
		}
	}
}