using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class GroupDetail
	{
		[StringLength(6), Column(Order = 1), ForeignKey("Group"), Key]
		public string Code
		{
			get; set;
		}
		[StringLength(6), Column(Order = 2)]
		public string Date
		{
			get; set;
		}		
		[Required]
		public int Current
		{
			get; set;
		}
		[Required]
		public double Rate
		{
			get; set;
		}
		[Required]
		public double Compare
		{
			get; set;
		}
		[Required]
		public double Percent
		{
			get; set;
		}
		[ForeignKey("Code")]
		public virtual ICollection<Tendency> Tendencies
		{
			get; set;
		}
		public GroupDetail() => Tendencies = new HashSet<Tendency>();
	}
}