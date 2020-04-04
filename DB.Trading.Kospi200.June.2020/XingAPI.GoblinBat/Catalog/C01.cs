using System;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    class C01 : Real, IReals, IStates<State>
    {
        protected internal override void OnReceiveRealData(string szTrCode)
        {
            API.OnReceiveBalance = false;
            string[] arr = Enum.GetNames(typeof(C1)), temp = new string[arr.Length];

            for (int i = 0; i < arr.Length - 1; i++)
                temp[i] = GetFieldData(OutBlock, arr[i]);

            if (uint.TryParse(temp[9], out uint number))
                switch (temp[20])
                {
                    case sell:
                        if (int.TryParse(temp[14], out int sq) && double.TryParse(temp[13], out double sp) && API.SellOrder.Remove(number.ToString()))
                        {
                            if (API.Quantity <= 0)
                                API.AvgPurchase = ((sp * sq - double.Parse(API.AvgPurchase) * API.Quantity) / (sq - API.Quantity)).ToString("F2");

                            API.Quantity -= sq;
                        }
                        break;

                    case buy:
                        if (int.TryParse(temp[14], out int bq) && double.TryParse(temp[13], out double bp) && API.BuyOrder.Remove(number.ToString()))
                        {
                            if (API.Quantity >= 0)
                                API.AvgPurchase = ((double.Parse(API.AvgPurchase) * API.Quantity + bp * bq) / (bq + API.Quantity)).ToString("F2");

                            API.Quantity += bq;
                        }
                        break;
                }
            if (API.Quantity == 0)
                API.AvgPurchase = avg;

            API.OnReceiveBalance = true;
            SendState?.Invoke(this, new State(API.OnReceiveBalance, API.SellOrder.Count, API.Quantity, API.BuyOrder.Count, API.AvgPurchase, API.MaxAmount));
        }
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(new Secret().GetResFileName(GetType().Name)))
                AdviseRealData();
        }
        internal C01() : base() => Console.WriteLine(GetType().Name);
        public event EventHandler<State> SendState;
    }
    enum C1
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
        ordordno = 10,
        expcode = 11,
        yakseq = 12,
        cheprice = 13,
        chevol = 14,
        sessionid = 15,
        chedate = 16,
        chetime = 17,
        spdprc1 = 18,
        spdprc2 = 19,
        dosugb = 20,
        accno1 = 21,
        sihogagb = 22,
        jakino = 23,
        daeyong = 24,
        mem_filler = 25,
        mem_accno = 26,
        mem_filler1 = 27
    }
}