using System;

namespace ShareInvest.EventHandler.OpenAPI
{
    public class StocksQuotes : EventArgs
    {
        public StocksQuotes(string code, string price)
        {
            Code = code;
            Price = int.TryParse(price.Substring(0, 1).Equals("-") ? price.Substring(1) : price, out int buy) ? buy : 0;
        }
        public string Code
        {
            get; private set;
        }
        public int Price
        {
            get; private set;
        }
    }
}