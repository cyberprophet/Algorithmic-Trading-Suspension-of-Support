using System;
using System.Collections.Generic;

namespace ShareInvest.EventHandler.OpenAPI
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
        public string Total
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
        public Quotes(string[] price, string[] quantity, string[] number, string time, Dictionary<string, double> sell, Dictionary<string, double> buy, string total)
        {
            Price = new double[price.Length];
            Quantity = new int[quantity.Length];
            Number = new int[number.Length];

            for (int i = 0; i < 10; i++)
                if (int.TryParse(number[i], out int qNum) && int.TryParse(quantity[i], out int qQuantity) && double.TryParse(price[i].Length == 6 ? price[i] : price[i].Substring(1), out double qPrice))
                {
                    Price[i] = qPrice;
                    Quantity[i] = qQuantity;
                    Number[i] = qNum;
                }
            Time = time;
            SellOrder = sell;
            BuyOrder = buy;
            Total = total;
        }
    }
}