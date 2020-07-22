using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    class T8406 : Query, ICharts<SendSecuritiesAPI>
    {
        public void QueryExcute(Retention retention)
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

                Connect.GetInstance().Request.RequestTrData(new Task(() => SendErrorMessage(GetType().Name, Request(false))));
            }
            Charts = new Stack<string>();
            Retention = retention;
        }
        protected internal override void OnReceiveData(string szTrCode)
        {
            var span = WaitForTheLimitTime(GetTRCountRequest(szTrCode));
            var enumerable = GetOutBlocks();
            var count = enumerable.Count;
            var sArray = new StringBuilder[count - 4];
            SendMessage(span);

            if (span.TotalSeconds > 0xC5 && span.TotalSeconds < 0xC8)
                Delay.Milliseconds = (int)span.TotalMilliseconds;

            else
                Delay.Milliseconds = 0x3E8 / GetTRCountPerSec(szTrCode);

            while (enumerable.Count > 0)
            {
                var param = enumerable.Dequeue();

                if (enumerable.Count < count - 4)
                {
                    var sb = new StringBuilder();

                    for (int i = 0; i < GetBlockCount(param.Block); i++)
                        sb.Append(GetFieldData(param.Block, param.Field, i)).Append(';');

                    sArray[enumerable.Count] = sb.Remove(sb.Length - 1, 1);
                }
            }
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
        [Conditional("DEBUG")]
        void SendMessage(TimeSpan span) => Console.WriteLine(span.TotalSeconds);
    }
}