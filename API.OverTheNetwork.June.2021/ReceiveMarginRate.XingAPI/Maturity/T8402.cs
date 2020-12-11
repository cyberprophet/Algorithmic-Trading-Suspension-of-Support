using System;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
	class T8402 : Query
	{
		public override event EventHandler<SendSecuritiesAPI> Send;
		Codes Model
		{
			get; set;
		}
		internal int Delay
		{
			get; private set;
		}
		internal T8402 QueryExcute(Codes model)
		{
			Model = model;
			QueryExcute();
			Delay = 0x65;

			return this;
		}
		internal override void QueryExcute()
		{
			if (LoadFromResFile(GetResFileName(GetType().Name)))
			{
				foreach (var param in GetInBlocks(GetType().Name))
					SetFieldData(param.Block, param.Field, param.Occurs, param.Data ?? Model.Code);

				SendErrorMessage(GetType().Name, Request(false));
			}
		}
		protected internal override void OnReceiveData(string szTrCode)
		{
			var enumerable = GetOutBlocks();
			int i = 0, count = enumerable.Count - 1;
			string[] temp = new string[count];

			while (enumerable.Count > 0)
			{
				var param = enumerable.Dequeue();

				if (count > enumerable.Count)
					temp[i++] = GetFieldData(param.Block, param.Field, 0);
			}
			var refresh = new Codes
			{
				Code = Model.Code,
				Name = temp[0x33].Trim(),
				MarginRate = Model.MarginRate,
				MaturityMarketCap = temp[0x16][2..],
				Price = temp[1]
			};
			Delay = 0x3E8 / GetTRCountPerSec(szTrCode) + 1;
			Send?.Invoke(this, new SendSecuritiesAPI(refresh));

			if (double.TryParse(temp[0x3E], out double transactionMutiplier) && transactionMutiplier != 0xA)
				Base.SendMessage(refresh.Name, refresh.MaturityMarketCap, GetType());
		}
		protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
			=> base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
	}
}