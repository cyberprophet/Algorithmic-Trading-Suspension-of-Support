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
        protected InBlock GetInBlock()
        {
            return new InBlock
            {
                Block = inBlock
            };
        }
        protected string OutBlock
        {
            get
            {
                return outBlock;
            }
        }
        protected enum C
        {
            chetime = 1,
            sign = 2,
            change = 3,
            drate = 4,
            price = 5,
            open = 6,
            high = 7,
            low = 8,
            cgubun = 9,
            cvolume = 10,
            volume = 11,
            value = 12,
            mdvolume = 13,
            mdchecnt = 14,
            msvolume = 15,
            mschecnt = 16,
            cpower = 17,
            offerho1 = 18,
            bidho1 = 19,
            openyak = 20,
            k200jisu = 21,
            theoryprice = 22,
            kasis = 23,
            sbasis = 24,
            ibasis = 25,
            openyakcha = 26,
            jgubun = 27,
            jnilvolume = 28,
            futcode = 29,
            chetime1 = 30
        }
        protected enum H
        {
            hotime = 12,
            offerho = 1,
            bidho = 2,
            offerrem = 3,
            bidrem = 4,
            offercnt = 5,
            bidcnt = 6,
            totofferrem = 7,
            totbidrem = 8,
            totoffercnt = 9,
            totbidcnt = 10,
            futcode = 11,
            dangochk = 'H',
            alloc_gubun = 'G'
        }
        protected ConnectAPI API
        {
            get
            {
                return ConnectAPI.GetInstance();
            }
        }
        private const string field = "futcode";
        private const string inBlock = "InBlock";
        private const string outBlock = "OutBlock";
    }
}