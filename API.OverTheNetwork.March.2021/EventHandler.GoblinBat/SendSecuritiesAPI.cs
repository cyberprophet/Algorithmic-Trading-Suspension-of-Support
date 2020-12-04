using System;
using System.Collections.Generic;

using ShareInvest.Catalog.Models;

namespace ShareInvest.EventHandler
{
	public class SendSecuritiesAPI : EventArgs
	{
		public SendSecuritiesAPI(int param) => Convey = param;
		public SendSecuritiesAPI(short error) => Convey = error;
		public SendSecuritiesAPI(Codes codes) => Convey = codes;
		public SendSecuritiesAPI(Queue<string[]> hold) => Convey = hold;
		public SendSecuritiesAPI(Tuple<string, string> tuple) => Convey = tuple;
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
		public object Convey
		{
			get; private set;
		}
	}
}