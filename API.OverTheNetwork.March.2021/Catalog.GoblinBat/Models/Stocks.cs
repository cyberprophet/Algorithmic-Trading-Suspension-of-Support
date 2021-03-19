using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ShareInvest.Catalog.Models
{
	public struct Stocks
	{
		public string Code
		{
			get; set;
		}
		public string Retention
		{
			get; set;
		}
		public string Date
		{
			get; set;
		}
		public string Price
		{
			get; set;
		}
		public int Volume
		{
			get; set;
		}
	}
	public class Comparer : IEqualityComparer<Stocks>
	{
		public bool Equals(Stocks x, Stocks y) => x.Code.Equals(y.Code) && x.Date.Equals(y.Date);
		public int GetHashCode([DisallowNull] Stocks stock) => (stock.Date is null ? 0 : stock.Date.GetHashCode()) ^ (stock.Code.GetHashCode());
	}
}