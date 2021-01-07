using System;
using System.Collections.Generic;
using System.Linq;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
	class Opt50028 : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
	{
		protected internal override (string[], Queue<string[]>) OnReceiveTrData(string[] single, string[] multi, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
		{
			var sTemp = single != null ? new string[single.Length] : null;

			if (single != null)
				for (int i = 0; i < single.Length; i++)
					sTemp[i] = API.GetCommData(e.sTrCode, e.sRQName, 0, single[i]);

			if (multi != null)
			{
				var temp = API.GetCommDataEx(e.sTrCode, e.sRQName);

				if (temp != null)
				{
					int x, y, lx = ((object[,])temp).GetUpperBound(0), ly = ((object[,])temp).GetUpperBound(1);
					var catalog = new Queue<string[]>();

					for (x = 0; x <= lx; x++)
					{
						var str = new string[ly + 1];

						for (y = 0; y <= ly; y++)
							str[y] = (string)((object[,])temp)[x, y];

						if (string.IsNullOrEmpty(e.sRQName) == false && (string.Compare(str[2][2..], e.sRQName) > 0 || e.sRQName.Equals(rqName)))
							catalog.Enqueue(str);

						else
							sTemp[1] = e.sRQName;
					}
					return (sTemp, catalog);
				}
			}
			return (sTemp, null);
		}
		internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
		{
			if (int.TryParse(e.sPrevNext, out int next))
			{
				var temp = OnReceiveTrData(opSingle, opMutiple, e);
				var tr = Connect.GetInstance().TR.First(o => o.GetType().Name[1..].Equals(e.sTrCode[1..]) && o.RQName.Equals(e.sRQName));

				while (temp.Item2 != null && temp.Item2.Count > 0)
				{
					var param = temp.Item2.Dequeue();
					storage.Push(string.Concat(param[2][2..], ";", param[0], ";", param[1]));
				}
				if (next > 0 && temp.Item1[1].Equals(e.sRQName) == false)
				{
					tr.PrevNext = next;
					Base.SendMessage(GetType(), e.sScrNo, e.sRQName);
					Connect.GetInstance().InputValueRqData(tr);
				}
				else
					Send?.Invoke(this, new SendSecuritiesAPI(temp.Item1[0].Trim(), storage));
			}
		}
		internal override string ID => id;
		internal override string Value
		{
			get => string.Concat(Code, tick);
			set => Code = value;
		}
		internal override string RQName
		{
			get; set;
		} = rqName;
		internal override string TrCode => code;
		internal override int PrevNext
		{
			get; set;
		}
		internal override string ScreenNo => LookupScreenNo;
		internal override AxKHOpenAPI API
		{
			get; set;
		}
		string Code
		{
			get; set;
		}
		readonly Stack<string> storage = new Stack<string>();
		readonly string[] opSingle = { "종목코드", "마지막틱갯수" };
		readonly string[] opMutiple = { "현재가", "거래량", "체결시간", "시가", "고가", "저가", "전일종가" };
		const string code = "opt50028";
		const string id = "종목코드;시간단위";
		const string tick = ";1";
		const string rqName = "선물틱차트요청";
		public override event EventHandler<SendSecuritiesAPI> Send;
	}
}