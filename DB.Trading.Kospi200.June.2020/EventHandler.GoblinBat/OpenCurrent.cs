using System;

namespace ShareInvest.EventHandler
{
    public class OpenCurrent : EventArgs
    {
        public int Quantity
        {
            get; private set;
        }
        public double Price
        {
            get; private set;
        }
        public OpenCurrent(int quantity, string[] param)
        {
            Quantity = quantity;
            Price = double.Parse(param[0].Contains("-") ? param[0].Substring(1) : param[0]);
        }
    }
}