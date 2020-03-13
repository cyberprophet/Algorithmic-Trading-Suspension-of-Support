using System;

namespace ShareInvest.Catalog
{
    public interface IEvent<T>
    {
        event EventHandler<T> Send;
    }
}