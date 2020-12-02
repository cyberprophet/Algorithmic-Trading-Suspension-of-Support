using System;

using ShareInvest.Catalog.Strategics;

namespace ShareInvest.EventHandler
{
	public class SendConsecutive : EventArgs
	{
		public string Date
		{
			get; private set;
		}
		public dynamic Price
		{
			get; private set;
		}
		public int Volume
		{
			get; private set;
		}
		public object Matrix
		{
			get; private set;
		}
		public SendConsecutive(Charts chart)
		{
			var str = chart.Price[0] == '-' ? chart.Price[1..] : chart.Price;
			Date = Base.CheckTheSAT(chart.Date);
			Volume = chart.Volume;

			if (int.TryParse(str, out int sPrice))
				Price = sPrice;

			else if (double.TryParse(str, out double fPrice))
				Price = fPrice;
		}
		public SendConsecutive(string date, int price, int volume)
		{
			Date = Base.CheckTheSAT(date);
			Price = price;
			Volume = volume;
		}
		public SendConsecutive(string[] param) => Matrix = param;
	}
}