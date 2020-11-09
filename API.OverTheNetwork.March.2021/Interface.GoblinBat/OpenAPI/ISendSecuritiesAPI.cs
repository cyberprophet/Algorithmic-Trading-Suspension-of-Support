using System;

namespace ShareInvest.Interface.OpenAPI
{
    public interface ISendSecuritiesAPI<T>
    {
        event EventHandler<T> Send;
    }
}