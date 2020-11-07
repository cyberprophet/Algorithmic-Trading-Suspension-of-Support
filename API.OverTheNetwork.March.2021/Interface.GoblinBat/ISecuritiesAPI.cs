using System;

namespace ShareInvest.Interface
{
    public interface ISecuritiesAPI<T>
    {
        event EventHandler<T> Send;
        dynamic API
        {
            get;
        }
        bool Start
        {
            get;
        }
        void StartProgress();
    }
}