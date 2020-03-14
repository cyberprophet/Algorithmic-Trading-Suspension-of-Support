using System;
using System.Collections.Generic;
using ShareInvest.Message;
using XA_DATASETLib;

namespace ShareInvest.XingAPI
{
    internal class Query : XAQueryClass
    {
        internal protected Query()
        {
            ReceiveData += OnReceiveData;
            ReceiveMessage += OnReceiveMessage;
        }
        protected Queue<InBlock> GetInBlocks(string name)
        {
            string block = string.Empty;
            var queue = new Queue<InBlock>();
            var secret = new Secret().GetData(name);
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

                var temp = str.Split(',');
                queue.Enqueue(new InBlock
                {
                    Block = block,
                    Field = temp[2],
                    Occurs = 0,
                    Data = secret[i++]
                });
                if (secret.Length == i)
                    break;
            }
            return queue;
        }
        protected Queue<OutBlock> GetOutBlocks()
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
        protected void SendErrorMessage(int error)
        {
            if (error < 0)
            {
                var param = GetErrorMessage(error);
                new ExceptionMessage(param);

                if (Array.Exists(new Secret().ErrorMessage, o => o.Equals(param)))
                    ConnectAPI.GetInstance(string.Empty).Dispose();
            }
        }
        protected virtual void OnReceiveData(string szTrCode)
        {
            Console.WriteLine(szTrCode);
        }
        protected virtual void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            if (bIsSystemError)
            {
                new ExceptionMessage(szMessage, nMessageCode);

                return;
            }
            if (int.TryParse(nMessageCode, out int code) && code > 999)
                new ExceptionMessage(szMessage, nMessageCode);
        }
        private const string record = "레코드명:";
        private const string separator = "No,한글명,필드명,영문명,";
    }
}