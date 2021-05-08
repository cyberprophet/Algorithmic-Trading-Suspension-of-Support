using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class Classification
	{
		[StringLength(6), Column(Order = 1)]
		public string Code
		{
			get; set;
		}
		[ForeignKey("Theme"), Column(Order = 2)]
		public string Index
		{
			get; set;
		}
		[Column(Order = 3)]
		public string Title
		{
			get; set;
		}
	}
}