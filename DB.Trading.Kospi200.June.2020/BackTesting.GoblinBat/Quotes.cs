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
                    if (e.OrderNumber.Count > 1)
                        SendCorrectionOrder(e.Price, e.Quantity);

                    else if (e.OrderNumber.Count < 2 && Difference > 1)
                        SendNewOrder(e.Price, Difference >= e.Price.Length / 2 ? e.Price.Length / 2 : (int)Difference);
                }
                else if (e.OrderNumber.Count > 0)
                    SendClearingOrder(Classification.Equals("2") ? e.OrderNumber.OrderBy(o => o.Key) : e.OrderNumber.OrderByDescending(o => o.Key));
            }
        }
        private void SendCorrectionOrder(double[] param, int[] amount)
        {
            double price;
            string number;

            if (Classification.Equals("1"))
            {
                if (amount[4] < amount[5] && api.OrderNumber.TryGetValue(param[4], out string buy))
                {
                    price = api.OrderNumber.Max(o => o.Key) + Const.ErrorRate;
                    number = buy;
                }
                else if (amount[4] > amount[5] && api.OrderNumber.ContainsKey(param[4]) == false)
                {
                    price = api.OrderNumber.Min(o => o.Key) - Const.ErrorRate;
                    number = api.OrderNumber[api.OrderNumber.Max(o => o.Key)];
                }
                else
                    return;
            }
            else if (Classification.Equals("2"))
            {
                if (amount[4] > amount[5] && api.OrderNumber.TryGetValue(param[5], out string sell))
                {
                    price = api.OrderNumber.Min(o => o.Key) - Const.ErrorRate;
                    number = sell;
                }
                else if (amount[4] < amount[5] && api.OrderNumber.ContainsKey(param[5]) == false)
                {
                    price = api.OrderNumber.Max(o => o.Key) + Const.ErrorRate;
                    number = api.OrderNumber[api.OrderNumber.Min(o => o.Key)];
                }
                else
                    return;
            }
            else
                return;

            if (api.OnReceiveBalance < 1 && api.OrderNumber.Count > 0)
            {
                api.OnReceiveBalance++;
                var before = api.OrderNumber.FirstOrDefault(o => o.Value.Equals(number)).Key;

                if (price != before)
                    api.OnReceiveOrder(new PurchaseInformation
                    {
                        RQName = string.Concat(before, ";", price),
                        ScreenNo = string.Concat(Classification, specify.Code.Substring(0, 3)),
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
                    api.OnReceiveBalance--;
            }
        }
        private void SendClearingOrder(IOrderedEnumerable<KeyValuePair<double, string>> param)
        {
            foreach (KeyValuePair<double, string> kv in param)
            {
                api.OnReceiveBalance++;
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = kv.Key.ToString(),
                    ScreenNo = string.Concat(Classification, specify.Code.Substring(0, 3)),
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
        }
        private void SendNewOrder(double[] param, int length)
        {
            for (int i = 1; i < length; i++)
            {
                api.OnReceiveBalance++;
                var price = param[Classification.Equals("2") ? i + 5 : 4 - i].ToString();
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