using System;

namespace ShareInvest.Catalog.XingAPI
{
    public interface IQuerys<T>
    {
        void QueryExcute();
        event EventHandler<T> Send;
    }
    public interface IQuerys
    {
        void QueryExcute(string code);
    }
}