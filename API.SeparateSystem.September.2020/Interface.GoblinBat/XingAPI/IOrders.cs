using System;

namespace ShareInvest.Interface.XingAPI
{
    public interface IOrders<T>
    {
        void QueryExcute(IOrders order);
        event EventHandler<T> Send;
    }
    public interface IOrders
    {
        string FnoIsuNo
        {
            get;
        }
        string OrgOrdNo
        {
            get;
        }
        string BnsTpCode
        {
            get;
        }
        string FnoOrdprcPtnCode
        {
            get;
        }
        string OrdPrc
        {
            get;
        }
        string OrdQty
        {
            get;
        }
    }
}