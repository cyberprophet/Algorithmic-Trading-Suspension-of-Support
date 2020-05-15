using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using ShareInvest.Message;
using XA_DATASETLib;

namespace ShareInvest.XingAPI
{
    class Query : XAQueryClass
    {
        protected internal Query()
        {
            ReceiveData += OnReceiveData;
            ReceiveMessage += OnReceiveMessage;
        }
        protected internal Queue<InBlock> GetInBlocks(string[] order)
        {
            string block = string.Empty;
            var queue = new Queue<InBlock>();
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
                    Data = order[i++]
                });
                if (order.Length == i)
                    break;
            }
            return queue;
        }
        protected internal Queue<InBlock> GetInBlocks(string name)
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
                new ExceptionMessage(param, name);

                if (Array.Exists(new Secret().ErrorMessage, o => o.Equals(param)))
                    API.Dispose();
            }
        }
        protected internal virtual void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            if (nMessageCode.Equals(impossible))
            {
                var api = API.querys[1];
                Thread.Sleep(1000 / GetTRCountPerSec(api.GetType().Name));
                api.QueryExcute();
                API.OnReceiveBalance = true;

                return;
            }
            if (bIsSystemError)
            {
                new ExceptionMessage(szMessage, nMessageCode);

                return;
            }
            if (int.TryParse(nMessageCode, out int code) && code > 999)
            {
                if (Array.Exists(exclusion, o => o.Equals(nMessageCode)) && TimerBox.Show(string.Concat(szMessage, new Secret().Message), nMessageCode, MessageBoxButtons.YesNo, MessageBoxIcon.Question, nMessageCode.Equals(rCancel) ? MessageBoxDefaultButton.Button1 : MessageBoxDefaultButton.Button2, 5175).Equals(DialogResult.Yes))
                    API.OnReceiveBalance = true;

                new ExceptionMessage(szMessage, nMessageCode);
            }
        }
        protected internal virtual void OnReceiveData(string szTrCode) => Console.WriteLine(szTrCode);
        protected internal ConnectAPI API => ConnectAPI.GetInstance();
        readonly string[] exclusion = { margin, rCancel, accept, cancel, correction };
        const string record = "레코드명:";
        const string separator = "No,한글명,필드명,영문명,";
        const string cancel = "02661";
        const string correction = "02667";
        const string impossible = "03590";
        const string accept = "08683";
        const string margin = "02752";
        const string rCancel = "03416";
    }
}