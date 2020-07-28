using System;
using System.Collections.Generic;
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
        HashSet<IStrategics> Strategics
        {
            get;
        }
        int SetStrategics(IStrategics strategics);
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