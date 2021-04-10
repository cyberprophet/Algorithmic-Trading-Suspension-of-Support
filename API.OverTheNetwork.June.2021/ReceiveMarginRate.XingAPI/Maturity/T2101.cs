using System;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
	class T2101 : Query
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
		internal T2101 QueryExcute(Codes model)
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
			Delay = 0x3E8 / GetTRCountPerSec(szTrCode) + 1;
			Send?.Invoke(this, new SendSecuritiesAPI(new Codes
			{
				Code = temp[0x3C],
				Name = Model.Code[0] == '1' ? temp[0].Split('F')[0].Trim() : temp[0].Remove(temp[0].Length - 2, 2),
				MarginRate = Model.MarginRate,
				MaturityMarketCap = temp[0x17][2..],
				Price = temp[1]
			}));
		}
		protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage) => base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
	}
}