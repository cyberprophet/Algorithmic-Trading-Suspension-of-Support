using System;
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
            Difference = difference > 5 ? 5 : difference;
            var temp = classification > 0 ? "2" : "1";
            CancelOrder = temp.Equals(Classification);
            Classification = temp;
        }
        private void OnReceiveQuotes(object sender, EventHandler.Quotes e)
        {
            if (api.OnReceiveBalance && api.RequestQueueCount == 0)
            {
                api.OnReceiveBalance = false;

                if (CancelOrder)
                {
                    if (e.OrderNumber.Count > 0 && Difference > 1)
                        SendCorrectionOrder(e.Price);

                    else if (e.OrderNumber.Count == 0 && Difference > 1)
                        SendNewOrder(e.Price, Difference >= e.Price.Length / 2 ? e.Price.Length / 2 : (int)Difference);

                    else
                        api.OnReceiveBalance = true;
                }
                else if (e.OrderNumber.Count > 0)
                    SendClearingOrder(e.OrderNumber.Count);

                else
                    api.OnReceiveBalance = true;
            }
        }
        private void SendCorrectionOrder(double[] param)
        {
            string price, number;

            if (Classification.Equals("2"))
            {
                if (param[4] <= api.OrderNumber.Max(o => o.Key))
                {
                    price = (api.OrderNumber.Min(o => o.Key) - Const.ErrorRate).ToString();
                    number = api.OrderNumber[api.OrderNumber.Max(o => o.Key)];
                }
                else if (param[5] > api.OrderNumber.Max(o => o.Key))
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
            else if (Classification.Equals("1"))
            {
                if (param[5] >= api.OrderNumber.Min(o => o.Key))
                {
                    price = (api.OrderNumber.Max(o => o.Key) + Const.ErrorRate).ToString();
                    number = api.OrderNumber[api.OrderNumber.Min(o => o.Key)];
                }
                else if (param[4] < api.OrderNumber.Min(o => o.Key))
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
            else
            {
                api.OnReceiveBalance = true;

                return;
            }
            api.OnReceiveOrder(new PurchaseInformation
            {
                RQName = price,
                ScreenNo = string.Concat(Classification, new Random().Next(1000).ToString("D3")),
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
        private void SendClearingOrder(int length)
        {
            for (int i = 0; i < length; i++)
                api.OnReceiveOrder(new PurchaseInformation
                {
                    RQName = string.Empty,
                    ScreenNo = string.Concat(Classification, "00", i),
                    AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                    Code = specify.Code,
                    OrdKind = 3,
                    SlbyTP = Classification.Equals("1") ? "2" : "1",
                    OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                    Qty = 1,
                    Price = string.Empty,
                    OrgOrdNo = api.OrderNumber[Classification.Equals("1") ? api.OrderNumber.Min(o => o.Key) + i * Const.ErrorRate : api.OrderNumber.Max(o => o.Key) - i * Const.ErrorRate]
                });
        }
        private void SendNewOrder(double[] param, int length)
        {
            for (int i = 0; i < length; i++)
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