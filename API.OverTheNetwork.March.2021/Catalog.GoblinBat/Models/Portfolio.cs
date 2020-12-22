using System.Collections.Generic;

using ShareInvest.Interface;

namespace ShareInvest.Catalog.Models
{
	public class Portfolio
	{
		public IEnumerable<Consensus> Consensus
		{
			get; set;
		}
		public bool RenderingConsensus
		{
			get; set;
		}
		public Balance Balance
		{
			get; set;
		}
		public bool RenderingBalance
		{
			get; set;
		}
		public IStrategics Strategics
		{
			get; set;
		}
		public bool RenderingStrategics
		{
			get; set;
		}
		public char SelectStrategics
		{
			get; set;
		}
	}
}