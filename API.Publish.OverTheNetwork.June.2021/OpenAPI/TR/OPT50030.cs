using System;
using System.Collections.Generic;
using System.Linq;

using AxKHOpenAPILib;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
	class OPT50030 : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
	{
		internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
		{
			if (int.TryParse(e.sPrevNext, out int next))
			{
				var temp = base.OnReceiveTrData(null, op_mutiple, e);
				var tr = Connect.GetInstance().TR.First(o => o.GetType().Name[1..].Equals(e.sTrCode[1..]) && o.RQName.Equals(e.sRQName));
				string date = DateTime.Now.AddMonths(-1).ToString(Base.LongDateFormat), code = Value.Split(';')[0];

				while (temp.Item2.TryDequeue(out string[] param))
				{
					if (param[2].CompareTo(date) > 0)
						confirm.Enqueue(new Stocks
						{
							Code = code,
							Date = param[2][2..],
							Price = param[4],
							Retention = param[5]
						});
					else if (param[2].CompareTo(date) < 0)
						next = 0;
				}
				if (next > 0)
				{
					tr.PrevNext = next;
					Connect.GetInstance().InputValueRqData(tr);
				}
				else
					Send?.Invoke(this, new SendSecuritiesAPI(code, confirm));
			}
		}
		internal override string ID => id;
		internal override string Value
		{
			get; set;
		}
		internal override string RQName
		{
			get; set;
		} = rq_name;
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
		const string code = "OPT50030";
		const string id = "종목코드;기준일자";
		const string rq_name = "선물옵션일차트요청";
		readonly string[] op_mutiple = { "현재가", "누적거래량", "일자", "시가", "고가", "저가", "수정주가이벤트", "전일종가" };
		readonly Queue<Stocks> confirm = new Queue<Stocks>();
		public override event EventHandler<SendSecuritiesAPI> Send;
	}
}