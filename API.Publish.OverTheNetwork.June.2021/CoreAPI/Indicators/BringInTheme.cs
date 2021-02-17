using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ShareInvest.Catalog.Strategics;
using ShareInvest.Client;
using ShareInvest.EventHandler;

namespace ShareInvest.Indicators
{
	class BringInTheme : BringIn
	{
		internal BringInTheme(API api, Catalog.Models.GroupDetail theme, Catalog.Models.Codes info)
		{
			this.theme = theme;
			this.api = api;
			this.info = info;
			Days = new Queue<Charts>(0x20);
		}
		protected override async IAsyncEnumerable<IEnumerable<T>> FindTheOldestDueDate<T>() where T : struct
		{
			var enumerable = Repository.RetrieveSavedMaterial(theme.Code);
			string sDate = await api.GetChartsAsync(new Catalog.Models.Charts
			{
				Code = theme.Code,
				Start = Base.Empty,
				End = Base.Empty
			})
				as string, date = string.IsNullOrEmpty(sDate) ? DateTime.Now.AddDays(-5).ToString(Base.DateFormat) : sDate.Substring(0, 6);

			foreach (var day in from day in (await api.GetChartsAsync(new Catalog.Models.Charts { Code = theme.Code, Start = string.Empty, End = string.Empty }) as IEnumerable<Charts>).OrderBy(o => o.Date) where string.Compare(day.Date[2..], date) < 0 select day)
				Days.Enqueue(day);

			yield return (IEnumerable<T>)enumerable;
		}
		protected override Queue<Charts> Days
		{
			get;
		}
		public override event EventHandler<SendConsecutive> Send;
		public override async Task<object> StartProgress()
		{
			var modify = (await api.GetContextAsync(new RevisedStockPrice { Code = theme.Code }) as Queue<ConfirmRevisedStockPrice>)?.ToArray();
			var tick = FindTheOldestDueDate<Catalog.Models.Tick>().GetAsyncEnumerator();

			while (await tick.MoveNextAsync())
				try
				{

				}
				catch (Exception ex)
				{
					Base.SendMessage(GetType(), info.Name, ex.StackTrace);
				}
			return null;
		}
		readonly API api;
		readonly Catalog.Models.Codes info;
		readonly Catalog.Models.GroupDetail theme;
	}
}