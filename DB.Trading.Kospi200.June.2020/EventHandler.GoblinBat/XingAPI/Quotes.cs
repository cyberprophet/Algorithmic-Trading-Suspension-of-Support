using System;

namespace ShareInvest.EventHandler.XingAPI
{
    public class Quotes : EventArgs
    {
        public Quotes(double[] price, int[] quantity, string[] temp)
        {
            Price = price;
            Quantity = quantity;
            Time = temp[5];
            Code = temp[4];

            if (int.TryParse(temp[0], out int sell))
                Sell = sell;

            if (int.TryParse(temp[1], out int buy))
                Buy = buy;
        }
        public Quotes(double[] price, string[] temp)
        {
            Price = price;
            Time = temp[5];
            Code = temp[4];

            if (int.TryParse(temp[0], out int sell))
                Sell = sell;

            if (int.TryParse(temp[1], out int buy))
                Buy = buy;
        }
        public double[] Price
        {
            get; private set;
        }
        public int[] Quantity
        {
            get; private set;
        }
        public string Time
        {
            get; private set;
        }
        public string Code
        {
            get; private set;
        }
        public int Sell
        {
            get; private set;
        }
        public int Buy
        {
            get; private set;
        }
    }
}