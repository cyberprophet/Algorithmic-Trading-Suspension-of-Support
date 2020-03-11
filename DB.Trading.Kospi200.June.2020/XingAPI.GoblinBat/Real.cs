using System;
using XA_DATASETLib;

namespace ShareInvest.XingAPI
{
    internal class Real : XARealClass
    {
        internal protected Real()
        {
            ReceiveRealData += OnReceiveRealData;
        }
        protected virtual void OnReceiveRealData(string szTrCode)
        {
            Console.WriteLine(szTrCode);
        }
        protected InBlock GetInBlock(string code)
        {
            return new InBlock
            {
                Block = inBlock,
                Field = field,
                Data = code
            };
        }
        protected string OutBlock
        {
            get
            {
                return outBlock;
            }
        }
        private const string field = "futcode";
        private const string inBlock = "InBlock";
        private const string outBlock = "OutBlock";
    }
}