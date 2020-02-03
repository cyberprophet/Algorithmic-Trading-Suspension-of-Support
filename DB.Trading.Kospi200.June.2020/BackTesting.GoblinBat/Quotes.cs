using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (api.RequestQueueCount == 0 && (CancelOrder == false && api.OrderNumber.Count > 0 || Difference > 1))
                for (int i = 0; i < (CancelOrder ? (api.OrderNumber.Count > 0 ? 1 : (Difference < e.Price.Length / 2 ? Difference : e.Price.Length / 2)) : api.OrderNumber.Count); i++)
                {
                    var price = GetOrderPrice(i, e.Price);
                    api.OnReceiveOrder(new PurchaseInformation
                    {
                        RQName = price,
                        ScreenNo = string.Concat(Classification, "00", i),
                        AccNo = Array.Find(specify.Account, o => o.Substring(8, 2).Equals("31")),
                        Code = specify.Code,
                        OrdKind = CancelOrder ? (api.OrderNumber.Count == 0 ? 1 : 2) : 3,
                        SlbyTP = CancelOrder ? Classification : (Classification.Equals("1") ? "2" : "1"),
                        OrdTp = ((int)PurchaseInformation.OrderType.지정가).ToString(),
                        Qty = 1,
                        Price = CancelOrder ? price : string.Empty,
                        OrgOrdNo = api.OrderNumber.Count == 0 ? string.Empty : api.OrderNumber[GetOrderIndex(i)]
                    });
                }
        }
        private string GetOrderPrice(int index, double[] price)
        {
            if (api.OrderNumber.Count > 0)
            {
                if (Classification.Equals("2"))
                {
                    if (price[4] <= api.OrderNumber.Max(o => o.Key))
                    {
                        Pursue = false;

                        return (api.OrderNumber.Min(o => o.Key) - Const.ErrorRate).ToString();
                    }
                    if (price[5] > api.OrderNumber.Max(o => o.Key))
                    {
                        Pursue = true;

                        return price[5].ToString();
                    }
                }
                if (price[5] >= api.OrderNumber.Min(o => o.Key))
                {
                    Pursue = false;

                    return (api.OrderNumber.Max(o => o.Key) + Const.ErrorRate).ToString();
                }
                if (price[4] < api.OrderNumber.Min(o => o.Key))
                {
                    Pursue = true;

                    return price[4].ToString();
                }
            }
            if (api.OrderNumber.Count == 0)
                return price[Classification.Equals("2") ? index + 5 : 4 - index].ToString();

            return "0";
        }
        private double GetOrderIndex(int index)
        {
            if (CancelOrder && (Classification.Equals("1") && Pursue || Classification.Equals("2") && Pursue == false))
                return api.OrderNumber.Max(o => o.Key);

            if (CancelOrder && (Classification.Equals("1") && Pursue == false || Classification.Equals("2") && Pursue))
                return api.OrderNumber.Min(o => o.Key);

            if (Classification.Equals("1"))
                return api.OrderNumber.Min(o => o.Key) + index * Const.ErrorRate;

            return api.OrderNumber.Max(o => o.Key) - index * Const.ErrorRate;
        }
        private bool Pursue
        {
            get; set;
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