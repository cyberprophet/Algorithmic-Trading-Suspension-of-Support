using System;
using System.IO;
using System.IO.Pipes;

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
        NamedPipeServerStream ConnectToReceiveRealTime
        {
            get;
        }
        StreamWriter Writer
        {
            get;
        }
        void StartProgress();
    }
}