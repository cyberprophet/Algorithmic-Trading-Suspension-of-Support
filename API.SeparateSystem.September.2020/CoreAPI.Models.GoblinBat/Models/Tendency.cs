using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class Tendency
	{
		[StringLength(6), Column(Order = 1)]
		public string Code
		{
			get; set;
		}
		[StringLength(6), Column(Order = 2)]
		public string Date
		{
			get; set;
		}
		[DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 3)]
		public int Tick
		{
			get; set;
		}
		[Required]
		public double Inclination
		{
			get; set;
		}
	}
}