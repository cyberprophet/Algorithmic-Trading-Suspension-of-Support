using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using ShareInvest.Catalog.Strategics;
using ShareInvest.Client;
using ShareInvest.EventHandler;
using ShareInvest.SecondaryIndicators;

namespace ShareInvest.Indicators
{
	class BringInTheme : BringIn<SendConsecutive>
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
			string sDate = await api.GetChartsAsync(new Catalog.Models.Charts
			{
				Code = theme.Code,
				Start = Base.Empty,
				End = Base.Empty
			})
				as string, date = string.IsNullOrEmpty(sDate) ? DateTime.Now.AddDays(-5).ToString(Base.DateFormat) : sDate.Substring(0, 6);
			DateTime restore = DateTime.TryParseExact(end, Base.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime store) ? store : DateTime.UnixEpoch, now = DateTime.Now;
			IEnumerable<Catalog.Models.Tick> repository = Repository.RetrieveSavedMaterial(theme.Code), storage = await api.GetContextAsync(new Catalog.Models.Tick(), theme.Code) as IEnumerable<Catalog.Models.Tick>;
			var stack = new Stack<Catalog.Models.Tick>();

			foreach (var day in from day in (await api.GetChartsAsync(new Catalog.Models.Charts { Code = theme.Code, Start = string.Empty, End = string.Empty }) as IEnumerable<Charts>).OrderBy(o => o.Date) where string.Compare(day.Date[2..], date) < 0 select day)
				Days.Enqueue(day);

