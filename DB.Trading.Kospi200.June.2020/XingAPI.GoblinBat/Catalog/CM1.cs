using System;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    class CM1 : Real, IReals, IStates<State>
    {
        protected internal override void OnReceiveRealData(string szTrCode)
        {
            API.OnReceiveBalance = false;
            string[] arr = Enum.GetNames(typeof(CMO)), temp = new string[arr.Length];

            for (int i = 0; i < arr.Length - 1; i++)
                temp[i] = GetFieldData(OutBlock, arr[i]);

            if (uint.TryParse(temp[45], out uint number))
                switch (temp[55])
                {
                    case sell:
                        if (int.TryParse(temp[83], out int sq) && double.TryParse(temp[82], out double sp) && API.SellOrder.Remove(number.ToString()))
                        {
                            if (API.Quantity <= 0)
                                API.AvgPurchase = ((sp * sq - double.Parse(API.AvgPurchase) * API.Quantity) / (sq - API.Quantity)).ToString("F2");

                            API.Quantity -= sq;
                        }
                        break;

                    case buy:
                        if (int.TryParse(temp[83], out int bq) && double.TryParse(temp[82], out double bp) && API.BuyOrder.Remove(number.ToString()))
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
        public event EventHandler<State> SendState;
    }
}