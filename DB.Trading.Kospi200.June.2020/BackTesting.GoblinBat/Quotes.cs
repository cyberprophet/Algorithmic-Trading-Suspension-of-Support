using System;
using System.Collections.Generic;
using System.Linq;
using ShareInvest.Interface.Struct;
using ShareInvest.OpenAPI;

namespace ShareInvest.Strategy
{
    public class Quotes
    {
        public Quotes(Specify specify, ConnectAPI api)
        {
            this.specify = specify;
            this.api = api;
            api.OnReceiveBalance = false;
            api.SendQuotes += OnReceiveQuotes;
        }
        internal void SetTrendFollowing(double difference, double classification)
        {
            Difference = api.Quantity > 0 && classification < 0 || api.Quantity < 0 && classification > 0 ? difference + 1 : difference;
            Classification = classification > 0 ? "2" : "1";
        }
        private bool GetCancelOrder(double[] price)
        {
            for (int i = Classification.Equals("2") ? 0 : 5; i < (Classification.Equals("2") ? 5 : 10); i++)
                if (api.OrderNumber.ContainsValue(price[i]))
                    return true;

            return false;
        }
        private void OnReceiveQuotes(object sender, EventHandler.Quotes e)
        {
            if (api.OnReceiveBalance)
            {
                if (GetCancelOrder(e.Price))
                    SendClearingOrder(Classification.Equals("2") ? api.OrderNumber.OrderBy(o => o.Value) : api.OrderNumber.OrderByDescending(o => o.Value));

                else if (api.OrderNumber.Count > 0)
                    SendCorrectionOrder(e.Price, e.Quantity);

                else if (api.OrderNumber.Count < 1 && Difference > 1)
                    SendNewOrder(e.Price, Difference);
            }
        }
        private void SendCorrectionOrder(double[] param, int[] amount)
        {
            double price = 0;
            string number = string.Empty;

            if (Classification.Equals("1"))
            {
                if (amount[4] < amount[5] && (api.OrderNumber.ContainsValue(param[3]) || api.OrderNumber.ContainsValue(param[4])))
                {
                    price = api.OrderNumber.Max(o => o.Value) + Const.ErrorRate;
                    number = api.OrderNumber.First(o => o.Value == api.OrderNumber.Min(p => p.Value)).Key;
                }
                else if (amount[4] > amount[5] && (api.OrderNumber.ContainsValue(param[3]) == false || api.OrderNumber.ContainsValue(param[4]) == false))
                {
                    price = api.OrderNumber.Min(o => o.Value) - Const.ErrorRate;
                    number = api.OrderNumber.First(o => o.Value == api.OrderNumber.Max(p => p.Value)).Key;

                    if (price <= param[5])
                        return;
                }
            }
            else if (Classification.Equals("2"))
            {
                if (amount[4] > amount[5] && (api.OrderNumber.ContainsValue(param[5]) || api.OrderNumber.ContainsValue(param[6])))
                {
                    price = api.OrderNumber.Min(o => o.Value) - Const.ErrorRate;
                    number = api.OrderNumber.First(o => o.Value == api.OrderNumber.Max(p => p.Value)).Key;
                }
                else if (amount[4] < amount[5] && (api.OrderNumber.ContainsValue(param[5]) == false || api.OrderNumber.ContainsValue(param[6]) == false))
                {
                    price = api.OrderNumber.Max(o => o.Value) + Const.ErrorRate;
                    number = api.OrderNumber.First(o => o.Value == api.OrderNumber.Min(p => p.Value)).Key;

                    if (price >= param[4])
                        return;
                }
            }
            if (api.OrderNumber.Count > 0 && price > 0 && number.Equals(string.Empty) == false && api.OrderNumber.ContainsValue(price) == false)
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = string.Concat(price, ';', number),
                    ScreenNo = string.Concat(Classification, number.Substring(number.Length - 3)),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 2,
                    SlbyTP = string.Empty,
                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                    Qty = 1,
                    Price = price.ToString(),
                    OrgOrdNo = number
                });
        }
        private void SendClearingOrder(IOrderedEnumerable<KeyValuePair<string, double>> param)
        {
            foreach (KeyValuePair<string, double> kv in param)
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = string.Concat(kv.Key, ";Clear"),
                    ScreenNo = string.Concat(Classification, kv.Key.Substring(kv.Key.Length - 3)),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 3,
                    SlbyTP = string.Empty,
                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                    Qty = 1,
                    Price = string.Empty,
                    OrgOrdNo = kv.Key
                });
        }
        private void SendNewOrder(double[] param, double length)
        {
            for (int i = 1; i < (length > 3 ? 4 : (length < 2 ? 2 : 3)); i++)
            {
                var price = param[Classification.Equals("2") ? 9 - i : i];

                if (api.OrderNumber.ContainsValue(price))
                    continue;

                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = string.Concat(price, ";New"),
                    ScreenNo = string.Concat(Classification, "00", i),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 1,
                    SlbyTP = Classification,
                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                    Qty = 1,
                    Price = price.ToString(),
                    OrgOrdNo = string.Empty
                });
            }
        }
        private double Difference
        {
            get; set;
        }
        private string Classification
        {
            get; set;
        }
        private readonly Specify specify;
        private readonly ConnectAPI api;
    }
}