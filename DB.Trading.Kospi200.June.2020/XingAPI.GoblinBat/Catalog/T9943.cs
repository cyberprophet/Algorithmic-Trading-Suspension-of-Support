using System;
using System.Collections.Generic;
using ShareInvest.Catalog;

namespace ShareInvest.XingAPI.Catalog
{
    internal class T9943 : Query, IQuerys
    {
        internal T9943() : base()
        {
            Console.WriteLine(GetType().Name);
        }
        protected override void OnReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            base.OnReceiveMessage(bIsSystemError, nMessageCode, szMessage);
        }
        protected override void OnReceiveData(string szTrCode)
        {
            var enumerable = GetOutBlocks();
            string[] code = null, name = null;

            while (enumerable.Count > 0)
            {
                var param = enumerable.Dequeue();
                var length = GetBlockCount(param.Block);

                switch (enumerable.Count)
                {
                    case 0:
                        API.CodeList = new Dictionary<string, string>();

                        for (int i = 0; i < code.Length; i++)
                            API.CodeList[code[i]] = string.Concat(kospi200, name[i]);

                        return;

                    case 1:
                        code = new string[length];
                        break;

                    case 2:
                        name = new string[length];
                        break;
                }
                for (int i = 0; i < length; i++)
                    switch (enumerable.Count)
                    {
                        case 1:
                            code[i] = GetFieldData(param.Block, param.Field, i);
                            break;

                        case 2:
                            name[i] = GetFieldData(param.Block, param.Field, i);
                            break;
                    }
            }
        }
        public void QueryExcute()
        {
            if (LoadFromResFile(new Secret().GetResFileName(GetType().Name)))
            {
                foreach (var param in GetInBlocks(GetType().Name))
                    SetFieldData(param.Block, param.Field, param.Occurs, param.Data);

                SendErrorMessage(Request(false));
            }
        }
        private const string kospi200 = "KOSPI200 ";
    }
}