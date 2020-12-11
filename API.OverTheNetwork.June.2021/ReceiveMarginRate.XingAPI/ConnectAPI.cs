using ShareInvest.Catalog.XingAPI;
using ShareInvest.XingAPI;

namespace ShareInvest
{
	sealed class ConnectAPI
	{
		internal ConnectAPI(dynamic param)
		{
			if (param is string[] privacy)
				API = new Connect(new Privacies
				{
					Identity = privacy[0],
					Password = privacy[1],
					Certificate = privacy[2],
					Key = privacy[^1]
				});
		}
		internal bool TryProgress() => API.Stack is not null && API.Stack.Count > 0;
		internal void StartProgress() => API.StartProgress();
		internal void Dispose() => API.Dispose();
		Connect API
		{
			get; set;
		}
	}
}