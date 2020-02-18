using System;
using System.Collections.Generic;

namespace ShareInvest.EventHandler
{
    public class Quotes : EventArgs
    {
        public double[] Price
        {
            get; private set;
        }
        public int[] Quantity
        {
            get; private set;
        }
        public int[] Number
        {
            get; private set;
        }
        public string Time
        {
            get; private set;
        }
        public Dictionary<string, double> SellOrder
        {
            get; private set;
        }
        public Dictionary<string, double> BuyOrder
        {
            get; private set;
        }
        public Quotes(string[] price, string[] quantity, string[] number, string time, Dictionary<string, double> sell, Dictionary<string, double> buy)
        {
            Price = new double[price.Length];
            Quantity = new int[quantity.Length];
            Number = new int[number.Length];

            for (int i = 0; i < 10; i++)
            {
                Price[i] = double.Parse(price[i].Substring(1));
                Quantity[i] = int.Parse(quantity[i]);
                Number[i] = int.Parse(number[i]);
            }
            Time = time;
            SellOrder = sell;
            BuyOrder = buy;
        }
    }
}