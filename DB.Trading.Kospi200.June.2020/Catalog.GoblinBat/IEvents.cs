using System;

namespace ShareInvest.Catalog
{
    public interface IEvents<T>
    {
        event EventHandler<T> Send;
    }
    public interface IMessage<T>
    {
        event EventHandler<T> SendMessage;
    }
    public interface ITrends<T>
    {
        event EventHandler<T> SendTrend;
    }
}