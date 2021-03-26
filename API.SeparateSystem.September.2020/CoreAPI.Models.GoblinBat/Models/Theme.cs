using System.Collections.Generic;
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
		[ForeignKey("Index")]
		public virtual ICollection<Group> Groups
		{
			get; set;
		}
		[ForeignKey("Index")]
		public virtual Url Url
		{
			get; set;
		}
		[ForeignKey("Index")]
		public virtual ThemeTags Tags
		{
			get; set;
		}
		public Theme() => Groups = new HashSet<Group>();
	}
}