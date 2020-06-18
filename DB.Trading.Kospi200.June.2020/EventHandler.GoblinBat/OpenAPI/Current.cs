using System;

namespace ShareInvest.EventHandler.OpenAPI
{
    public class Current : EventArgs
    {
        public int Quantity
        {
            get; private set;
        }
        public double Price
        {
            get; private set;
        }
        public Current(int quantity, string[] param)
        {
            if (double.TryParse(param[0].Contains("-") ? param[0].Substring(1) : param[0], out double price))
                Price = price;

            Quantity = quantity;
        }
    }
}