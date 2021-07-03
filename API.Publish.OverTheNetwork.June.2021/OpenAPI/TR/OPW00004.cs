using AxKHOpenAPILib;

using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

using System;
using System.Collections.Generic;

namespace ShareInvest.OpenAPI.Catalog
{
	class OPW00004 : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
	{
		internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
		{
			var temp = base.OnReceiveTrData(single, multiple, e);

			if (temp.Item1 is not null)
				Send?.Invoke(this, new SendSecuritiesAPI(single, temp.Item1));

			if (temp.Item2 is not null && temp.Item2.Count > 0)
			{
				var queue = new Queue<ShareInvest.Catalog.OpenAPI.OPW00004>();

				while (temp.Item2.TryDequeue(out string[] dequeue))
					if (int.TryParse(dequeue[0xE], out int sales) && int.TryParse(dequeue[0xD], out int pur) && int.TryParse(dequeue[0xC], out int pre_sales) && int.TryParse(dequeue[0xB], out int pre_purchase) && long.TryParse(dequeue[0xA], out long balance) && long.TryParse(dequeue[9], out long purchase) && double.TryParse(dequeue[7].Insert(dequeue[7].Length - 6, "."), out double rate) && long.TryParse(dequeue[6], out long amount) && long.TryParse(dequeue[5], out long evaluation) && int.TryParse(dequeue[4], out int current) && int.TryParse(dequeue[3], out int average) && int.TryParse(dequeue[2], out int quantity))
						queue.Enqueue(new ShareInvest.Catalog.OpenAPI.OPW00004
						{
							Account = Value.Split(';')[0],
							Code = dequeue[0],
							Name = dequeue[1],
							Quantity = quantity,
							Average = average,
							Current = current,
							Evaluation = evaluation,
							Amount = amount,
							Rate = rate,
							Loan = dequeue[8],
							Purchase = purchase,
							Balance = balance,
							PreviousPurchaseQuantity = pre_purchase,
							PreviousSalesQuantity = pre_sales,
							PurchaseQuantity = pur,
							SalesQuantity = sales
						});
				if (queue.Count > 0)
					Send?.Invoke(this, new SendSecuritiesAPI(queue));
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
		} = name;
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
		const string code = "OPW00004";
		const string name = "계좌평가현황요청";
		const string id = "계좌번호;비밀번호;상장폐지조회구분;비밀번호입력매체구분";
		readonly string[] single = { "계좌명", "지점명", "예수금", "D+2추정예수금", "유가잔고평가액", "예탁자산평가액", "총매입금액", "추정예탁자산", "매도담보대출금", "당일투자원금", "당월투자원금", "누적투자원금", "당일투자손익", "당월투자손익", "누적투자손익", "당일손익율", "당월손익율", "누적손익율", "출력건수" };
		readonly string[] multiple = { "종목코드", "종목명", "보유수량", "평균단가", "현재가", "평가금액", "손익금액", "손익율", "대출일", "매입금액", "결제잔고", "전일매수수량", "전일매도수량", "금일매수수량", "금일매도수량" };
		public override event EventHandler<SendSecuritiesAPI> Send;
	}
}