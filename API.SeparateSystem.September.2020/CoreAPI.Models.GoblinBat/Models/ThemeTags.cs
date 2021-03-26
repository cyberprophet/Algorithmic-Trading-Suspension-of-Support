using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class ThemeTags
	{
		[Key, Column(Order = 1), ForeignKey("Theme")]
		public string Index
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