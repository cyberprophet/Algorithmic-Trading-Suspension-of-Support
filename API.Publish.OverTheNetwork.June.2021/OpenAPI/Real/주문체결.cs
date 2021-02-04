using System;
using System.Collections.Generic;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
	class 주문체결 : Chejan
	{
		public override event EventHandler<SendSecuritiesAPI> Send;
		protected internal override AxKHOpenAPI API
		{
			get; set;
		}
		protected internal override string Identity
		{
			get; set;
		}
		internal override void OnReceiveChejanData(_DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
		{
			var conclusion = new Dictionary<int, string>();

			foreach (int fid in Enum.GetValues(typeof(Conclusion)))
				conclusion[fid] = API.GetChejanData(fid);

			var code = conclusion[(int)Conclusion.종목코드_업종코드];

			if (Connect.GetInstance().StocksHeld.TryGetValue(code[0] is 'A' ? code[1..] : code, out Analysis analysis))
			{
				if (analysis.OrderNumber is null)
					analysis.OrderNumber = new Dictionary<string, dynamic>();

				Send?.Invoke(this, new SendSecuritiesAPI(analysis.OnReceiveConclusion(conclusion)));
			}
		}
	}
}