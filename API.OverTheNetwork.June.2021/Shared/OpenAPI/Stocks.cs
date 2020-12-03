using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI
{
	public class Stocks : Analysis
	{
		public override event EventHandler<SendConsecutive> Send;
		public override bool Market
		{
			get; set;
		}
		public override dynamic SellPrice
		{
			protected internal get; set;
		}
		public override dynamic BuyPrice
		{
			protected internal get; set;
		}
		public override void OnReceiveDrawChart(object sender, SendConsecutive e)
		{
			if (e.Matrix is null)
			{

				return;
			}
		}
		public override int GetQuoteUnit(int price, bool info) => base.GetQuoteUnit(price, info);
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
			if (Balance is Balance bal && int.TryParse(param[1][0] is '-' ? param[1][1..] : param[1], out int current))
			{
				Current = current;
				bal.Revenue = (current - bal.Purchase) * bal.Quantity;
				bal.Rate = current / (double)bal.Purchase - 1;
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
		public override Queue<Collect> Collection
		{
			get; set;
		}
		public override Dictionary<string, dynamic> OrderNumber
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
		public override dynamic Bid
		{
			get; set;
		}
		protected internal override Stack<double> Short
		{
			get; set;
		}
		protected internal override Stack<double> Long
		{
			get; set;
		}
		protected internal override Stack<double> Trend
		{
			get; set;
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