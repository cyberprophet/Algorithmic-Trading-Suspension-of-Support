using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ShareInvest.Catalog.XingAPI;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
	class MMDAQ91200 : Query
	{
		public override event EventHandler<SendSecuritiesAPI> Send;
		internal override void QueryExcute()
		{
			if (LoadFromResFile(GetResFileName(GetType().Name)))
			{
				foreach (var param in GetInBlocks(GetType().Name))
				{
					SetFieldData(param.Block, param.Field, param.Occurs, param.Data);
					InBlock = new InBlock
					{
						Block = param.Block,
						Field = param.Field,
						Occurs = param.Occurs,
						Data = param.Data
					};
				}
				SendErrorMessage(GetType().Name, Request(false));
			}
			Stack = new Stack<StringBuilder>();
		}
		protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
			=> base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
		protected internal override void OnReceiveData(string szTrCode)
		{
			var enumerable = GetOutBlocks();
			var list = new List<StringBuilder>();

			while (enumerable.Count > 0)
			{
				var param = enumerable.Dequeue();

				if (enumerable.Count < 9)
					for (int i = 0; i < GetBlockCount(param.Block); i++)
					{
						if (enumerable.Count == 8)
							list.Add(new StringBuilder());

						list[i] = list[i].Append(GetFieldData(param.Block, param.Field, i)).Append(';');
					}
			}
			foreach (var sb in list)
				Stack.Push(sb);

			if (IsNext)
				new Task(() =>
				{
					Thread.Sleep(0x3E8 / GetTRCountPerSec(szTrCode));
					SetFieldData(InBlock.Block, InBlock.Field, InBlock.Occurs, InBlock.Data);
					SendErrorMessage(GetType().Name, Request(IsNext));
				}).Start();
			else
				Send?.Invoke(this, new SendSecuritiesAPI(Stack));
		}
		InBlock InBlock
		{
			get; set;
		}
		Stack<StringBuilder> Stack
		{
			get; set;
		}
	}
}