			if (await api.GetContextAsync(new Catalog.Models.Charts { Code = theme.Code, Start = Days.Count > 0 ? Days.Max(o => o.Date)[2..] : date, End = end }) is IEnumerable<Charts> enumerable)
			{
				string max = string.Empty;

				if (Days.Count > 0)
					max = Days.Max(o => o.Date)[2..];

				foreach (var append in enumerable)
					if (string.IsNullOrEmpty(max) || max.CompareTo(append.Date.Substring(0, 6)) < 0)
						Days.Enqueue(append);
			}
			while (restore.CompareTo(now) < 0)
			{
				if (restore.DayOfWeek is not DayOfWeek.Saturday or DayOfWeek.Sunday && Array.Exists(Base.Holidays, o => o.Equals(restore.ToString(Base.DateFormat))) is false)
				{
					var str = restore.ToString(Base.LongDateFormat);

					if (repository.Any(o => o.Date.Equals(str)))
					{
						var my = repository.First(o => o.Date.Equals(str));

						if (storage is IEnumerable<Catalog.Models.Tick> && storage.Any(o => o.Date.Equals(str)))
						{
							var server = storage.First(o => o.Date.Equals(str));

							if (string.IsNullOrEmpty(my.Close) is false && my.Close.Length == 9 && my.Close.EndsWith(ends) && my.Close[1] is '5' or '6' && string.IsNullOrEmpty(my.Open) is false && my.Open.Length == 9 && my.Open.EndsWith(ends) && my.Open[1] is '9' or '0' && my.Open.CompareTo(server.Open) <= 0 && my.Close.CompareTo(server.Close) >= 0)
							{
								if ((server.Open.Equals(my.Open) && server.Close.Equals(my.Close)) is false && Repository.RetrieveSavedMaterial(my) is string file && await api.PostContextAsync(new Catalog.Models.Tick { Code = my.Code, Date = my.Date, Open = my.Open, Close = my.Close, Price = my.Price, Contents = file }) is int num)
								{
									if (num < 1)
										Base.SendMessage(GetType(), info.Name, "if_post_context");

									await Task.Delay(0x400);
								}
								stack.Push(my);
							}
							else if (string.IsNullOrEmpty(server.Close) is false && server.Close.Length == 9 && server.Close.EndsWith(ends) && server.Close[1] is '5' or '6' && string.IsNullOrEmpty(server.Open) is false && server.Open.Length == 9 && server.Open.EndsWith(ends) && server.Open[1] is '9' or '0' && server.Open.CompareTo(my.Open) <= 0 && server.Close.CompareTo(my.Close) >= 0)
							{
								if ((my.Open.Equals(server.Open) && my.Close.Equals(server.Close)) is false && await api.GetContextAsync(new Catalog.Models.Tick { Code = theme.Code, Date = server.Date, Open = server.Open, Close = server.Close, Price = server.Price, Contents = server.Contents }) is Catalog.Models.Tick tick)
								{
									stack.Push(new Catalog.Models.Tick
									{
										Code = tick.Code,
										Date = tick.Date,
										Open = tick.Open,
										Close = tick.Close,
										Price = tick.Price,
										Contents = string.Empty
									});
									if (Repository.Delete(my))
										Base.SendMessage(GetType(), info.Name, "else if_get_context");

									Repository.KeepOrganizedInStorage(tick);
									await Task.Delay(0x400);
								}
								else
									stack.Push(my);
							}
							else
								stack.Push(my);
						}
						else
						{
							if (Repository.RetrieveSavedMaterial(my) is string file && await api.PostContextAsync(new Catalog.Models.Tick { Code = my.Code, Date = my.Date, Open = my.Open, Close = my.Close, Price = my.Price, Contents = file }) is int num)
							{
								if (num < 1)
									Base.SendMessage(GetType(), info.Name, "else_post_context");

								await Task.Delay(0x400);
							}
							stack.Push(my);
						}
					}
					else if (storage is IEnumerable<Catalog.Models.Tick> && storage.Any(o => o.Date.Equals(str)))
					{
						var server = storage.First(o => o.Date.Equals(str));

						if (string.IsNullOrEmpty(server.Close) is false && server.Close.Length == 9 && server.Close.EndsWith(ends) && server.Close[1] is '5' or '6' && string.IsNullOrEmpty(server.Open) is false && server.Open.Length == 9 && server.Open.EndsWith(ends) && server.Open[1] is '9' or '0' && await api.GetContextAsync(new Catalog.Models.Tick { Code = theme.Code, Date = server.Date, Open = server.Open, Close = server.Close, Price = server.Price, Contents = server.Contents }) is Catalog.Models.Tick tick)
						{
							stack.Push(new Catalog.Models.Tick
							{
								Code = tick.Code,
								Date = tick.Date,
								Open = tick.Open,
								Close = tick.Close,
								Price = tick.Price,
								Contents = string.Empty
							});
							Repository.KeepOrganizedInStorage(tick);
							await Task.Delay(0x400);
						}
					}
				}
				restore = restore.AddDays(1);
			}
			yield return (IEnumerable<T>)stack.OrderBy(o => o.Date);
		}
		protected override Queue<Charts> Days
		{
			get;
		}
		public override event EventHandler<SendConsecutive> Send;
		public override async Task<object> StartProgress()
		{
			var index = uint.MinValue;
			var collection = new int[0x14];
			var percent = double.NaN;
			ConfirmRevisedStockPrice[] modify = null;
			Queue<ConfirmRevisedStockPrice> revise = null;
			var tick = FindTheOldestDueDate<Catalog.Models.Tick>().GetAsyncEnumerator();

			if (await api.GetContextAsync(new RevisedStockPrice { Code = theme.Code }) is Queue<ConfirmRevisedStockPrice> queue && queue.Count > 0)
			{
				modify = new ConfirmRevisedStockPrice[queue.Count];
				revise = queue;
			}
			while (await tick.MoveNextAsync())
				if (tick.Current is IEnumerable<Catalog.Models.Tick> enumerable)
					foreach (var model in enumerable)
						try
						{
							while (modify is ConfirmRevisedStockPrice[] && revise.TryDequeue(out ConfirmRevisedStockPrice param))
								if (param.Date.CompareTo(Days.Count > 0 ? (Days.Any(o => o.Date.Length == 8) ? Days.Where(o => o.Date.Length == 8).Max(o => o.Date)[2..] : Days.Max(o => o.Date).Substring(0, 6)) : model.Date[2..]) > 0 && (revise.Count == 0 || revise.TryPeek(out ConfirmRevisedStockPrice peek) && param.Rate != peek.Rate))
									modify[index++] = param;

							index = uint.MinValue;

							if (Days.Count > 0)
								while (Days.TryDequeue(out Charts dequeue))
								{
									if (int.TryParse(dequeue.Price[0] is '-' ? dequeue.Price[1..] : dequeue.Price, out int price))
									{
										var rate = 1D;

										switch (modify)
										{
											case ConfirmRevisedStockPrice[]:
												foreach (var param in Array.FindAll(modify, o => string.IsNullOrEmpty(o.Date) is false && o.Date.CompareTo(dequeue.Date.Length == 8 ? dequeue.Date[2..] : dequeue.Date.Substring(0, 6)) > 0))
													if (double.IsNormal(param.Rate) && param.Rate != 0)
														rate *= param.Rate * 1e-2 + 1;
												break;
										}
										var now = rate == 1D ? price : Base.GetStartingPrice((int)(rate * price), info.MarginRate == 1);
										collection[index++ % 0x14] = now;

										if (index > 0x13)
											Bollinger.CalculateBands(now, collection, 2);
									}
									Send?.Invoke(this, null);
								}
							if (int.TryParse(model.Price, out int current))
							{
								var rate = 1D;

								switch (modify)
								{
									case ConfirmRevisedStockPrice[]:
										foreach (var param in Array.FindAll(modify, o => string.IsNullOrEmpty(o.Date) is false && o.Date.CompareTo(model.Date.Substring(2, 6)) > 0))
											if (double.IsNormal(param.Rate) && param.Rate != 0)
												rate *= 1 + param.Rate * 1e-2;
										break;
								}
								var now = rate == 1D ? current : Base.GetStartingPrice((int)(rate * current), info.MarginRate == 1);
								collection[index++ % 0x14] = now;

								if (index > 0x13)
								{
									var band = Bollinger.CalculateBands(now, collection, 2);
									percent = band.Item4;
								}
							}
						}
						catch (Exception ex)
						{
							Base.SendMessage(GetType(), info.Name, ex.StackTrace);
						}
			return percent;
		}
		readonly API api;
		readonly Catalog.Models.Codes info;
		readonly Catalog.Models.GroupDetail theme;
		const string end = "210105";
		const string ends = "000";
	}
}