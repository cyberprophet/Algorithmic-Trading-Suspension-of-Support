using System;
using System.Collections.Generic;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
	class 파생잔고 : Chejan
	{
		public override event EventHandler<SendSecuritiesAPI> Send;
		protected internal override AxKHOpenAPI API
		{
			get; set;
		}
		internal override void OnReceiveChejanData(_DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
		{
			var conclusion = new Dictionary<string, string>();

			foreach (var fid in Enum.GetValues(typeof(Derivatives)))
				conclusion[fid.ToString()] = API.GetChejanData((int)fid);

			Send?.Invoke(this, new SendSecuritiesAPI(conclusion));
		}
	}
}