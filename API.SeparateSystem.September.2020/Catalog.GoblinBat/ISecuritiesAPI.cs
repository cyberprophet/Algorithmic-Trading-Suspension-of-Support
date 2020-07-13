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
        bool Start
        {
            get;
        }
        void StartProgress();
        void SetForeColor(Color color, string remain);
        AccountInformation SetPrivacy(Privacies privacy);
        event EventHandler<EventHandler.SendSecuritiesAPI> Send;
    }
    public enum SecuritiesCOM
    {
        OpenAPI = 'O',
        XingAPI = 'X'
    }
}