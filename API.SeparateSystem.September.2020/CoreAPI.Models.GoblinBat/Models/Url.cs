using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class Url
	{
		[Key, Column(Order = 1), ForeignKey("Theme")]
		public string Index
		{
			get; set;
		}
		[StringLength(6), Column(Order = 3)]
		public string Record
		{
			get; set;
		}
		[Column(Order = 2)]
		public string Json
		{
			get; set;
		}
	}
}