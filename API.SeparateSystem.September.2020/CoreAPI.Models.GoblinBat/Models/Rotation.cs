using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class Rotation
	{
		[Column(Order = 1), StringLength(6)]
		public string Date
		{
			get; set;
		}
		[ForeignKey("Codes"), Column(Order = 2), MinLength(6), MaxLength(8)]
		public string Code
		{
			get; set;
		}
		[Column(Order = 3)]
		public int Purchase
		{
			get; set;
		}
		public int High
		{
			get; set;
		}
		public double MaxReturn
		{
			get; set;
		}
		public int Low
		{
			get; set;
		}
		public double MaxLoss
		{
			get; set;
		}
		public int Close
		{
			get; set;
		}
		public double Liquidation
		{
			get; set;
		}
	}
}