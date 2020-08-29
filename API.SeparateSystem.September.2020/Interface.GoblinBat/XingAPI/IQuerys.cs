using System;

namespace ShareInvest.Interface.XingAPI
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
    public interface ICharts<T>
    {
        void QueryExcute(IRetention retention);
        event EventHandler<T> Send;
    }   
}