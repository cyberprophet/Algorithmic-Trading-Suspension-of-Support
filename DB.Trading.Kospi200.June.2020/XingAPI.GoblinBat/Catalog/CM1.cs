using System;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    internal class CM1 : Real, IReals, IStates<State>
    {
        internal CM1() : base()
        {
            Console.WriteLine(GetType().Name);
        }
        protected override void OnReceiveRealData(string szTrCode)
        {
            API.OnReceiveBalance = false;
            string[] arr = Enum.GetNames(typeof(CMO)), temp = new string[arr.Length];

            for (int i = 0; i < arr.Length - 1; i++)
                temp[i] = GetFieldData(OutBlock, arr[i]);

            switch (temp[55].Equals(sell))
            {
                case true:
                    if (API.SellOrder.Remove(temp[45]) && int.TryParse(temp[83], out int sell))
                    {
                        if (API.Quantity <= 0 && double.TryParse(temp[82], out double sp))
                            API.AvgPurchase = ((sp * sell - double.Parse(API.AvgPurchase) * API.Quantity) / (sell - API.Quantity)).ToString("F2");

                        API.Quantity -= sell;
                    }
                    break;

                case false:
                    if (API.BuyOrder.Remove(temp[45]) && int.TryParse(temp[83], out int buy))
                    {
                        if (API.Quantity >= 0 && double.TryParse(temp[82], out double bp))
                            API.AvgPurchase = ((double.Parse(API.AvgPurchase) * API.Quantity + bp * buy) / (buy + API.Quantity)).ToString("F2");

                        API.Quantity += buy;
                    }
                    break;
            }
            if (API.Quantity == 0)
                API.AvgPurchase = "000.00";

            API.OnReceiveBalance = true;
            SendState?.Invoke(this, new State(API.OnReceiveBalance, API.SellOrder.Count, API.Quantity, API.BuyOrder.Count, API.AvgPurchase));
        }
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(new Secret().GetResFileName(GetType().Name)))
                AdviseRealData();
        }
        public event EventHandler<State> SendState;
    }
}