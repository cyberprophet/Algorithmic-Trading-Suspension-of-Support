using System;
using System.Collections.Generic;

using ShareInvest.Catalog.XingAPI;
using ShareInvest.EventHandler;

using XA_DATASETLib;

namespace ShareInvest.XingAPI
{
	abstract class Query : XAQueryClass
	{
		public abstract event EventHandler<SendSecuritiesAPI> Send;
		internal abstract void QueryExcute();
		protected internal static string GetResFileName(string file) => string.Concat(path, file, res);
		protected internal Query()
		{
			ReceiveData += OnReceiveData;
			ReceiveMessage += OnReceiveMessage;
		}
		protected internal Queue<InBlock> GetInBlocks(string name)
		{
			string block = string.Empty;
			var queue = new Queue<InBlock>();
			var secret = Data[name];
			int i = 0;

			foreach (var str in GetResData().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (str.Contains(record) && str.Contains("InBlock"))
				{
					block = str.Replace("*", string.Empty).Replace(record, string.Empty).Trim();

					continue;
				}
				else if (str.Contains(record) && str.Contains("OutBlock"))
					break;

				else if (str.Contains(separator))
					continue;

				queue.Enqueue(new InBlock
				{
					Block = block,
					Field = str.Split(',')[2],
					Occurs = 0,
					Data = secret?[i++]
				});
				if (secret == null || secret.Length == i)
					break;
			}
			return queue;
		}
		protected internal Queue<InBlock> GetInBlocks(string name, string gubun)
		{
			SetData(gubun);

			return GetInBlocks(name);
		}
		protected internal void SetData(string gubun) => Convert = new string[] { gubun };
		protected internal Queue<OutBlock> GetOutBlocks()
		{
			string block = string.Empty;
			var queue = new Queue<OutBlock>();

			foreach (var str in GetResData().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (str.Contains(record) && str.Contains("InBlock"))
					continue;

				else if (str.Contains(record) && str.Contains("OutBlock"))
				{
					block = str.Replace("*", string.Empty).Replace(record, string.Empty).Trim();

					continue;
				}
				else if (str.Contains(separator))
					continue;

				var temp = str.Split(',');
				queue.Enqueue(new OutBlock
				{
					Block = block,
					Field = temp[2]
				});
			}
			return queue;
		}
		protected internal void SendErrorMessage(string name, int error)
		{
			if (error < 0)
			{
				var param = GetErrorMessage(error);
				Base.SendMessage(GetType(), name, param);
			}
		}
		protected internal virtual void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
		{
			if (bIsSystemError == false && int.TryParse(nMessageCode, out int code) && code > 999)
				Base.SendMessage(GetType(), nMessageCode, szMessage);
		}
		protected internal abstract void OnReceiveData(string szTrCode);
		Dictionary<string, string[]> Data => new() { { t2101, null }, { t8402, null }, { t8435, Convert }, { t8401, new string[] { rec } }, { t8432, new string[] { rec } }, { mmdaq91200, new string[] { rec } } };
		string[] Convert
		{
			get; set;
		}
		const string record = "레코드명:";
		const string separator = "No,한글명,필드명,영문명,";
		const string path = @"C:\eBEST\xingAPI\Res\";
		const string res = ".res";
		const string rec = "1";
		const string mmdaq91200 = "MMDAQ91200";
		const string t8435 = "T8435";
		const string t8432 = "T8432";
		const string t8402 = "T8402";
		const string t8401 = "T8401";
		const string t2101 = "T2101";
	}
}