using System.Collections.Generic;

namespace ShareInvest.Catalog.Models
{
	public class Portfolio
	{
		public IEnumerable<Consensus> Consensus
		{
			get; set;
		}
		public Balance Balance
		{
			get; set;
		}
	}
}