using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Interface.OpenAPI;
using ShareInvest.SecondaryIndicators;

namespace ShareInvest.Indicators
{
	public class TrendsToCashflow : Analysis
	{
		int Accumulative
		{
			get; set;
		}
		double Before
		{
			get; set;
		}
		long TodayRevenue
		{
			get; set;
		}
		long TodayUnrealize
		{
			get; set;
		}
		internal Catalog.Strategics.Statistics SendMessage
		{
			get; private set;
		}
		internal (Stack<double>, Stack<double>, Stack<double>) ReturnTheUsedStack
		{
			get
			{
				Stack<double> st = new Stack<double>(), lg = new Stack<double>(), td = new Stack<double>(),
					short_stack = new Stack<double>(), long_stack = new Stack<double>(), trend_stack = new Stack<double>();

				if (Strategics is Catalog.TrendsToCashflow tc)
				{
					while (st.Count <= tc.Short)
						st.Push(Short.Pop());

					while (lg.Count <= tc.Long)
						lg.Push(Long.Pop());

					while (td.Count <= tc.Trend)
						td.Push(Trend.Pop());

					while (st.Count > 0)
						short_stack.Push(st.Pop());

					while (lg.Count > 0)
						long_stack.Push(lg.Pop());

					while (td.Count > 0)
						trend_stack.Push(td.Pop());
				}
				return (short_stack, long_stack, trend_stack);
			}
		}
		internal void StartProgress(double commission)
		{
			if (Strategics is Catalog.TrendsToCashflow cf)
			{
				Commission = commission;
				Short = new Stack<double>();
				Long = new Stack<double>();
				Trend = new Stack<double>();
				OrderNumber = new Dictionary<string, dynamic>();
				NextOrderTime = DateTime.MinValue;
				Line = new Tuple<int, int, int>(cf.Short, cf.Long, cf.Trend);
			}
		}
		public override event EventHandler<SendConsecutive> Send;
		public override async Task<Balance> OnReceiveBalance<T>(T param) where T : struct
		{
			await Slim.WaitAsync();

			return Balance;
		}
		public override async Task<Tuple<dynamic, bool, int>> OnReceiveConclusion<T>(T param) where T : struct
		{
			await Slim.WaitAsync();

			return null;
		}
		public override async Task AnalyzeTheConclusionAsync(string[] param)
		{
			try
			{
				await Slim.WaitAsync();
				Send?.Invoke(this, new SendConsecutive(param));
			}
			catch (Exception ex)
			{
				Base.SendMessage(Code, ex.StackTrace, GetType());
			}
			finally
			{
				if (Slim.Release() > 0)
					Base.SendMessage(Code, param[0], GetType());
			}
		}
		public override async Task AnalyzeTheQuotesAsync(string[] param)
		{
			try
			{
				await Quote.WaitAsync();
				Send?.Invoke(this, new SendConsecutive(param));
			}
			catch (Exception ex)
			{
				Base.SendMessage(Code, ex.StackTrace, GetType());
			}
			finally
			{
				if (Quote.Release() > 0)
					Base.SendMessage(Code, param[0], GetType());
			}
		}
		public override void OnReceiveDrawChart(object sender, SendConsecutive e)
		{
			if (GetCheckOnDate(e.Date))
			{
				Short.Pop();
				Long.Pop();
				Trend.Pop();
			}
			Trend.Push(Trend.Count > 0 ? EMA.Make(Line.Item3, Trend.Count, e.Price, Trend.Peek()) : EMA.Make(e.Price));
			Short.Push(Short.Count > 0 ? EMA.Make(Line.Item1, Short.Count, e.Price, Short.Peek()) : EMA.Make(e.Price));
			Long.Push(Long.Count > 0 ? EMA.Make(Line.Item2, Long.Count, e.Price, Long.Peek()) : EMA.Make(e.Price));

			if (e.Volume != 0 && e.Date.Length > 8 && Short.Count > 1 && Long.Count > 1)
			{
				double popShort = Short.Pop(), popLong = Long.Pop(), gap = popShort - popLong - (Short.Peek() - Long.Peek());
				Short.Push(popShort);
				Long.Push(popLong);
				var date = e.Date.Substring(6, 4);

				if (date.CompareTo(Base.Start) > 0 && date.CompareTo(Base.Transmit) < 0 && Strategics is Catalog.TrendsToCashflow tc
					&& DateTime.TryParseExact(e.Date.Substring(0, 12), Base.FullDateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime cInterval))
				{
					if (Balance.Quantity > tc.ReservationQuantity - 1 && (Offer ?? int.MaxValue) < e.Price
						&& OrderNumber.Any(o => o.Key[0] == '8' && o.Value == e.Price - GetQuoteUnit(e.Price, Market)))
					{
						CumulativeFee += (uint)(e.Price * tc.ReservationQuantity * (Commission + Base.Tax));
						Balance.Revenue += (long)((e.Price - (Balance.Purchase ?? 0D)) * tc.ReservationQuantity);
						Balance.Quantity -= tc.ReservationQuantity;
						var profit = OrderNumber.First(o => o.Key.StartsWith("8") && o.Value == e.Price - GetQuoteUnit(e.Price, Market));

						if (OrderNumber.Remove(profit.Key))
						{
							Capital -= profit.Value * tc.ReservationQuantity;
							Offer = profit.Value;
						}
					}
					else if ((Bid ?? int.MinValue) > e.Price && OrderNumber.Any(o => o.Key[0] == '7' && o.Value == e.Price + GetQuoteUnit(e.Price, Market)))
					{
						CumulativeFee += (uint)(e.Price * Commission * tc.ReservationQuantity);
						Balance.Purchase
							= (double)((e.Price * tc.ReservationQuantity + (Balance.Purchase ?? 0D) * Balance.Quantity) / (Balance.Quantity + tc.ReservationQuantity));
						Balance.Quantity += tc.ReservationQuantity;
						var profit = OrderNumber.First(o => o.Key.StartsWith("7") && o.Value == e.Price + GetQuoteUnit(e.Price, Market));

						if (OrderNumber.Remove(profit.Key))
						{
							Capital += profit.Value * tc.ReservationQuantity;
							Bid = profit.Value;
						}
					}
					else if (Balance.Quantity > tc.TradingQuantity - 1 && OrderNumber.Any(o => o.Key[0] == '2' && o.Value == e.Price - GetQuoteUnit(e.Price, Market)))
					{
						CumulativeFee += (uint)(e.Price * tc.TradingQuantity * (Commission + Base.Tax));
						Balance.Revenue += (long)((e.Price - (Balance.Purchase ?? 0D)) * tc.TradingQuantity);
						Balance.Quantity -= tc.TradingQuantity;
						var profit = OrderNumber.First(o => o.Key.StartsWith("2") && o.Value == e.Price - GetQuoteUnit(e.Price, Market));

						if (OrderNumber.Remove(profit.Key))
							Capital -= profit.Value * tc.TradingQuantity;
					}
					else if (OrderNumber.Any(o => o.Key.StartsWith("1") && o.Value == e.Price + GetQuoteUnit(e.Price, Market)))
					{
						CumulativeFee += (uint)(e.Price * Commission * tc.TradingQuantity);
						Balance.Purchase = (double)((e.Price * tc.TradingQuantity + (Balance.Purchase ?? 0D) * Balance.Quantity) / (Balance.Quantity + tc.TradingQuantity));
						Balance.Quantity += tc.TradingQuantity;
						var profit = OrderNumber.First(o => o.Key.StartsWith("1") && o.Value == e.Price + GetQuoteUnit(e.Price, Market));

						if (OrderNumber.Remove(profit.Key))
							Capital += profit.Value * tc.TradingQuantity;
					}
					else if (Balance.Quantity > tc.TradingQuantity - 1 && OrderNumber.ContainsValue(e.Price) == false
						&& e.Price > Trend.Peek() * (1 + tc.PositionRevenue) && e.Price > (Balance.Purchase ?? 0D)
						&& gap < 0 && (tc.Interval == 0 || tc.Interval > 0 && cInterval.CompareTo(NextOrderTime) > 0))
					{
						var unit = GetQuoteUnit(e.Price, Market);

						if (OrderNumber.ContainsValue(e.Price + unit) == false)
							OrderNumber[Base.GetOrderNumber((int)OrderType.신규매도)] = e.Price + unit;

						if (tc.Interval > 0)
							NextOrderTime = Base.MeasureTheDelayTime(tc.Interval, cInterval);
					}
					else if (tc.TradingQuantity > 0 && OrderNumber.ContainsValue(e.Price) == false && e.Price < Trend.Peek() * (1 - tc.PositionAddition)
						&& gap > 0 && (tc.Interval == 0 || tc.Interval > 0 && cInterval.CompareTo(NextOrderTime) > 0))
					{
						var unit = GetQuoteUnit(e.Price, Market);

						if (OrderNumber.ContainsValue(e.Price - unit) == false)
							OrderNumber[Base.GetOrderNumber((int)OrderType.신규매수)] = e.Price - unit;

						if (tc.Interval > 0)
							NextOrderTime = Base.MeasureTheDelayTime(tc.Interval, cInterval);
					}
				}
				else if (date.CompareTo(Base.Transmit) > 0 && Strategics is Catalog.TrendsToCashflow cf)
				{
					OrderNumber.Clear();
					long revenue = Balance.Revenue - CumulativeFee, unrealize = (long)((e.Price - (Balance.Purchase ?? 0D)) * Balance.Quantity);
					var avg = EMA.Make(++Accumulative, revenue - TodayRevenue + unrealize - TodayUnrealize, Before);

					if (cf.ReservationQuantity > 0 && Balance.Quantity > cf.ReservationQuantity - 1)
					{
						var stock = Market;
						int quantity = Balance.Quantity / cf.ReservationQuantity, price = e.Price, sell = (int)((Balance.Purchase ?? 0D) * (1 + cf.ReservationRevenue)),
							buy = (int)((Balance.Purchase ?? 0D) * (1 - cf.Addition)), upper = (int)(price * 1.3), lower = (int)(price * 0.7),
							bPrice = Base.GetStartingPrice(lower, stock), sPrice = Base.GetStartingPrice(sell, stock);
						sPrice = sPrice < lower ? lower + GetQuoteUnit(sPrice, stock) : sPrice;

						while (sPrice < upper && quantity-- > 0)
						{
							OrderNumber[Base.GetOrderNumber((int)OrderType.예약매도)] = sPrice;

							for (int i = 0; i < cf.Unit; i++)
								sPrice += GetQuoteUnit(sPrice, stock);
						}
						while (bPrice < upper && bPrice < buy)
						{
							OrderNumber[Base.GetOrderNumber((int)OrderType.예약매수)] = bPrice;

							for (int i = 0; i < cf.Unit; i++)
								bPrice += GetQuoteUnit(bPrice, stock);
						}
						Bid = OrderNumber.Count > 0 && OrderNumber.Any(o => o.Key.StartsWith("7")) ? OrderNumber.Where(o => o.Key.StartsWith("7")).Max(o => o.Value) : 0;
						Offer = OrderNumber.Count > 0 && OrderNumber.Any(o => o.Key.StartsWith("8")) ? OrderNumber.Where(o => o.Key.StartsWith("8")).Min(o => o.Value) : 0;
					}
					SendMessage = new Catalog.Strategics.Statistics
					{
						Key = string.Concat("TC.", cf.AnalysisType),
						Date = e.Date.Substring(0, 6),
						Cumulative = revenue + unrealize,
						Base = SendMessage.Base > Capital ? SendMessage.Base : Capital,
						Statistic = (int)avg,
						Price = (int)Trend.Peek()
					};
					Before = avg;
					TodayRevenue = revenue;
					TodayUnrealize = unrealize;
				}
			}
		}
		public override bool Collector
		{
			get; set;
		}
		public override bool Wait
		{
			get; set;
		}
		public override string Code
		{
			get; set;
		}
		public override dynamic Current
		{
			get; set;
		}
		public override dynamic Offer
		{
			get; set;
		}
		public override dynamic Bid
		{
			get; set;
		}
		public override double Capital
		{
			get; protected internal set;
		}
		public override Balance Balance
		{
			get; set;
		}
		public override IStrategics Strategics
		{
			get; set;
		}
		public override Queue<Collect> Collection
		{
			get; set;
		}
		public override Dictionary<string, dynamic> OrderNumber
		{
			get; set;
		}
		public override Stack<double> Short
		{
			protected internal get; set;
		}
		public override Stack<double> Long
		{
			protected internal get; set;
		}
		public override Stack<double> Trend
		{
			protected internal get; set;
		}
		protected internal override Tuple<int, int, int> Line
		{
			get; set;
		}
		protected internal override SemaphoreSlim Quote => new SemaphoreSlim(1, 1);
		protected internal override SemaphoreSlim Slim => new SemaphoreSlim(1, 1);
		protected internal override DateTime NextOrderTime
		{
			get; set;
		}
		protected internal override string DateChange
		{
			get; set;
		}
		protected internal override uint CumulativeFee
		{
			get; set;
		}
		protected internal override bool GetCheckOnDeadline(string time)
		{
			var date = time.Substring(0, 6);
			var change = string.IsNullOrEmpty(DateChange) == false && DateChange.Equals(date);
			DateChange = date;

			return change;
		}
		protected internal override bool GetCheckOnDate(string date) => date.Length > 8 && GetCheckOnDeadline(date);
	}
}