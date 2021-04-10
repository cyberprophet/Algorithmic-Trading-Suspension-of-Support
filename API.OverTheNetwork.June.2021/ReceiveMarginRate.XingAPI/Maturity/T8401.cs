using System;

using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
	class T8401 : Query
	{
		protected internal override void OnReceiveData(string szTrCode)
		{
			var enumerable = GetOutBlocks();
			var temp = string.Empty;
			string[] code = null, name = null;

			while (enumerable.Count > 0)
			{
				var param = enumerable.Dequeue();
				var length = GetBlockCount(param.Block);

				switch (enumerable.Count)
				{
					case 2:
						code = new string[length];
						break;

					case 3:
						name = new string[length];
						break;
				}
				for (int i = 0; i < length; i++)
					switch (enumerable.Count)
					{
						case 2:
							code[i] = GetFieldData(param.Block, param.Field, i);
							break;

						case 3:
							name[i] = GetFieldData(param.Block, param.Field, i);
							break;
					}
			}
			for (int i = 0; i < code.Length; i++)
			{
				var distinct = string.Concat(code[i].Substring(0, 3), code[i][5..]);

				if ((string.IsNullOrEmpty(temp) || temp.Equals(distinct) == false) && code[i].StartsWith("1"))
				{
					temp = distinct;
					Send?.Invoke(this, new SendSecuritiesAPI(code[i], name[i]));
				}
			}
		}
		protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage) => base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
		internal override void QueryExcute()
		{
			if (LoadFromResFile(GetResFileName(GetType().Name)))
			{
				foreach (var param in GetInBlocks(GetType().Name))
					SetFieldData(param.Block, param.Field, param.Occurs, param.Data);

				SendErrorMessage(GetType().Name, Request(false));
			}
		}
		public override event EventHandler<SendSecuritiesAPI> Send;
	}
}