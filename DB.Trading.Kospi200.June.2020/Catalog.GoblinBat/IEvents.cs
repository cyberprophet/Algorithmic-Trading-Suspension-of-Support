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
}