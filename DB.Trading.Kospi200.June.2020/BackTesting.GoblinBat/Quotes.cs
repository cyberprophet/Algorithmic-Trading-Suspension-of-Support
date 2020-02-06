using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                new Task(() =>
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
                }).Start();
        }
        private void SendCorrectionOrder(double[] param, int[] amount)
        {
            double price;
            string number;

            if (Classification.Equals("1"))
            {
                if (amount[4] < amount[5] && api.OrderNumber.TryGetValue(param[4], out string[] buy))
                {
                    price = api.OrderNumber.Max(o => o.Key) + Const.ErrorRate;
                    number = buy[1];
                }
                else if (amount[4] > amount[5] && api.OrderNumber.ContainsKey(param[4]) == false)
                {
                    price = api.OrderNumber.Min(o => o.Key) - Const.ErrorRate;
                    number = api.OrderNumber[api.OrderNumber.Max(o => o.Key)][1];
                }
                else
                    return;
            }
            else if (Classification.Equals("2"))
            {
                if (amount[4] > amount[5] && api.OrderNumber.TryGetValue(param[5], out string[] sell))
                {
                    price = api.OrderNumber.Min(o => o.Key) - Const.ErrorRate;
                    number = sell[1];
                }
                else if (amount[4] < amount[5] && api.OrderNumber.ContainsKey(param[5]) == false)
                {
                    price = api.OrderNumber.Max(o => o.Key) + Const.ErrorRate;
                    number = api.OrderNumber[api.OrderNumber.Min(o => o.Key)][1];
                }
                else
                    return;
            }
            else
                return;

            if (api.OnReceiveBalance < 1 && api.OrderNumber.Count > 0)
            {
                api.OnReceiveBalance++;
                var before = api.OrderNumber.FirstOrDefault(o => o.Value[1].Equals(number));
                bool kind = price.ToString().Equals(before.Value[0]);
                Thread.Sleep(205);
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = string.Concat(before.Key, ';', price),
                    ScreenNo = string.Concat(Classification, specify.Code.Substring(0, 3)),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = kind ? 3 : 2,
                    SlbyTP = string.Empty,
                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                    Qty = 1,
                    Price = kind ? string.Empty : price.ToString(),
                    OrgOrdNo = number
                });
            }
        }
        private void SendClearingOrder(IOrderedEnumerable<KeyValuePair<double, string[]>> param)
        {
            foreach (KeyValuePair<double, string[]> kv in param)
            {
                while (api.OnReceiveBalance > 1)
                    Thread.Sleep(205);

                api.OnReceiveBalance++;
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = string.Concat(kv.Key, ';', kv.Value[1]),
                    ScreenNo = string.Concat(Classification, specify.Code.Substring(0, 3)),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 3,
                    SlbyTP = string.Empty,
                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                    Qty = 1,
                    Price = string.Empty,
                    OrgOrdNo = kv.Value[1]
                });
            }
        }
        private void SendNewOrder(double[] param, int length)
        {
            for (int i = 1; i < length; i++)
            {
                while (api.OnReceiveBalance > 1)
                    Thread.Sleep(205);

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