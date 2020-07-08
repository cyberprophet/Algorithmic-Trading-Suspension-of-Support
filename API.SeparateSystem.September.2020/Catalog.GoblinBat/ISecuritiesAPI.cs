using System;
using System.Drawing;

namespace ShareInvest.Catalog
{
    public interface ISecuritiesAPI
    {
        dynamic API
        {
            get;
        }
        void SetForeColor(Color color);
        AccountInformation SetPrivacy(Privacies privacy);
        event EventHandler<EventHandler.SendSecuritiesAPI> Send;
    }
    public enum SecuritiesCOM
    {
        OpenAPI = 'O',
        XingAPI = 'X'
    }
}