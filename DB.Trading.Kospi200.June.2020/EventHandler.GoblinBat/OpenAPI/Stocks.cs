using System;

namespace ShareInvest.EventHandler.OpenAPI
{
    public class Stocks : EventArgs
    {
        public string Time
        {
            get; private set;
        }
        public string Code
        {
            get; private set;
        }
        public int Price
        {
            get; private set;
        }
        public int Volume
        {
            get; private set;
        }
        public Stocks(string code, string[] param)
        {
            Time = param[0];
            Price = int.TryParse(param[1].Substring(0, 1).Equals("-") ? param[1].Substring(1) : param[1], out int price) ? price : 0;
            Volume = int.TryParse(param[6], out int volume) ? volume : 0;
            Code = code;
        }
    }
}