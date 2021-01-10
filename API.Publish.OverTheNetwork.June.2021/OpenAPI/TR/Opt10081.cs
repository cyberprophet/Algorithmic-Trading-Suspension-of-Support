using System;
using System.Collections.Generic;
using System.Linq;

using AxKHOpenAPILib;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
	class Opt10081 : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
	{
		protected internal override (string[], Queue<string[]>) OnReceiveTrData(string[] single, string[] multi, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
		{
			var sTemp = single is null ? null : new string[single.Length];

			if (single is not null)
				for (int i = 0; i < single.Length; i++)
					sTemp[i] = API.GetCommData(e.sTrCode, e.sRQName, 0, single[i]).Trim();

			if (multi is not null)
			{
				var temp = API.GetCommDataEx(e.sTrCode, e.sRQName);

				if (temp is not null)
				{
					int x, y, lx = ((object[,])temp).GetUpperBound(0), ly = ((object[,])temp).GetUpperBound(1);
					var catalog = new Queue<string[]>();

					for (x = 0; x <= lx; x++)
					{
						var str = new string[ly + 1];

						for (y = 0; y <= ly; y++)
							str[y] = ((string)((object[,])temp)[x, y]).Trim();

						if (string.IsNullOrEmpty(e.sRQName) is false && (e.sRQName.Equals(rqName) || string.Compare(str[4], e.sRQName) < 0))
							catalog.Enqueue(str);
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
				var temp = base.OnReceiveTrData(opSingle, opMutiple, e);
				var tr = Connect.GetInstance().TR.First(o => o.GetType().Name[1..].Equals(e.sTrCode[1..]) && o.RQName.Equals(e.sRQName));
				var date = DateTime.Now;

				while (temp.Item2.TryDequeue(out string[] param))
				{
					if (param[4].CompareTo(date.AddYears(-1).ToString(Base.LongDateFormat)) > 0)
					{
						if (string.IsNullOrEmpty(param[8]) is false && int.TryParse(param[8], out int revise) && param[9].Equals("0.00") is false)
							storage.Push(new RevisedStockPrice
							{
								Code = temp.Item1[0],
								Name = API.GetMasterCodeName(temp.Item1[0]),
								Revise = Enum.GetName(typeof(ShareInvest.Catalog.OpenAPI.RevisedStockPrice), revise),
								Rate = param[9],
								Date = param[4][2..],
								Price = param[1]
							});
						if (param[4].CompareTo(date.AddMonths(-3).ToString(Base.LongDateFormat)) > 0)
							confirm.Enqueue(new Stocks
							{
								Code = temp.Item1[0],
								Date = param[4][2..],
								Price = param[6],
								Retention = param[7],
								Volume = int.TryParse(param[9].Replace(".", string.Empty), out int rate) ? rate : 0
							});
					}
					else if (param[4].CompareTo(date.AddYears(-1).ToString(Base.LongDateFormat)) < 0)
						next = 0;
				}
				if (next > 0)
				{
					tr.PrevNext = next;
					Connect.GetInstance().InputValueRqData(tr);
				}
				else
					Send?.Invoke(this, new SendSecuritiesAPI(temp.Item1[0], storage, confirm));
			}
		}
		internal override string ID => id;
		internal override string Value
		{
			get => string.Concat(Code, ";1");
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
		readonly Queue<Stocks> confirm = new Queue<Stocks>();
		readonly Stack<RevisedStockPrice> storage = new Stack<RevisedStockPrice>();
		readonly string[] opSingle = { "종목코드" };
		readonly string[] opMutiple = { "종목코드", "현재가", "거래량", "거래대금", "일자", "시가", "고가", "저가", "수정주가구분", "수정비율", "대업종구분", "소업종구분", "종목정보", "수정주가이벤트", "전일종가" };
		const string code = "opt10081";
		const string id = "종목코드;기준일자;수정주가구분";
		const string rqName = "주식일봉차트조회요청";
		string Code
		{
			get; set;
		}
		public override event EventHandler<SendSecuritiesAPI> Send;
	}
}