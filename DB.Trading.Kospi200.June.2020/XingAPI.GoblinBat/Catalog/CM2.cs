using System;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    internal class CM2 : Real, IReals, IStates<State>
    {
        protected override void OnReceiveRealData(string szTrCode)
        {
            string[] arr = Enum.GetNames(typeof(CMO)), temp = new string[arr.Length];

            for (int i = 0; i < arr.Length - 1; i++)
                temp[i] = GetFieldData(OutBlock, arr[i]);

            if (temp[56].Equals(sell) && uint.TryParse(temp[45], out uint number) && uint.TryParse(temp[47], out uint org) && double.TryParse(temp[60], out double price))
                switch (temp[55])
                {
                    case sell:
                        if (API.SellOrder.Remove(org.ToString()))
                            API.SellOrder[number.ToString()] = price;

                        break;

                    case buy:
                        if (API.BuyOrder.Remove(org.ToString()))
                            API.BuyOrder[number.ToString()] = price;

                        break;
                }
            else if (temp[56].Equals(buy) && uint.TryParse(temp[47], out uint ord))
                switch (temp[55])
                {
                    case sell:
                        API.SellOrder.Remove(ord.ToString());
                        break;

                    case buy:
                        API.BuyOrder.Remove(ord.ToString());
                        break;
                }
            API.OnReceiveBalance = true;
            SendState?.Invoke(this, new State(API.OnReceiveBalance, API.SellOrder.Count, API.Quantity, API.BuyOrder.Count, API.AvgPurchase, API.MaxAmount));
        }
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(new Secret().GetResFileName(GetType().Name)))
                AdviseRealData();
        }
        internal CM2() : base() => Console.WriteLine(GetType().Name);
        public event EventHandler<State> SendState;
    }
}