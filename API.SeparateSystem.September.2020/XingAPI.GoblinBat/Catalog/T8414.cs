using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class T8414 : Query, ICharts<SendSecuritiesAPI>
    {
        public void QueryExcute(IRetention retention)
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
            {
                InBlock = new HashSet<InBlock>();
                SendMessage(retention.Code, retention.LastDate, string.Empty);

                foreach (var param in GetInBlocks(GetType().Name))
                    if (InBlock.Add(new InBlock
                    {
                        Block = param.Block,
                        Field = param.Field,
                        Occurs = param.Occurs,
                        Data = param.Data ?? retention.Code
                    }))
                        SetFieldData(param.Block, param.Field, param.Occurs, param.Data ?? retention.Code);

                new Task(() =>
                {
                    Thread.Sleep(0xBB9);
                    SendErrorMessage(GetType().Name, Request(false));
                }).Start();
            }
            Charts = new Stack<string>();
            Retention = retention.LastDate?.Substring(0, 0xC);
        }
        protected internal override void OnReceiveData(string szTrCode)
        {
            var enumerable = GetOutBlocks();
            var list = new List<string[]>();
            var index = int.MinValue;
            var code = string.Empty;

            while (enumerable.Count > 0)
            {
                var param = enumerable.Dequeue();

                switch (enumerable.Count)
                {
                    case int count when count > 0x1C:
                        var block = InBlock.First(o => o.Field.Equals(param.Field));
                        SetFieldData(block.Block, block.Field, block.Occurs, block.Data);
                        continue;

                    case int count when count < 7 || count == 7 && Decompress(param.Block) > 0:
                        var bCount = GetBlockCount(param.Block);
                        var array = new string[bCount];

                        for (int i = 0; i < bCount; i++)
                            array[i] = GetFieldData(param.Block, param.Field, i);

                        list.Add(array);
                        break;

                    case int count when count == 0xD || count == 0xC:
                        var data = GetFieldData(param.Block, param.Field, 0);
                        var refresh = InBlock.First(o => o.Field.Equals(param.Field));
                        SetFieldData(refresh.Block, refresh.Field, refresh.Occurs, data);
                        continue;

                    case 0x1A:
                        var temp = InBlock.First(o => o.Field.Equals(param.Field));
                        SetFieldData(temp.Block, temp.Field, temp.Occurs, temp.Data);
                        continue;

                    case 0x19:
                        code = GetFieldData(param.Block, param.Field, 0);
                        continue;

                    case 8:
                        if (int.TryParse(GetFieldData(param.Block, param.Field, 0), out int rCount))
                            index = rCount;

                        continue;
                }
            }
            var span = WaitForTheLimitTime(GetTRCountRequest(szTrCode));
            SendMessage(span);

            if (span.TotalSeconds > 0xC5 && span.TotalSeconds < 0xC8)
                Milliseconds = (int)span.TotalMilliseconds;

            else
                Milliseconds = 0x3EB / GetTRCountPerSec(szTrCode);

            if (index > 0)
                for (int i = index - 1; i >= 0; i--)
                {
                    var temp = string.Concat(list[0][i].Substring(2), list[1][i].Substring(0, 6));

                    if (string.IsNullOrEmpty(Retention) || string.Compare(temp, Retention) > 0)
                        Charts.Push(string.Concat(temp, ";", list[5][i], ";", list[6][i]));

                    else
                    {
                        new Task(() =>
                        {
                            Thread.Sleep(Milliseconds);
                            Send?.Invoke(this, new SendSecuritiesAPI(code, Charts));
                        }).Start();
                        return;
                    }
                }
            new Task(() =>
            {
                Thread.Sleep(Milliseconds);

                if (IsNext && Array.Exists(Retention.ToCharArray(), o => char.IsLetter(o)) == false)
                    SendErrorMessage(GetType().Name, Request(IsNext));

                else
                    Send?.Invoke(this, new SendSecuritiesAPI(code, Charts));
            }).Start();
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
        string Retention
        {
            get; set;
        }
        int Milliseconds
        {
            get; set;
        }
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}