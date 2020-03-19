using ShareInvest.Catalog.XingAPI;

namespace ShareInvest.Catalog
{
    public interface IOrders
    {
        void QueryExcute(Order order);
    }
}