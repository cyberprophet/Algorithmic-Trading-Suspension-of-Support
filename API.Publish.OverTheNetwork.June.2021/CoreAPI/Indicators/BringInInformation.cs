using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using ShareInvest.Catalog.Models;
using ShareInvest.Catalog.Strategics;
using ShareInvest.Client;
using ShareInvest.EventHandler;

namespace ShareInvest.Indicators
{
	class BringInInformation
	{
		public event EventHandler<SendConsecutive> Send;
		internal BringInInformation(Codes cm, Queue<ConfirmRevisedStockPrice> revise, API api)
		{
			this.api = api;
			this.cm = cm;
			this.revise = revise;
			Days = new Queue<Catalog.Strategics.Charts>();
		}
		internal async Task<string> StartProgress()
		{
			var modify = revise is not null && revise.Count > 0 ? new ConfirmRevisedStockPrice[revise.Count] : null;
			var index = 0;
			var tick = FindTheOldestDueDate().GetAsyncEnumerator();
			var response = string.Empty;

			while (await tick.MoveNextAsync())
				if (tick.Current is not null)
					try
					{
						var enumerable = tick.Current.OrderBy(o => o.Date);
						var before = enumerable.First().Date.Substring(0, 6);

						while (revise != null && revise.Count > 0)
						{
							var param = revise.Dequeue();

							if (param.Date.CompareTo(Days.Count > 0 ? Days.Max(o => o.Date)[2..] : before) > 0)
							{
								if (revise.Count == 0)
								{
									modify[index] = param;

									break;
								}
								var peek = revise.Peek();

								if (param.Rate != peek.Rate)
									modify[index++] = param;
							}
						}
						while (Days.Count > 0)
						{
							var day = Days.Dequeue();

							if (string.Compare(day.Date[2..], before) < 0)
							{
								SendConsecutive convey;

								if (modify != null && int.TryParse(day.Price[0] is '-' ? day.Price[1..] : day.Price, out int price))
								{
									var rate = 1D;

									foreach (var param in Array.FindAll(modify, o => string.IsNullOrEmpty(o.Date) == false && o.Date.CompareTo(day.Date[2..]) > 0))
										rate *= param.Rate;

									convey = new SendConsecutive(day.Date, Base.GetStartingPrice((int)((1 + rate * 1e-2) * price), cm.MarginRate == 1), day.Volume);
								}
								else
									convey = new SendConsecutive(day);

								Send?.Invoke(this, convey);
							}
						}
						foreach (var consecutive in enumerable)
						{
							SendConsecutive convey;
							response = consecutive.Date;

							if (modify != null && int.TryParse(consecutive.Price[0] is '-' ? consecutive.Price[1..] : consecutive.Price, out int price))
							{
								var rate = 1D;

								foreach (var param in Array.FindAll(modify, o => string.IsNullOrEmpty(o.Date) == false && o.Date.CompareTo(consecutive.Date.Substring(0, 6)) > 0))
									rate *= param.Rate;

								convey = new SendConsecutive(consecutive.Date, Base.GetStartingPrice((int)((1 + rate * 1e-2) * price), cm.MarginRate == 1), consecutive.Volume);
							}
							else
								convey = new SendConsecutive(consecutive);

							Send?.Invoke(this, convey);
						}
					}
					catch (Exception ex)
					{
						Base.SendMessage(ex.StackTrace, cm.Name, GetType());
					}
			return string.Concat(cm.Name, '_', response);
		}
		async IAsyncEnumerable<IEnumerable<Catalog.Strategics.Charts>> FindTheOldestDueDate()
		{
			if (cm.Code.Length == 8 && cm.Code[0] == '1' && cm.Code[1] == '0')
			{
				var stack = new Stack<Codes>();
				var list = await api.GetContextAsync(new Codes(), 8) as List<Codes>;

				foreach (var arg in list.Where(o => o.Code.StartsWith(cm.Code.Substring(0, 3)) && o.Code.EndsWith(cm.Code[5..]))
					.OrderByDescending(o => o.MaturityMarketCap.Length == 8 ? o.MaturityMarketCap[2..] : o.MaturityMarketCap))
				{
					if (uint.TryParse(arg.MaturityMarketCap.Length == 8 ? arg.MaturityMarketCap[2..] : arg.MaturityMarketCap, out uint remain))
					{

					}
					stack.Push(arg);
				}
				foreach (var arg in (await api.GetChartsAsync(new Catalog.Models.Charts
				{
					Code = stack.Peek().Code,
					Start = string.Empty,
					End = string.Empty
				})
					as IEnumerable<Catalog.Strategics.Charts>).OrderBy(o => o.Date))
					Days.Enqueue(arg);

				while (stack.Count > 0)
				{
					var codes = stack.Pop();
					string start = string.Empty, last = list.LastOrDefault(o => o.Code.StartsWith(codes.Code.Substring(0, 3)) && o.Code.EndsWith(codes.Code[5..])
						&& string.Compare(o.MaturityMarketCap.Length == 8 ? o.MaturityMarketCap[2..] : o.MaturityMarketCap, codes.MaturityMarketCap.Length == 8 ? codes.MaturityMarketCap[2..] : codes.MaturityMarketCap) < 0).MaturityMarketCap;

					if (codes.Code.Equals("105QA000"))
					{
						start = Base.CallUpTheChart(list.First(o => o.Code.Equals("101QC000")), last);

						foreach (var arg in (await api.GetChartsAsync(new Catalog.Models.Charts
						{
							Code = codes.Code,
							Start = start.Length == 8 ? start[2..] : start,
							End = codes.MaturityMarketCap.Length == 8 ? codes.MaturityMarketCap[2..] : codes.MaturityMarketCap
						})
							as IEnumerable<Catalog.Strategics.Charts>).OrderBy(o => o.Date))
							if (arg.Date.Substring(6, 4).Equals("1545"))
								Days.Enqueue(new Catalog.Strategics.Charts
								{
									Date = string.Concat("20", arg.Date.Substring(0, 6)),
									Price = arg.Price
								});
						continue;
					}
					start = Base.CallUpTheChart(codes, last);

					yield return await api.GetChartsAsync(new Catalog.Models.Charts
					{
						Code = codes.Code,
						Start = start.Length == 8 ? start[2..] : start,
						End = codes.MaturityMarketCap.Length == 8 ? codes.MaturityMarketCap[2..] : codes.MaturityMarketCap
					})
						as IEnumerable<Catalog.Strategics.Charts>;
				}
			}
			else if (cm.Code.Length == 6)
			{
				string sDate = await api.GetChartsAsync(new Catalog.Models.Charts { Code = cm.Code, Start = empty, End = empty }) as string, date = string.IsNullOrEmpty(sDate) ? DateTime.Now.AddDays(-5).ToString(Base.DateFormat) : sDate.Substring(0, 6);

				foreach (var day in from day in (await api.GetChartsAsync(new Catalog.Models.Charts { Code = cm.Code, Start = string.Empty, End = string.Empty }) as IEnumerable<Catalog.Strategics.Charts>).OrderBy(o => o.Date) where string.Compare(day.Date[2..], date) < 0 select day)
					Days.Enqueue(day);

				if (DateTime.TryParseExact(date, Base.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime start))
				{
					var end = string.Empty;
					var count = 0;

					while (string.IsNullOrEmpty(end) || string.Compare(end, DateTime.Now.ToString(Base.DateFormat)) <= 0)
						yield return await api.GetChartsAsync(new Catalog.Models.Charts
						{
							Code = cm.Code,
							Start = start.AddDays(-1).AddDays(0x4B * count++).ToString(Base.DateFormat),
							End = end = start.AddDays(-1).AddDays(0x4B * count).ToString(Base.DateFormat)
						}) as IEnumerable<Catalog.Strategics.Charts>;
				}
			}
		}
		Queue<Catalog.Strategics.Charts> Days
		{
			get;
		}
		readonly Queue<ConfirmRevisedStockPrice> revise;
		readonly Codes cm;
		readonly API api;
		const string empty = "empty";
	}
}