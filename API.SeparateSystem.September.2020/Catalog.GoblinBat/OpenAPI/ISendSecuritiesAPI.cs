using System;

using ShareInvest.EventHandler;

namespace ShareInvest.Catalog.OpenAPI
{
    public interface ISendSecuritiesAPI
    {
        event EventHandler<SendSecuritiesAPI> Send;
    }
}