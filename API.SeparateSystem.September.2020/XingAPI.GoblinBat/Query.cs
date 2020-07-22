using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using XA_DATASETLib;

namespace ShareInvest.XingAPI
{
    abstract class Query : XAQueryClass
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
            var secret = Secrecy.GetData(name);
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
                    Data = secret?[i++]
                });
                if (secret == null || secret.Length == i)
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
                SendMessage(name, param);

                if (Secrecy.ErrorMessage.Equals(param))
                    Connect.GetInstance().Dispose();
            }
        }
        protected internal TimeSpan WaitForTheLimitTime(int limit) => TimeSpan.FromSeconds(limit);
        protected internal virtual void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            if (bIsSystemError == false && int.TryParse(nMessageCode, out int code) && code > 999)
            {
                if (nMessageCode.Equals(rCancel))
                {

                }
                else if (Array.Exists(exclusion, o => o.Equals(code)))
                    SendMessage(nMessageCode, szMessage);
            }
            else
                SendMessage(nMessageCode, szMessage);
        }
        protected internal abstract void OnReceiveData(string szTrCode);
        [Conditional("DEBUG")]
        void SendMessage(string code, string message) => new Task(() => Console.WriteLine(code + "\t" + message)).Start();
        readonly string[] exclusion = { margin, accept, cancel, correction, impossible };
        const string record = "레코드명:";
        const string separator = "No,한글명,필드명,영문명,";
        const string cancel = "02661";
        const string correction = "02667";
        const string impossible = "03590";
        const string accept = "08683";
        const string margin = "02752";
        const string rCancel = "03416";
    }
    enum JongCHK
    {
        권리락 = 1,
        배당락 = 2,
        액면분할 = 4,
        액면병합 = 8,
        주식병합 = 0x10,
        기업분할 = 0x20,
        관리 = 0x80,
        투자경고 = 0x100,
        거래정지 = 0x200,
        기준가조정 = 0x1000,
        우선주 = 0x4000,
        CB발동예고 = 0x8000,
        중간배당락 = 0x10000,
        권리중간배당락 = 0x20000,
        시간범위연장_Random_end = 0x40000,
        종가범위연장_Random_end = 0x80000,
        증거금100Percent = 0x200000,
        ETF종목 = 0x800000,
        정리매매종목 = 0x1000000,
        뮤추얼펀드 = 0x8000000,
        불성실공시종목 = 0x7FFFFFFF
    }
}