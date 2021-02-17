using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ShareInvest.EventHandler;

namespace ShareInvest.Indicators
{
	public abstract partial class BringIn
	{
		public abstract event EventHandler<SendConsecutive> Send;
		public abstract Task<object> StartProgress();
		protected abstract Queue<Catalog.Strategics.Charts> Days
		{
			get;
		}
		protected abstract IAsyncEnumerable<IEnumerable<T>> FindTheOldestDueDate<T>() where T : struct;
	}
}