namespace ShareInvest.BackUp
{
    class Scalping
    {
        /*
        private void BackUp()
        {
            if (api != null)
            {
                if (api.Remaining == null)
                    api.RemainingDay();

                if (e.Time.Equals("154458") || e.Time.Equals("154459") || e.Time.Equals("154500") || e.Time.Equals("154454") || e.Time.Equals("154455") || e.Time.Equals("154456") || e.Time.Equals("154457") || (e.Time.Equals("151957") || e.Time.Equals("151958") || e.Time.Equals("151959") || e.Time.Equals("152000")) && api.Remaining.Equals("1"))
                {
                    while (api.Quantity != 0)
                        Order(Operate(api.Quantity > 0 ? -1 : 1));

                    return;
                }
                if (e.Volume > secret || e.Volume < -secret)
                {
                    quantity = Order(gap, width_gap);

                    if (quantity > 0 ? api.PurchaseQuantity == false : api.SellQuantity == false && Math.Abs(e.Volume) < Math.Abs(e.Volume + quantity))
                    {
                        MaximumQuantity = (int)(basicAsset / (e.Price * tm * margin));

                        while (Math.Abs(api.Quantity + quantity) < MaximumQuantity)
                            repeat += Operate(quantity);

                        Order(repeat);

                        return;
                    }
                }
                if (api.Quantity > 0 && e.Volume < -secret && api.PurchasePrice >= e.Price || api.Quantity < 0 && e.Volume > secret && api.PurchasePrice <= e.Price)
                {
                    while (api.Quantity != 0)
                        repeat += Operate(e.Volume < 0 ? -1 : 1);

                    Order(repeat);

                    return;
                }
                Order(Operate(DetermineProfit(e.Price, e.Volume, secret)));
            }
        }
        private int MaximumQuantity
        {
            get; set;
        }
        private int DetermineProfit(double price, int volume, int secret)
        {
            if (volume < -secret / 3 && api.Quantity > 0 && (api.PurchaseQuantity == true || api.PurchasePrice - 5d / volume < price))
                return -1;

            if (volume > secret / 3 && api.Quantity < 0 && (api.SellQuantity == true || api.PurchasePrice + 5d / volume > price))
                return 1;

            return 0;
        }
        private int Operate(int quantity)
        {
            api.Quantity += quantity;

            return quantity;
        }
        private void Order(int repeat)
        {
            if (repeat != 0)
            {
                if (repeat > MaximumQuantity || repeat < -MaximumQuantity)
                {
                    api.OnReceiveOrder(ScreenNo, dic[repeat < 0 ? -1 : 1], MaximumQuantity);

                    Order(repeat > 0 ? repeat - MaximumQuantity : repeat + MaximumQuantity);

                    return;
                }
                api.OnReceiveOrder(ScreenNo, dic[repeat < 0 ? -1 : 1], repeat);
            }
        }
        private int Operate(int quantity)
        {
            api.Quantity += quantity;

            return quantity;
        }
        public void OnReceiveOrder(string sScreenNo, string sSlbyTP, int lQty)
        {
            request.RequestTrData(new Task(() =>
            {
                Error_code = axAPI.SendOrderFO(string.Concat(sSlbyTP, ';', lQty), sScreenNo, Account, Code, 1, sSlbyTP, "3", Math.Abs(lQty), "", "");

                if (Error_code != 0)
                    new Error(Error_code);
            }));
        }
        */
    }
}