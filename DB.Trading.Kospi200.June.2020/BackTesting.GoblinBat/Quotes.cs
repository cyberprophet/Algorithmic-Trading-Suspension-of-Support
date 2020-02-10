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
            api.OnReceiveBalance = 1;
            api.SendQuotes += OnReceiveQuotes;
        }
        internal void SetTrendFollowing(double difference, double classification)
        {
            Difference = difference;
            var temp = classification > 0 ? "2" : "1";
            CancelOrder = temp.Equals(Classification);
            Classification = temp;
        }
        private void OnReceiveQuotes(object sender, EventHandler.Quotes e)
        {
            if (api.OnReceiveBalance < 1)
            {
                if (CancelOrder)
                {
                    if (e.OrderNumber.Count > 0)
                    {
                        api.OnReceiveBalance++;
                        SendCorrectionOrder(e.Price, e.Quantity);
                    }
                    else if (e.OrderNumber.Count < 1 && Difference > 1)
                    {
                        api.OnReceiveBalance++;
                        SendNewOrder(e.Price, Difference);
                    }
                }
                else if (e.OrderNumber.Count > 0)
                {
                    api.OnReceiveBalance++;
                    SendClearingOrder(Classification.Equals("2") ? e.OrderNumber.OrderBy(o => o.Key) : e.OrderNumber.OrderByDescending(o => o.Key));
                }
            }
        }
        private void SendCorrectionOrder(double[] param, int[] amount)
        {
            double price;
            string number;

            if (Classification.Equals("1"))
            {
                if (amount[4] < amount[5] && api.OrderNumber.TryGetValue(param[3], out string buy))
                {
                    price = api.OrderNumber.Max(o => o.Key) + Const.ErrorRate;
                    number = buy;
                }
                else if (amount[4] > amount[5] && api.OrderNumber.ContainsKey(param[3]) == false)
                {
                    price = api.OrderNumber.Min(o => o.Key) - Const.ErrorRate;
                    number = api.OrderNumber[api.OrderNumber.Max(o => o.Key)];
                }
                else
                {
                    api.OnReceiveBalance--;

                    return;
                }
            }
            else if (Classification.Equals("2"))
            {
                if (amount[4] > amount[5] && api.OrderNumber.TryGetValue(param[6], out string sell))
                {
                    price = api.OrderNumber.Min(o => o.Key) - Const.ErrorRate;
                    number = sell;
                }
                else if (amount[4] < amount[5] && api.OrderNumber.ContainsKey(param[6]) == false)
                {
                    price = api.OrderNumber.Max(o => o.Key) + Const.ErrorRate;
                    number = api.OrderNumber[api.OrderNumber.Min(o => o.Key)];
                }
                else
                {
                    api.OnReceiveBalance--;

                    return;
                }
            }
            else
            {
                api.OnReceiveBalance--;

                return;
            }
            if (api.OnReceiveBalance < 1 && api.OrderNumber.Count > 0)
            {
                var before = api.OrderNumber.First(o => o.Value.Equals(number)).Key;

                if (price != before)
                    api.OnReceiveOrder(new PurchaseInformation
                    {
                        RQName = string.Concat(before, ";", price),
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
                else
                {
                    api.OnReceiveBalance--;

                    return;
                }
            }
            else
                api.OnReceiveBalance--;
        }
        private void SendClearingOrder(IOrderedEnumerable<KeyValuePair<double, string>> param)
        {
            foreach (KeyValuePair<double, string> kv in param)
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = kv.Key.ToString(),
                    ScreenNo = string.Concat(Classification, kv.Value.Substring(kv.Value.Length - 3)),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 3,
                    SlbyTP = string.Empty,
                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                    Qty = 1,
                    Price = string.Empty,
                    OrgOrdNo = kv.Value
                });
        }
        private void SendNewOrder(double[] param, double length)
        {
            for (int i = 1; i < (length > 3 ? 4 : (length < 2 ? 2 : 3)); i++)
            {
                var price = param[Classification.Equals("2") ? 9 - i : i].ToString();
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = price,
                    ScreenNo = string.Concat(Classification, "00", i),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 1,
                    SlbyTP = Classification,
                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                    Qty = 1,
                    Price = price,
                    OrgOrdNo = string.Empty
                });
            }
        }
        private bool CancelOrder
        {
            get; set;
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