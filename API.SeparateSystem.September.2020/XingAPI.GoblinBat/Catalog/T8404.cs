using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ShareInvest.Catalog;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class T8404 : Query, ICharts<SendSecuritiesAPI>
    {
        public void QueryExcute(IRetention retention)
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
            {
                InBlock = new HashSet<InBlock>();

                foreach (var param in GetInBlocks(GetType().Name))
                    if (InBlock.Add(new InBlock
                    {
                        Block = param.Block,
                        Field = param.Field,
                        Occurs = param.Occurs,
                        Data = param.Data ?? retention.Code
                    }))
                        SetFieldData(param.Block, param.Field, param.Occurs, param.Data ?? retention.Code);

                SendErrorMessage(GetType().Name, Request(false));
            }
            Charts = new Stack<string>();

            if (retention is Retention tention)
                Retention = tention;
        }
        protected internal override void OnReceiveData(string szTrCode)
        {
            var enumerable = GetOutBlocks();
            var list = new List<string[]>();

            while (enumerable.Count > 0)
            {
                var param = enumerable.Dequeue();

                switch (enumerable.Count)
                {
                    case int count when count > 0x17:
                        var block = InBlock.First(o => o.Field.Equals(param.Field));
                        SetFieldData(block.Block, block.Field, block.Occurs, block.Data);
                        continue;

                    case 0x17:
                        continue;

                    case 0x16:
                        var data = GetFieldData(param.Block, param.Field, 0);
                        var refresh = InBlock.First(o => o.Field.Equals(param.Field));
                        SetFieldData(refresh.Block, refresh.Field, refresh.Occurs, data);
                        continue;

                    default:
                        var bCount = GetBlockCount(param.Block);
                        var array = new string[bCount];

                        for (int i = 0; i < bCount; i++)
                            array[i] = GetFieldData(param.Block, param.Field, i);

                        list.Add(array);
                        break;
                }
            }
            var span = WaitForTheLimitTime(GetTRCountRequest(szTrCode));
            SendMessage(span);

            if (span.TotalSeconds > 0xC5 && span.TotalSeconds < 0xC8)
                Delay.Milliseconds = (int)span.TotalMilliseconds;

            else
                Delay.Milliseconds = 0x3E8 / GetTRCountPerSec(szTrCode);

            for (int i = list[0].Length - 1; i >= 0; i--)
            {
                var temp = string.Concat(list[0][i].Substring(2), list[1][i].Substring(0, 6));

                if (string.IsNullOrEmpty(Retention.LastDate) || string.Compare(temp, Retention.LastDate) > 0)
                    Charts.Push(string.Concat(temp, ";", list[5][i], ";", list[6][i]));

                else
                {
                    Send?.Invoke(this, new SendSecuritiesAPI(Retention.Code, Charts));

                    return;
                }
            }
            if (IsNext)
                Connect.GetInstance().Request.RequestTrData(new Task(() => SendErrorMessage(GetType().Name, Request(IsNext))));

            else
                Send?.Invoke(this, new SendSecuritiesAPI(Retention.Code, Charts));
        }
        protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage) => base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
        HashSet<InBlock> InBlock
        {
            get; set;
        }
        Stack<string> Charts
        {
            get; set;
        }
        Retention Retention
        {
            get; set;
        }
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}