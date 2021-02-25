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
		[DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 2)]
		public int Tick
		{
			get; set;
		}
		[Required, Column(Order = 3)]
		public double Inclination
		{
			get; set;
		}
	}
}