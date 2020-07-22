using System.Linq;

using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.Client;
using ShareInvest.DelayRequest;

namespace ShareInvest.XingAPI.Catalog
{
    class T8402 : Query, IQuerys
    {
        string Code
        {
            get; set;
        }
        public void QueryExcute(string code)
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
            {
                foreach (var param in GetInBlocks(GetType().Name))
                    SetFieldData(param.Block, param.Field, param.Occurs, param.Data ?? code);

                SendErrorMessage(GetType().Name, Request(false));
            }
            Code = code;
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
            var codes = ConnectAPI.Codes.First(o => o.Code.Equals(Code));
            var refresh = new Codes
            {
                Code = Code,
                Name = temp[0x33].Trim(),
                MarginRate = codes.MarginRate * (double.TryParse(temp[62], out double transactionMutiplier) ? transactionMutiplier : 0),
                MaturityMarketCap = temp[0x16].Substring(2),
                Price = temp[1]
            };
            if (ConnectAPI.Codes.Remove(codes) && ConnectAPI.Codes.Add(refresh) && GoblinBatClient.GetInstance().PutContext<Codes>(refresh) is int statusCode && statusCode == 0xC8)
                Delay.Milliseconds = 0x3E8 / GetTRCountPerSec(szTrCode);
        }
        protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage) => base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
    }
}