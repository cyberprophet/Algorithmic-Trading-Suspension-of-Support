using System;
using System.Collections.Generic;
using System.Drawing;

namespace ShareInvest.EventHandler.XingAPI
{
    public class Quotes : EventArgs
    {
        public Quotes(double[] price, int[] quantity, string[] temp, Dictionary<string, double> so, Dictionary<string, double> bo, bool state)
        {
            SellOrder = so;
            BuyOrder = bo;
            Price = price;
            Quantity = quantity;
            Time = temp[5];
            Code = temp[4];
            Color = so.Count == 0 && bo.Count == 0 ? (state ? Color.Ivory : Color.Maroon) : Color.Gold;

            if (int.TryParse(temp[0], out int sell))
                Sell = sell;

            if (int.TryParse(temp[1], out int buy))
                Buy = buy;
        }
        public Quotes(double[] price, string[] temp, Dictionary<string, double> so, Dictionary<string, double> bo, bool state)
        {
            SellOrder = so;
            BuyOrder = bo;
            Price = price;
            Time = temp[5];
            Code = temp[4];
            Color = so.Count == 0 && bo.Count == 0 ? (state ? Color.Ivory : Color.Maroon) : Color.Gold;

            if (int.TryParse(temp[0], out int sell))
                Sell = sell;

            if (int.TryParse(temp[1], out int buy))
                Buy = buy;
        }
        public Dictionary<string, double> SellOrder
        {
            get; private set;
        }
        public Dictionary<string, double> BuyOrder
        {
            get; private set;
        }
        public Color Color
        {
            get; private set;
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