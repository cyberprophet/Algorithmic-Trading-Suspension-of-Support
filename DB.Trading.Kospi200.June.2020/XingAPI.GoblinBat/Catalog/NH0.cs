using System;
using ShareInvest.Catalog;

namespace ShareInvest.XingAPI.Catalog
{
    internal class NH0 : Real, IReal
    {
        internal NH0() : base()
        {

        }
        protected override void OnReceiveRealData(string szTrCode)
        {
            foreach (var str in GetBlockData(OutBlock).Split(new char[]
            {
                ' ',
                '?'
            }))
                Console.WriteLine(szTrCode + "\t" + str);
        }
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(new Secret().GetResFileName(GetType().Name)))
            {
                var param = GetInBlock(code);
                SetFieldData(param.Block, param.Field, param.Data);
                AdviseRealData();
            }
        }
    }
}