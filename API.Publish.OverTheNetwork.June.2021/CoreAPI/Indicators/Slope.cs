using System;
using System.Collections.Generic;

using ShareInvest.EventHandler;
using ShareInvest.SecondaryIndicators;

namespace ShareInvest.Indicators
{
	class Slope
	{
		internal Slope(string name)
		{
			this.name = name;
			values = Enum.GetValues(typeof(Interface.KRX.Line));
			Stack = new Stack<int>(0x80);
			Line = new Dictionary<Interface.KRX.Line, int[]>();

			foreach (var length in values)
				Line[(Interface.KRX.Line)length] = new int[(int)length];
		}
		internal void OnReceiveCurrentLocation(object sender, SendConsecutive e)
		{
			Stack.Push(e.Price);
			Date = e.Date[2..];
		}
		internal double[] CalculateTheSlope
		{
			get
			{
				var count = 0;
				var response = new double[values.Length];

				while (Stack.TryPop(out int price))
				{
					foreach (var kv in Line)
						if (kv.Value.Length > count)
							kv.Value[kv.Value.Length - count - 1] = price;

					count++;
				}
				count = 0;

				foreach (var kv in Line)
					try
					{
						if (kv.Value is not null && Array.Exists(kv.Value, o => double.IsSubnormal(o) || o == 0) is false)
						{
							var temp = new int[(int)kv.Key];

							for (int i = 0; i < (int)kv.Key; i++)
								temp[i] = i + 1;

							Normalization x = new(temp), y = new(kv.Value);
							double[] xs = new double[(int)kv.Key], ys = new double[kv.Value.Length];

							for (int i = 0; i < (int)kv.Key; i++)
							{
								xs[i] = x.Normalize(temp[i]);
								ys[i] = y.Normalize(kv.Value[i]);
							}
							response[count] = new LinearRegressionLine(xs, ys).Slope;
						}
						else
							response[count] = double.NaN;
					}
					catch (Exception ex)
					{
						Base.SendMessage(GetType(), name, ex.StackTrace);
					}
					finally
					{
						count++;
						Base.SendMessage(GetType(), name, response[count - 1]);
					}
				return response;
			}
		}
		internal string Date
		{
			get; private set;
		}
		Stack<int> Stack
		{
			get;
		}
		Dictionary<Interface.KRX.Line, int[]> Line
		{
			get;
		}
		readonly Array values;
		readonly string name;
	}
}