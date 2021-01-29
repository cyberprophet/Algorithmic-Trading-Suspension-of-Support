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
		public IStrategics Strategics
		{
			get; set;
		}
		public bool RenderingStrategics
		{
			get; set;
		}
		public bool IsClickedAmend
		{
			get; set;
		}
		public char SelectStrategics
		{
			get; set;
		}
		public char Temp
		{
			get; set;
		}
	}
}