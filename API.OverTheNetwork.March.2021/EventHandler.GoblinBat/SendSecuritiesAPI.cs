using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ShareInvest.Catalog.Models;
using ShareInvest.Catalog.OpenAPI;

namespace ShareInvest.EventHandler
{
	public class SendSecuritiesAPI : EventArgs
	{
		public SendSecuritiesAPI(int param) => Convey = param;
		public SendSecuritiesAPI(short error) => Convey = error;
		public SendSecuritiesAPI(Codes codes) => Convey = codes;
		public SendSecuritiesAPI(Queue<string[]> hold) => Convey = hold;
		public SendSecuritiesAPI(Tuple<string, string> tuple) => Convey = tuple;
		public SendSecuritiesAPI(Operation operation, string time, string remain) => Convey = new Tuple<Operation, string, string>(operation, time, remain);
		public SendSecuritiesAPI(Dictionary<string, string> param) => Convey = param;
		public SendSecuritiesAPI(string code, Stack<string> stack) => Convey = new Tuple<string, Stack<string>>(code, stack);
		public SendSecuritiesAPI(string message) => Convey = message;
		public SendSecuritiesAPI(string[] accounts) => Convey = accounts;
		public SendSecuritiesAPI(string sEvaluation, string sDeposit, string sAvailable)
		{
			if (long.TryParse(sEvaluation, out long evaluation) && long.TryParse(sDeposit, out long deposit) && long.TryParse(sAvailable, out long available))
				Convey = new Tuple<long, long>(evaluation + deposit, available);
		}
		public SendSecuritiesAPI(string code, string name, string retention, string price, int market)
			=> Convey = new Tuple<string, string, string, string, int>(code, name, retention, price, market);
		public SendSecuritiesAPI(Stack<StringBuilder> stack)
		{
			var dic = new Dictionary<string, Tuple<string, string>>();

			while (stack.Count > 0)
			{
				string[] temp = stack.Pop().ToString().Split(';'), name = temp[3].Split('_');
				string key = Base.rename.FirstOrDefault(o => o.Value.Equals(name[^1])).Key, exists = string.Empty;

				switch (temp[4])
				{
					case "미래에셋대우":
						exists = "미래대우";
						break;

					case "LG디스플레이":
						exists = "LGD";
						break;

					case "SK머티리얼즈":
						exists = "SK머티리얼";
						break;

					case "셀트리온헬스케어":
						exists = "셀트리온헬";
						break;

					case "한화솔루션":
						exists = "한화솔루션";
						break;

					case "SK이노베이션":
						exists = "SK이노베이";
						break;

					default:
						if (string.IsNullOrEmpty(key) == false)
							dic[key] = new Tuple<string, string>(name[^1], temp[5]);

						else if (temp[4].Equals("KOSPI200"))
							dic[Base.rename.First(o => o.Key.StartsWith("101") && o.Key.Length == 8 && o.Key.EndsWith("000")).Key]
								= new Tuple<string, string>(temp[4], temp[5]);

						else if (temp[4].Equals("미니KOSPI200"))
							dic[Base.rename.First(o => o.Key.StartsWith("105") && o.Key.Length == 8 && o.Key.EndsWith("000")).Key]
								= new Tuple<string, string>(temp[4], temp[5]);

						else if (temp[4].Equals("코스닥150"))
							dic[Base.rename.First(o => o.Key.StartsWith("106") && o.Key.Length == 8 && o.Key.EndsWith("000")).Key]
								= new Tuple<string, string>(temp[4], temp[5]);

						else
						{
							key = Base.rename.FirstOrDefault(o => temp[4].StartsWith(o.Value, StringComparison.CurrentCultureIgnoreCase)).Key;

							if (string.IsNullOrEmpty(key) == false)
								dic[key] = new Tuple<string, string>(temp[4], temp[5]);
						}
						continue;
				}
				dic[Base.rename.First(o => o.Value.Equals(exists)).Key] = new Tuple<string, string>(temp[4], temp[5]);
			}
			Convey = dic;
		}
		public SendSecuritiesAPI(string code, string name)
		{
			var temp = name.Split('F')[0].Trim();
			Base.rename[code] = string.IsNullOrEmpty(temp) ? name : temp;
			Convey = new Tuple<string, string, string>(code, name, string.Empty);
		}
		public SendSecuritiesAPI(Tuple<string, string, string> tuple)
		{
			if (tuple.Item1.Length == 8)
				Base.rename[tuple.Item1] = tuple.Item2;

			Convey = tuple;
		}
		public object Convey
		{
			get; private set;
		}
	}
}