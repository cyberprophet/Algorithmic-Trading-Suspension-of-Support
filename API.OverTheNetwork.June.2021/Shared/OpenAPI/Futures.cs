using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI
{
	public class Futures : Analysis
	{
		public override event EventHandler<SendConsecutive> Send;
		public override double MarginRate
		{
			get; set;
		}
		public override void OnReceiveDrawChart(object sender, SendConsecutive e)
		{
			if (e.Matrix is null)
			{

				return;
			}
		}
		public override async Task<Catalog.Models.Balance> OnReceiveBalance<T>(T param) where T : struct
		{
			if (param is Catalog.OpenAPI.Derivatives bal
					&& double.TryParse(bal.Offer[0] is '-' ? bal.Offer[1..] : bal.Offer, out double offer)
					&& double.TryParse(bal.Bid[0] is '-' ? bal.Bid[1..] : bal.Bid, out double bid)
					&& double.TryParse(bal.Purchase[0] is '-' ? bal.Purchase[1..] : bal.Purchase, out double unit)
					&& double.TryParse(bal.Unit, out double transaction) && int.TryParse(bal.Quantity, out int amount)
					&& double.TryParse(bal.Current[0] is '-' ? bal.Current[1..] : bal.Current, out double price))
				try
				{
					await Slim.WaitAsync();
					var classification = bal.TradingClassification[0] is '1' ? -1 : 1;
					var index = bal.Code[1] is '0';
					Current = index ? price : (int)price;
					Balance.Quantity = amount * classification;
					Balance.Purchase = index ? unit : (int)unit;
					Balance.Revenue = (long)((price - unit) * classification * amount * transaction);
					Balance.Rate = price / unit - 1;
					Bid = index ? bid : (int)bid;
					Offer = index ? offer : (int)offer;
					Wait = true;
				}
				catch (Exception ex)
				{
					Base.SendMessage(bal.Name, ex.StackTrace, param.GetType());
				}
				finally
				{
					if (Slim.Release() > 0)
						Base.SendMessage(bal.Name, bal.Account, param.GetType());
				}
			return new Catalog.Models.Balance
			{
				Code = Code,
				Name = Balance.Name,
				Quantity = Balance.Quantity.ToString("N0"),
				Purchase = Balance.Purchase.ToString(Code[1] is '0' ? "N2" : "N0"),
				Current = Current.ToString(Code[1] is '0' ? "N2" : "N0"),
				Revenue = Balance.Revenue.ToString("C0"),
				Rate = Balance.Rate.ToString("P2")
			};
		}
		public override async Task<Tuple<dynamic, bool, int>> OnReceiveConclusion<T>(T param) where T : struct
		{
			if (param is Catalog.OpenAPI.Conclusion con && double.TryParse(con.CurrentPrice[0] is '-' ? con.CurrentPrice[1..] : con.CurrentPrice, out double current))
				try
				{
					var remove = true;
					await Slim.WaitAsync();

					switch (con.OrderState)
					{
						case conclusion:
							if (OrderNumber.Remove(con.OrderNumber))
								remove = false;

							break;

						case acceptance when con.UnsettledQuantity[0] is not '0':
							if (double.TryParse(con.OrderPrice, out double price) && price > 0)
								OrderNumber[con.OrderNumber] = con.Code[1] is '0' ? price : (int)price;

							break;

						case confirmation when con.OrderClassification.EndsWith(cancellantion) || con.OrderClassification.EndsWith(correction):
							remove = OrderNumber.Remove(con.OriginalOrderNumber);
							break;
					}
					return new Tuple<dynamic, bool, int>(con.Code[1] is '0' ? current : (int)current, remove, 0);
				}
				catch (Exception ex)
				{
					Base.SendMessage(con.Name, ex.StackTrace, param.GetType());
				}
				finally
				{
					if (Slim.Release() > 0)
						Base.SendMessage(con.Name, con.Account, param.GetType());
				}
			return null;
		}
		public override async Task AnalyzeTheConclusionAsync(string[] param)
		{
			if (int.TryParse(param[^1], out int volume))
				try
				{
					await Slim.WaitAsync();
					Send?.Invoke(this, new SendConsecutive(new Catalog.Strategics.Charts
					{
						Date = param[0],
						Price = param[1],
						Volume = volume
					}));
				}
				catch (Exception ex)
				{
					Base.SendMessage(Code, ex.StackTrace, GetType());
				}
				finally
				{
					if (Slim.Release() > 0)
						Base.SendMessage(Code, param.Length, GetType());
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
					Base.SendMessage(Code, param.Length, GetType());
			}
		}
		public override (IEnumerable<Collect>, uint, uint, string) SortTheRecordedInformation => base.SortTheRecordedInformation;
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
		public override Interface.IStrategics Strategics
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
		protected internal override bool GetCheckOnDate(string date)
		{
			return true;
		}
		protected internal override bool GetCheckOnDeadline(string time)
		{
			return true;
		}
	}
}