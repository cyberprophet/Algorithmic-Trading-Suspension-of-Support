using System.Linq;

using ShareInvest.Catalog;
using ShareInvest.Client;
using ShareInvest.DelayRequest;
using ShareInvest.Interface.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    class T2101 : Query, IQuerys
    {
        public void QueryExcute(string code)
        {
            if (LoadFromResFile(Secrecy.GetResFileName(GetType().Name)))
            {
                foreach (var param in GetInBlocks(GetType().Name))
                    SetFieldData(param.Block, param.Field, param.Occurs, param.Data ?? code);

                SendErrorMessage(GetType().Name, Request(false));
            }
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
            var codes = ConnectAPI.Codes.First(o => o.Code.Equals(temp[0x3C]));
            var refresh = new Codes
            {
                Code = temp[0x3C],
                Name = codes.Code.StartsWith("1") ? temp[0].Split('F')[0].Trim() : temp[0].Remove(temp[0].Length - 2, 2),
                MarginRate = codes.MarginRate,
                MaturityMarketCap = temp[0x17].Substring(2),
                Price = temp[1]
            };
            if (ConnectAPI.Codes.Remove(codes) && ConnectAPI.Codes.Add(refresh) && GoblinBatClient.GetInstance().PutContext<Codes>(refresh).Result is int statusCode && statusCode == 0xC8 && GetTRCountRequest(szTrCode) == 1)
                Delay.Milliseconds = 0x3E8 / GetTRCountPerSec(szTrCode);
        }
        protected internal override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage) => base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
    }
}