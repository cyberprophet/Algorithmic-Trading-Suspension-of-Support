using System;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    internal class H01 : Real, IReals, IStates<State>
    {
        internal H01() : base()
        {
            Console.WriteLine(GetType().Name);
        }
        protected override void OnReceiveRealData(string szTrCode)
        {
            string[] arr = Enum.GetNames(typeof(H1)), temp = new string[arr.Length];

            for (int i = 0; i < arr.Length - 1; i++)
                temp[i] = GetFieldData(OutBlock, arr[i]);

            if (temp[13].Equals(buy) && uint.TryParse(temp[9], out uint number) && double.TryParse(temp[16], out double price))
                switch (temp[12])
                {
                    case sell:
                        API.SellOrder[number.ToString()] = price;
                        break;

                    case buy:
                        API.BuyOrder[number.ToString()] = price;
                        break;
                }
            SendState?.Invoke(this, new State(API.OnReceiveBalance = true, API.SellOrder.Count, API.Quantity, API.BuyOrder.Count, API.AvgPurchase));
        }
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(new Secret().GetResFileName(GetType().Name)))
                AdviseRealData();
        }
        public event EventHandler<State> SendState;
    }
    internal enum H1
    {
        lineseq = 0,
        accno = 1,
        user = 2,
        seq = 3,
        trcode = 4,
        megrpno = 5,
        boardid = 6,
        memberno = 7,
        bpno = 8,
        ordno = 9,
        orgordno = 10,
        expcode = 11,
        dosugb = 12,
        mocagb = 13,
        accno1 = 14,
        qty2 = 15,
        price = 16,
        ordgb = 17,
        hogagb = 18,
        sihogagb = 19,
        tradid = 20,
        treacode = 21,
        askcode = 22,
        creditcode = 23,
        jakigb = 24,
        trustnum = 25,
        ptgb = 26,
        substonum = 27,
        accgb = 28,
        accmarggb = 29,
        nationcode = 30,
        investgb = 31,
        forecode = 32,
        medcode = 33,
        ordid = 34,
        macid = 35,
        orddate = 36,
        rcvtime = 37,
        mem_filler = 38,
        mem_accno = 39,
        mem_filler1 = 40,
        ordacpttm = 41,
        qty = 42,
        autogb = 43,
        rejcode = 44,
        prgordde = 45
    }
}