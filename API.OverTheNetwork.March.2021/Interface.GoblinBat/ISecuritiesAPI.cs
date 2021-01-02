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
		string[] Account
		{
			get; set;
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
		string Securities(string param);
		void SendOrder(ISendOrder order);
		void StartProgress();
	}
}