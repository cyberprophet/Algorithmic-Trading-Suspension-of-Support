using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareInvest.Indicators
{
	public abstract partial class BringIn<Event>
	{
		public abstract event EventHandler<Event> Send;
		public abstract Task<object> StartProgress();
		protected abstract Queue<Catalog.Strategics.Charts> Days
		{
			get;
		}
		protected abstract IAsyncEnumerable<IEnumerable<T>> FindTheOldestDueDate<T>() where T : struct;
	}
}