using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ShareInvest.Catalog.Models;
using ShareInvest.Client;
using ShareInvest.EventHandler;
using ShareInvest.XingAPI.Catalog;

using XA_SESSIONLib;

namespace ShareInvest.XingAPI
{
	class Connect : XASessionClass
	{
		internal Stack<Codes> Stack
		{
			get; private set;
		}
		internal Connect(ShareInvest.Catalog.XingAPI.Privacies privacy)
		{
			if (ConnectServer(hts, 0x4E21) && Login(privacy.Identity, privacy.Password, privacy.Certificate, 0, string.IsNullOrEmpty(privacy.Certificate) == false) && IsLoadAPI())
			{
				_IXASessionEvents_Event_Login += OnEventConnect;
				Disconnect += Dispose;
				key = privacy.Key;
			}
			else
				Dispose();
		}
		internal void StartProgress()
		{
			while (Stack.Count > 0)
			{
				var model = Stack.Pop();
				int milliseconds;

				if (model.Code[1] is '0')
				{
					var ctor = new T2101();
					ctor.Send += OnReceiveSecuritiesAPI;
					milliseconds = ctor.QueryExcute(model).Delay;
				}
				else
				{
					var ctor = new T8402();
					ctor.Send += OnReceiveSecuritiesAPI;
					milliseconds = ctor.QueryExcute(model).Delay;
				}
				Base.SendMessage(GetType(), model.Name, Stack.Count);
				Thread.Sleep(milliseconds);
			}
		}
		internal void Dispose()
		{
			try
			{
				DisconnectServer();
				GC.Collect();
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
			}
		}
		void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e)
		{
			switch (sender)
			{
				case T2101:
					(sender as T2101).Send -= OnReceiveSecuritiesAPI;
					break;

				case T8402:
					(sender as T8402).Send -= OnReceiveSecuritiesAPI;
					break;

				case T8435:
				case T8401:
				case T8432:
					if (e.Convey is Tuple<string, string, string> code)
					{
						Models[code.Item1] = new Codes
						{
							Code = code.Item1,
							Name = code.Item2,
							Price = code.Item3
						};
						Base.SendMessage(sender.GetType(), code.Item2, Models.Count);
					}
					return;

				case MMDAQ91200:
					Stack = new Stack<Codes>();

					foreach (var kv in e.Convey as Dictionary<string, Tuple<string, string>>)
						if (Models.TryGetValue(kv.Key, out Codes info) && double.TryParse(kv.Value.Item2, out double rate))
							Stack.Push(new Codes
							{
								Code = info.Code,
								Name = kv.Value.Item1,
								MarginRate = rate * 1e-2,
								Price = info.Price,
								MaturityMarketCap = string.Empty
							});
					Base.SendMessage(sender.GetType(), GetType().Name, Stack.Count);
					return;
			}
			if (e.Convey is Codes model)
				new Task(async () =>
				{
					var status = await Client.PutContextAsync(model) is int code ? code : int.MinValue;
					Base.SendMessage(GetType(), model.Name, status);
					Base.SendMessage(GetType(), model.Name, Count++);

				}).Start();
		}
		void OnEventConnect(string szCode, string szMsg)
		{
			if (szCode.Equals(code) && szMsg.Equals(msg) && IsConnected())
			{
				var account = new string[GetAccountListCount()];

				for (int i = 0; i < account.Length; i++)
					account[i] = GetAccountList(i);

				Models = new Dictionary<string, Codes>();
				Client = new Maturity(new string[] { key, GetType().Name });

				foreach (var ctor in new Query[] { new T8435(), new T8401(), new T8432(), new MMDAQ91200() })
				{
					ctor.Send += OnReceiveSecuritiesAPI;
					ctor.QueryExcute();
				}
			}
		}
		Maturity Client
		{
			get; set;
		}
		Dictionary<string, Codes> Models
		{
			get; set;
		}
		uint Count
		{
			get; set;
		}
		readonly string key;
		const string code = "0000";
		const string msg = "로그인 성공";
		const string hts = "hts.ebestsec.co.kr";
	}
}