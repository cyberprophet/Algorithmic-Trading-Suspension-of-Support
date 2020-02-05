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
            if (api.OnReceiveBalance)
            {
                api.OnReceiveBalance = false;

                if (CancelOrder)
                {
                    if (e.OrderNumber.Count > 0)
                        SendCorrectionOrder(e.Price, e.Quantity);

                    else if (e.OrderNumber.Count == 0 && Difference > 1)
                        SendNewOrder(e.Price, Difference >= e.Price.Length / 3 ? e.Price.Length / 3 : (int)Difference);

                    else
                        api.OnReceiveBalance = true;
                }
                else if (e.OrderNumber.Count > 0)
                    SendClearingOrder(Classification.Equals("2") ? e.OrderNumber.OrderBy(o => o.Key) : e.OrderNumber.OrderByDescending(o => o.Key));

                else
                    api.OnReceiveBalance = true;
            }
        }
        private void SendCorrectionOrder(double[] param, int[] amount)
        {
            string price, number;

            if (Classification.Equals("1"))
            {
                if (amount[4] < amount[5] && api.OrderNumber.TryGetValue(param[4], out string buy))
                {
                    price = (api.OrderNumber.Max(o => o.Key) + Const.ErrorRate).ToString();
                    number = buy;
                }
                else if (amount[4] > amount[5] && api.OrderNumber.ContainsKey(param[4]) == false)
                {
                    price = param[4].ToString();
                    number = api.OrderNumber[api.OrderNumber.Max(o => o.Key)];
                }
                else
                {
                    api.OnReceiveBalance = true;

                    return;
                }
            }
            else if (Classification.Equals("2"))
            {
                if (amount[4] > amount[5] && api.OrderNumber.TryGetValue(param[5], out string sell))
                {
                    price = (api.OrderNumber.Min(o => o.Key) - Const.ErrorRate).ToString();
                    number = sell;
                }
                else if (amount[4] < amount[5] && api.OrderNumber.ContainsKey(param[5]) == false)
                {
                    price = param[5].ToString();
                    number = api.OrderNumber[api.OrderNumber.Min(o => o.Key)];
                }
                else
                {
                    api.OnReceiveBalance = true;

                    return;
                }
            }
            else
            {
                api.OnReceiveBalance = true;

                return;
            }
            api.OnReceiveOrder(new PurchaseInformation
            {
                RQName = string.Concat(api.OrderNumber.FirstOrDefault(o => o.Value.Equals(number)).Key, ';', price),
                ScreenNo = number.Substring(number.Length - 4, 4),
                AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                Code = specify.Code,
                OrdKind = 2,
                SlbyTP = Classification,
                OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                Qty = 1,
                Price = price,
                OrgOrdNo = number
            });
        }
        private void SendClearingOrder(IOrderedEnumerable<KeyValuePair<double, string>> param)
        {
            foreach (KeyValuePair<double, string> kv in param)
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = string.Concat(kv.Key, ';', kv.Value),
                    ScreenNo = kv.Value.Substring(kv.Value.Length - 4, 4),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 3,
                    SlbyTP = Classification.Equals("1") ? "2" : "1",
                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                    Qty = 1,
                    Price = string.Empty,
                    OrgOrdNo = kv.Value
                });
        }
        private void SendNewOrder(double[] param, int length)
        {
            for (int i = 1; i < length; i++)
            {
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