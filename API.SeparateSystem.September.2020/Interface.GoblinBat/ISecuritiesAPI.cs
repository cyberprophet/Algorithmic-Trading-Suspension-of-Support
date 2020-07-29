using System;
using System.Collections.Generic;
using System.Drawing;

namespace ShareInvest.Interface
{
    public interface ISecuritiesAPI<T>
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
        IAccountInformation SetPrivacy(IAccountInformation privacy);
        event EventHandler<T> Send;
    }
    public enum SecuritiesCOM
    {
        OpenAPI = 'O',
        XingAPI = 'X'
    }
}